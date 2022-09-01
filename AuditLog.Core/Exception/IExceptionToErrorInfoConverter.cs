

namespace AuditLog.Core
{
    public interface IExceptionToErrorInfoConverter
    {
        IExceptionToErrorInfoConverter Next { set; }
        ErrorInfo Convert(Exception exception);
    }
}
