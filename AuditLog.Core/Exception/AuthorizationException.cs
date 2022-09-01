using System.Runtime.Serialization;

namespace AuditLog.Core
{
    [Serializable]
    public class AuthorizationException : Exception, IHasLogSeverity
    {
       
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;

       
        public LogSeverity Severity { get; set; }

       
        public AuthorizationException()
        {
            Severity = DefaultLogSeverity;
        }

      
        public AuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

       
        public AuthorizationException(string message)
            : base(message)
        {
            Severity = DefaultLogSeverity;
        }

       
        public AuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = DefaultLogSeverity;
        }
    }
}
