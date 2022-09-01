using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using AuditLog.Core.Auditing;

namespace AuditLog.Auditing
{
    public class HttpContextClientInfoProvider : IClientInfoProvider
    {
        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public string ComputerName => GetComputerName();

        public string Reason => GetReason();

        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;
               
        public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected virtual string GetReason()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request.GetDisplayUrl();
        }

        protected virtual string GetBrowserInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request?.Headers?["User-Agent"];
        }

        protected virtual string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Connection?.RemoteIpAddress?.ToString();

            }
            catch (Exception ex)
            {
                
            }

            return null;
        }

        protected virtual string GetComputerName()
        {
            try
            {
                return Dns.GetHostEntry(GetClientIpAddress()).HostName;
            }
            catch
            {
                return null;
            }


        }

        
    }
}
