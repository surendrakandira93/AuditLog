using AuditLog.Core.Auditing;
using AuditLog.Core.Extensions;

namespace AuditLog.Core.Auditing
{
    public class DefaultAuditInfoProvider : IAuditInfoProvider
    {
        public IClientInfoProvider ClientInfoProvider { get; set; }

        public DefaultAuditInfoProvider(IClientInfoProvider _clientInfoProvider)
        {
            ClientInfoProvider = _clientInfoProvider;
        }

        public virtual void Fill(AuditInfo auditInfo)
        {
            if (auditInfo.ClientIpAddress.IsNullOrEmpty())
            {
                auditInfo.ClientIpAddress = ClientInfoProvider.ClientIpAddress;
            }

            if (auditInfo.BrowserInfo.IsNullOrEmpty())
            {
                auditInfo.BrowserInfo = ClientInfoProvider.BrowserInfo;
            }

            if (auditInfo.ClientName.IsNullOrEmpty())
            {
                auditInfo.ClientName = ClientInfoProvider.ComputerName;
            }
        }
    }
}
