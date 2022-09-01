
using AuditLog.Core;
using AuditLog.Core.EntityHistory;
using AuditLog.Core.Extensions;
using AuditLog.Core.Reflection;
using AuditLog.Data.Auditing;
using AuditLog.Data.Entities;
using AuditLog.Data.EntityHistory;
using AuditLog.Data.EntityHistory.Extensions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data
{
    public class EmployeeDbContext : DbContext
    {
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<EntityChange> EntityChanges { get; set; }
        public virtual DbSet<EntityChangeSet> EntityChangeSets { get; set; }
        public virtual DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }

        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        IClientInfoProvider ClientInfoProvider;
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options, IEntityHistoryHelper entityHistoryHelper)
           : base(options)
        {
            EntityHistoryHelper = entityHistoryHelper;
        }



        public override int SaveChanges()
        {
            EntityChangeSet changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            int result = base.SaveChanges();
            if (changeSet.EntityChanges.Any())
            {
                changeSet = EntityHistoryHelper.UpdateEntityChangeSet(changeSet);
                if (changeSet.EntityChanges.Any())
                {
                    this.EntityChangeSets.Add(changeSet);
                    base.SaveChanges();
                }                
            }


            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            EntityChangeSet changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            if (changeSet.EntityChanges.Any())
            {
                changeSet = EntityHistoryHelper.UpdateEntityChangeSet(changeSet);
                if (changeSet.EntityChanges.Any())
                {
                   await this.EntityChangeSets.AddAsync(changeSet);
                  await base.SaveChangesAsync();
                }
            }

            return result;
        }

        protected virtual string GetEntityId(EntityEntry entry)
        {
            var primaryKeys = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
            return primaryKeys.First().CurrentValue?.ToJsonString();
        }
        protected virtual bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo == null) // Shadow properties or if mapped directly to a field
            {
                return defaultValue;
            }

            return defaultValue;
        }

        [CanBeNull]
        private EntityChange CreateEntityChange(EntityEntry entityEntry)
        {
            var entityId = GetEntityId(entityEntry);
            var entityTypeFullName = ProxyHelper.GetUnproxiedType(entityEntry.Entity).FullName;
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                case EntityState.Modified:
                    changeType = entityEntry.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return null;
                default:
                    return null;
            }

            if (entityId == null && changeType != EntityChangeType.Created)
            {
                return null;
            }

            return new EntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeFullName = entityTypeFullName
            };
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChange> GetPropertyChanges(EntityEntry entityEntry,
            bool auditedPropertiesOnly)
        {
            var propertyChanges = new List<EntityPropertyChange>();
            var properties = entityEntry.Metadata.GetProperties();

            foreach (var property in properties)
            {
                if (property.IsPrimaryKey())
                {
                    continue;
                }

                var propertyEntry = entityEntry.Property(property.Name);

                if (ShouldSavePropertyHistory(propertyEntry, !auditedPropertiesOnly))
                {

                    propertyChanges.Add(
                        CreateEntityPropertyChange(
                            propertyEntry.GetOriginalValue(),
                            propertyEntry.GetNewValue(),
                            property
                        )
                    );
                }
            }

            return propertyChanges;
        }

        /// <summary>
        /// Updates change time, entity id, Adds foreign keys, Removes/Updates property changes after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(EntityChangeSet changeSet)
        {
            var entityChangesToRemove = new List<EntityChange>();
            foreach (var entityChange in changeSet.EntityChanges)
            {
                var entityEntry = entityChange.EntityEntry.As<EntityEntry>();
                var entityEntryType = ProxyHelper.GetUnproxiedType(entityEntry.Entity);

                /* Update change time */
                entityChange.ChangeTime = GetChangeTime(entityChange.ChangeType, entityEntry.Entity);

                /* Update entity id */
                entityChange.EntityId = GetEntityId(entityEntry);

                /* Update property changes */
                var trackedPropertyNames = entityChange.PropertyChanges.Select(pc => pc.PropertyName).ToList();

                var additionalForeignKeys = entityEntry.Metadata.GetDeclaredReferencingForeignKeys()
                                                    .Where(fk => trackedPropertyNames.Contains(fk.Properties[0].Name))
                                                    .ToList();

                /* Add additional foreign keys from navigation properties */
                foreach (var foreignKey in additionalForeignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {

                        var propertyEntry = entityEntry.Property(property.Name);

                        var newValue = propertyEntry.GetNewValue()?.ToJsonString();
                        var oldValue = propertyEntry.GetOriginalValue()?.ToJsonString();

                        // Add foreign key
                        entityChange.PropertyChanges.Add(CreateEntityPropertyChange(oldValue, newValue, property));
                    }
                }

                /* Update/Remove property changes */
                var propertyChangesToRemove = new List<EntityPropertyChange>();
                var foreignKeys = entityEntry.Metadata.GetForeignKeys();
                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    var propertyEntry = entityEntry.Property(propertyChange.PropertyName);

                    // Take owner entity type if this is an owned entity
                    var propertyEntityType = entityEntryType;
                    if (entityEntry.Metadata.IsOwned())
                    {
                        var ownerForeignKey = foreignKeys.First(fk => fk.IsOwnership);
                        propertyEntityType = ownerForeignKey.PrincipalEntityType.ClrType;
                    }
                    var property = propertyEntry.Metadata;
                    var isAuditedProperty = property.PropertyInfo != null;
                    var isForeignKeyShadowProperty = property.IsShadowProperty() && foreignKeys.Any(fk => fk.Properties.Any(p => p.Name == propertyChange.PropertyName));

                    propertyChange.SetNewValue(propertyEntry.GetNewValue()?.ToJsonString());
                    if ((!isAuditedProperty && !isForeignKeyShadowProperty) || propertyChange.IsValuesEquals())
                    {
                        // No change
                        propertyChangesToRemove.Add(propertyChange);
                    }
                }

                foreach (var propertyChange in propertyChangesToRemove)
                {
                    entityChange.PropertyChanges.Remove(propertyChange);
                }

                if (entityChange.PropertyChanges.Count == 0)
                {
                    entityChangesToRemove.Add(entityChange);
                }
            }

            foreach (var entityChange in entityChangesToRemove)
            {
                changeSet.EntityChanges.Remove(entityChange);
            }
        }

        private EntityPropertyChange CreateEntityPropertyChange(object oldValue, object newValue, IProperty property)
        {
            var entityPropertyChange = new EntityPropertyChange()
            {
                PropertyName = property.Name.TruncateWithPostfix(EntityPropertyChange.MaxPropertyNameLength),
                PropertyTypeFullName = property.ClrType.FullName.TruncateWithPostfix(
                    EntityPropertyChange.MaxPropertyTypeFullNameLength
                )
            };

            entityPropertyChange.SetNewValue(newValue?.ToJsonString());
            entityPropertyChange.SetOriginalValue(oldValue?.ToJsonString());
            return entityPropertyChange;
        }

        protected virtual DateTime GetChangeTime(EntityChangeType entityChangeType, object entity)
        {
            switch (entityChangeType)
            {
                case EntityChangeType.Created:
                    return (entity as IHasCreationTime)?.CreationTime ?? DateTime.Now;
                case EntityChangeType.Deleted:
                    return (entity as IHasDeletionTime)?.DeletionTime ?? DateTime.Now;
                case EntityChangeType.Updated:
                    return (entity as IHasModificationTime)?.LastModificationTime ?? DateTime.Now;
                default:
                    return DateTime.Now;
            }
        }
        protected virtual bool IsTypeOfEntity(Type entityType)
        {
            return EntityHelper.IsEntity(entityType) && entityType.IsPublic;
        }


        //private void OnBeforeSaveChanges()
        //{
        //    ChangeTracker.DetectChanges();
        //    var auditEntries = new List<AuditLogEntry>();
        //    foreach (var entry in ChangeTracker.Entries().Where(p => p.Properties.Any(c => c.OriginalValue != c.CurrentValue)).ToList())
        //    {

        //        if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //            continue;
        //        var auditEntry = new AuditLogEntry(entry);
        //        auditEntry.TableName = entry.Entity.GetType().Name;
        //        auditEntries.Add(auditEntry);
        //        foreach (var property in entry.Properties)
        //        {
        //            AuditDetail auditDto = new AuditDetail();
        //            string propertyName = property.Metadata.Name;
        //            auditDto.FieldName = propertyName;
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues = property.CurrentValue;
        //                continue;
        //            }
        //            switch (entry.State)
        //            {
        //                case EntityState.Added:
        //                    auditEntry.AuditType = AuditType.Create;
        //                    auditDto.ValueBefore = null;
        //                    auditDto.ValueAfter = property.CurrentValue;
        //                    break;
        //                case EntityState.Deleted:
        //                    auditEntry.AuditType = AuditType.Delete;
        //                    auditDto.ValueBefore = property.OriginalValue;
        //                    auditDto.ValueAfter = null;
        //                    break;
        //                case EntityState.Modified:
        //                    if (property.IsModified)
        //                    {
        //                        auditDto.ValueBefore = property.OriginalValue;
        //                        auditDto.ValueAfter = property.CurrentValue;
        //                        auditEntry.AuditType = AuditType.Update;
        //                    }
        //                    break;
        //            }

        //            if ((auditDto.ValueBefore != null && !auditDto.ValueBefore.Equals(auditDto.ValueAfter)) || (auditDto.ValueAfter != null && !auditDto.ValueAfter.Equals(auditDto.ValueBefore)))
        //                auditEntry.Changes.Add(auditDto);
        //        }
        //    }
        //    foreach (var auditEntry in auditEntries)
        //    {
        //        AuditLogs.Add(auditEntry.ToAudit());
        //    }
        //}
    }
}
