using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Auditing
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
