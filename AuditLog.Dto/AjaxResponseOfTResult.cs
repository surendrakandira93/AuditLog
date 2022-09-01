using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    [Serializable]
    public class AjaxResponse<TResult> : AjaxResponseBase
    {
        public TResult Result { get; set; }

      
        public AjaxResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

       
        public AjaxResponse()
        {
            Success = true;
        }

       
        public AjaxResponse(bool success)
        {
            Success = success;
        }      
       
        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }
}
