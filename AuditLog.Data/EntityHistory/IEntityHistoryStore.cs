using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.EntityHistory
{
    public interface IEntityHistoryStore
    {
        
        Task SaveAsync(EntityChangeSet entityChangeSet);

      
        void Save(EntityChangeSet entityChangeSet);
    }
}
