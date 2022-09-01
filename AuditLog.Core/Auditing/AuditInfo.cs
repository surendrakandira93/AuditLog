using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core.Auditing
{
    public class AuditInfo
    {
        
        public long? UserId { get; set; }

        public string ServiceName { get; set; }

       
        public string MethodName { get; set; }

      
        public string Parameters { get; set; }

      
        public string ReturnValue { get; set; }

        
        public DateTime ExecutionTime { get; set; }

       
        public int ExecutionDuration { get; set; }

       
        public string ClientIpAddress { get; set; }

       
        public string ClientName { get; set; }

        
        public string BrowserInfo { get; set; }

        public string CustomData { get; set; }

        
        public Exception Exception { get; set; }

        public override string ToString()
        {
            var loggedUserId = UserId.HasValue
                                   ? "user " + UserId.Value
                                   : "an anonymous user";

            var exceptionOrSuccessMessage = Exception != null
                ? "exception: " + Exception.Message
                : "succeed";

            return $"AUDIT LOG: {ServiceName}.{MethodName} is executed by {loggedUserId} in {ExecutionDuration} ms from {ClientIpAddress} IP address with {exceptionOrSuccessMessage}.";
        }
    }
}
