using AuditLog.Core;
using AuditLog.Core.Auditing;
using AuditLog.Core.Extensions;
using AuditLog.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationException = AuditLog.Core.ValidationException;

namespace AuditLog.Data.Auditing
{
    [Table("AuditLogs")]
    [DisableAuditing]
    public class AuditLog : Entity<long>
    {
        
        public static int MaxServiceNameLength = 256;

        public static int MaxMethodNameLength = 256;

        public static int MaxParametersLength = 1024;

        
        public static int MaxReturnValueLength = 1024;

        public static int MaxClientIpAddressLength = 64;

        public static int MaxClientNameLength = 128;

       
        public static int MaxBrowserInfoLength = 512;

        public static int MaxExceptionMessageLength = 1024;

        public static int MaxExceptionLength = 2000;

       
        public static int MaxCustomDataLength = 2000;

       

        public virtual long? UserId { get; set; }

     
        public virtual string ServiceName { get; set; }

       
        public virtual string MethodName { get; set; }

      
        public virtual string Parameters { get; set; }

      
        public virtual string ReturnValue { get; set; }

       
        public virtual DateTime ExecutionTime { get; set; }

       
        public virtual int ExecutionDuration { get; set; }

      
        public virtual string ClientIpAddress { get; set; }

       
        public virtual string ClientName { get; set; }

       
        public virtual string BrowserInfo { get; set; }

        
        public virtual string ExceptionMessage { get; set; }

        
        public virtual string Exception { get; set; }

        
        public virtual string CustomData { get; set; }

       
        public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        {
            var exceptionMessage = GetAbpClearException(auditInfo.Exception);
            return new AuditLog
            {                
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
                Parameters = auditInfo.Parameters.TruncateWithPostfix(MaxParametersLength),
                ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(MaxReturnValueLength),
                ExecutionTime = auditInfo.ExecutionTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(MaxClientIpAddressLength),
                ClientName = auditInfo.ClientName.TruncateWithPostfix(MaxClientNameLength),
                BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(MaxBrowserInfoLength),
                Exception = exceptionMessage.TruncateWithPostfix(MaxExceptionLength),
                ExceptionMessage = auditInfo.Exception?.GetBaseException().Message.TruncateWithPostfix(MaxExceptionMessageLength),                
                CustomData = auditInfo.CustomData.TruncateWithPostfix(MaxCustomDataLength)
            };
        }

        public override string ToString()
        {
            return string.Format(
                "AUDIT LOG: {0}.{1} is executed by user {2} in {3} ms from {4} IP address.",
                ServiceName, MethodName, UserId, ExecutionDuration, ClientIpAddress
            );
        }

        public static string GetAbpClearException(Exception exception)
        {
            var clearMessage = "";
            switch (exception)
            {
                case null:
                    return null;

                case DbUpdateException dbException:
                    clearMessage = "There are " + dbException.Entries.Count + " validation errors:";
                    foreach (var entity in dbException.Entries)
                    {
                        var validationContext = new ValidationContext(entity);
                        var memberNames = "";
                        if (validationContext != null && validationContext.DisplayName.Any())
                        {
                            memberNames = " (" + string.Join(", ", validationContext.DisplayName) + ")";
                        }

                        clearMessage += "\r\n" + validationContext.Items.Values + memberNames;
                    }

                    break;

               

                case Exception exception1:
                    clearMessage = exception1.GetBaseException()?.Message;
                    break;
            }

            return exception + (clearMessage.IsNullOrWhiteSpace() ? "" : "\r\n\r\n" + clearMessage);
        }
        
    }
}
