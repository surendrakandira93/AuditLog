using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core
{
    [Serializable]
    public class ValidationException : Exception, IHasLogSeverity
    {
       
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;

       
        public IList<ValidationResult> ValidationErrors { get; set; }

      
        public LogSeverity Severity { get; set; }

        
        public ValidationException()
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        
        public ValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

       
        public ValidationException(string message)
            : base(message)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        
        public ValidationException(string message, IList<ValidationResult> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
            Severity = DefaultLogSeverity;
        }

        
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }
    }
}
