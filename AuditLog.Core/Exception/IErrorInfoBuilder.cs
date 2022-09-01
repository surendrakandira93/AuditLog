using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core
{
    public interface IErrorInfoBuilder
    {
        
        ErrorInfo BuildForException(Exception exception);

      
        void AddExceptionConverter(IExceptionToErrorInfoConverter converter);
    }
}
