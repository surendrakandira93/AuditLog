using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    public abstract class AjaxResponseBase
    {
        
        public string TargetUrl { get; set; }

       
        public bool Success { get; set; }

       
        public ErrorInfo Error { get; set; }

      
        public bool UnAuthorizedRequest { get; set; }

      
        public bool __abp { get; } = true;
    }
}
