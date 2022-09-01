using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    [Serializable]
    public class AjaxResponse : AjaxResponse<object>
    {
       
        public AjaxResponse()
        {

        }

       
        public AjaxResponse(bool success)
            : base(success)
        {

        }

      
        public AjaxResponse(object result)
            : base(result)
        {

        }

       
        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}
