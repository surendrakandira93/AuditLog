using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core
{
    public interface IHasLogSeverity
    {
       
        LogSeverity Severity { get; set; }
    }
}
