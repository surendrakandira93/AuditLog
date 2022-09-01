using AuditLog.Core.Extensions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Reflection;
using AuditLog.Data.Entities;
using AuditLog.Data.Auditing;
using Microsoft.EntityFrameworkCore.Metadata;
using AuditLog.Core.Reflection;
using Microsoft.EntityFrameworkCore;
using AuditLog.Data.EntityHistory;
using AuditLog.Data.EntityHistory.Extensions;
using AuditLog.Core.Auditing;

namespace AuditLog.Core.EntityHistory
{

    public class EntityHistoryHelper : IEntityHistoryHelper
    {
        private readonly IClientInfoProvider ClientInfoProvider;

        public EntityHistoryHelper(IClientInfoProvider _clientInfoProvider)
        {            
            ClientInfoProvider = _clientInfoProvider;
        }

        public virtual EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            var changeSet = new EntityChangeSet
            {
                Reason = ClientInfoProvider.Reason.TruncateWithPostfix(EntityChangeSet.MaxReasonLength),
                BrowserInfo = ClientInfoProvider.BrowserInfo.TruncateWithPostfix(EntityChangeSet.MaxBrowserInfoLength),
                ClientIpAddress = ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityChangeSet.MaxClientIpAddressLength),
                ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(EntityChangeSet.MaxClientNameLength),
                //ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                // UserId = AbpSession.UserId,
                CreationTime = DateTime.Now
            };


            foreach (var entityEntry in entityEntries)
            {
                var shouldSaveEntityHistory = ShouldSaveEntityHistory(entityEntry);
                if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
                {
                    continue;
                }

                var entityChange = CreateEntityChange(entityEntry);
                if (entityChange == null)
                {
                    continue;
                }

                var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;
                var propertyChanges = GetPropertyChanges(entityEntry, shouldSaveAuditedPropertiesOnly);
                if (propertyChanges.Count == 0)
                {
                    continue;
                }

                entityChange.PropertyChanges = propertyChanges;
                changeSet.EntityChanges.Add(entityChange);
            }

            return changeSet;
        }

       
        public virtual EntityChangeSet UpdateEntityChangeSet(EntityChangeSet changeSet)
        {
            UpdateChangeSet(changeSet);
            return changeSet;

        }

        protected virtual string GetEntityId(EntityEntry entry)
        {
            var primaryKeys = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
            return primaryKeys.First().CurrentValue?.ToJsonString();
        }

        protected virtual bool? ShouldSaveEntityHistory(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Detached ||
                entityEntry.State == EntityState.Unchanged)
            {
                return false;
            }

            var typeOfEntity = ProxyHelper.GetUnproxiedType(entityEntry.Entity);
            var shouldTrackEntity = IsTypeOfTrackedEntity(typeOfEntity);
            if (shouldTrackEntity.HasValue && !shouldTrackEntity.Value)
            {
                return false;
            }

            if (!IsTypeOfEntity(typeOfEntity) && !entityEntry.Metadata.IsOwned())
            {
                return false;
            }

            var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
            if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
            {
                return false;
            }

            bool? shouldAuditOwnerEntity = null;
            bool? shouldAuditOwnerProperty = null;
            if (!shouldAuditEntity.HasValue && entityEntry.Metadata.IsOwned())
            {
                // Check if owner entity has auditing attribute
                var ownerForeignKey = entityEntry.Metadata.GetForeignKeys().First(fk => fk.IsOwnership);
                var ownerEntityType = ownerForeignKey.PrincipalEntityType.ClrType;

                shouldAuditOwnerEntity = IsTypeOfAuditedEntity(ownerEntityType);
                if (shouldAuditOwnerEntity.HasValue && !shouldAuditOwnerEntity.Value)
                {
                    return false;
                }

                var ownerPropertyInfo = ownerForeignKey.PrincipalToDependent.PropertyInfo;
                shouldAuditOwnerProperty = IsAuditedPropertyInfo(ownerEntityType, ownerPropertyInfo);
                if (shouldAuditOwnerProperty.HasValue && !shouldAuditOwnerProperty.Value)
                {
                    return false;
                }
            }

            return shouldAuditEntity ?? shouldAuditOwnerEntity ?? shouldAuditOwnerProperty ?? shouldTrackEntity;
        }

        protected virtual bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo == null) // Shadow properties or if mapped directly to a field
            {
                return defaultValue;
            }

            return IsAuditedPropertyInfo(propertyInfo) ?? defaultValue;
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
                var isAuditedEntity = IsTypeOfAuditedEntity(entityEntryType) == true;

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
                        var shouldSaveProperty = property.PropertyInfo == null // Shadow properties or if mapped directly to a field
                            ? null
                            : IsAuditedPropertyInfo(entityEntryType, property.PropertyInfo);

                        if (shouldSaveProperty.HasValue && !shouldSaveProperty.Value)
                        {
                            continue;
                        }

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
                    var isAuditedProperty = property.PropertyInfo != null &&
                                            (IsAuditedPropertyInfo(propertyEntityType, property.PropertyInfo) ?? false);
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

                if (!isAuditedEntity && entityChange.PropertyChanges.Count == 0)
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

        protected virtual bool? IsTypeOfAuditedEntity(Type entityType)
        {
            var entityTypeInfo = entityType.GetTypeInfo();
            if (entityTypeInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (entityTypeInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }

        protected virtual bool? IsTypeOfTrackedEntity(Type entityType)
        {
            //if (EntityHistoryConfiguration.IgnoredTypes.Any(type => type.GetTypeInfo().IsAssignableFrom(entityType)))
            //{
            //    return false;
            //}

            //if (EntityHistoryConfiguration.Selectors.Any(selector => selector.Predicate(entityType)))
            //{
            //    return true;
            //}

            //return null;
            return true;
        }

        protected virtual bool? IsAuditedPropertyInfo(Type entityType, PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            var isTrackedEntity = IsTypeOfTrackedEntity(entityType);
            var isAuditedEntity = IsTypeOfAuditedEntity(entityType);

            return (isTrackedEntity ?? false) || (isAuditedEntity ?? false);
        }

        protected virtual bool? IsAuditedPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }
    }
}
