using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core
{
    public interface IHasErrorCode
    {
        int Code { get; set; }
    }
}
