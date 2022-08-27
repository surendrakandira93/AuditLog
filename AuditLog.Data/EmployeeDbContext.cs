using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data
{
    public class EmployeeDbContext : DbContext
    {
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
           : base(options)
        {
        }
        
        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            var result = base.SaveChanges();
            return result;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLogEntry>();
            foreach (var entry in ChangeTracker.Entries().Where(p=>p.Properties.Any(c=>c.OriginalValue!=c.CurrentValue)).ToList())
            {

                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditLogEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    AuditDetail auditDto = new AuditDetail();
                    string propertyName = property.Metadata.Name;
                    auditDto.FieldName = propertyName;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditDto.ValueBefore = null;
                            auditDto.ValueAfter = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditDto.ValueBefore = property.OriginalValue;
                            auditDto.ValueAfter = null;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditDto.ValueBefore = property.OriginalValue;
                                auditDto.ValueAfter = property.CurrentValue;
                                auditEntry.AuditType = AuditType.Update;
                            }
                            break;
                    }

                    if((auditDto.ValueBefore !=null && !auditDto.ValueBefore.Equals(auditDto.ValueAfter)) || (auditDto.ValueAfter != null && !auditDto.ValueAfter.Equals(auditDto.ValueBefore)))
                        auditEntry.Changes.Add(auditDto);
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}
