using AuditLog.Core.Auditing;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Auditing
{
    public class AuditingStore : IAuditingStore
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;

        public AuditingStore(IRepository<AuditLog, long> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public virtual void DetachAllEntities()
        {
            _auditLogRepository.DetachAllEntities();
        }

        public virtual async Task SaveAsync(AuditInfo auditInfo)
        {
            _auditLogRepository.InsertAsync(AuditLog.CreateFromAuditInfo(auditInfo));
            await _auditLogRepository.SaveChangesAsync();

        }

        public virtual void Save(AuditInfo auditInfo)
        {
            _auditLogRepository.Insert(AuditLog.CreateFromAuditInfo(auditInfo));
            _auditLogRepository.SaveChanges();
        }
    }
}
