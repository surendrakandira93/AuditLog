using AuditLog.Core.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Auditing
{
    public interface IAuditingStore
    {
        
        Task SaveAsync(AuditInfo auditInfo);

       
        void Save(AuditInfo auditInfo);
    }
}
