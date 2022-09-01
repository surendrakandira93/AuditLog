
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

    }
}
