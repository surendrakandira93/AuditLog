namespace AuditLog.Core.Auditing
{
    public interface IAuditInfoProvider
    {
        void Fill(AuditInfo auditInfo);
    }
}
