using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core
{
    public enum EntityChangeType : byte
    {
        Created = 0,
        Updated = 1,
        Deleted = 2
    }
}
