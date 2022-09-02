using AuditLog.Core.Auditing;
using AuditLog.Data.Auditing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace AuditLog.Auditing
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditSerializer _auditSerializer;

        public AuditActionFilter(IAuditingHelper auditingHelper,
            IAuditSerializer auditSerializer)
        {
            _auditingHelper = auditingHelper;
            _auditSerializer = auditSerializer;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }


            var auditInfo = _auditingHelper.CreateAuditInfo(
                context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                context.ActionArguments
            );

            var stopwatch = Stopwatch.StartNew();

            ActionExecutedContext result = null;
            try
            {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    auditInfo.Exception = result.Exception;                   

                    if (result.Exception is DbUpdateException)
                    {                       
                       
                        try
                        {
                            _auditingHelper.DetachAllEntities();
                        }
                        catch 
                        {

                            
                        }
                        
                    }
                }
            }      
            catch (Exception ex)
            {               
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                
                if (result != null)
                {
                    switch (result.Result)
                    {
                        case ObjectResult objectResult:
                            auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }

                await _auditingHelper.SaveAsync(auditInfo);
            }

        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return actionContext.ActionDescriptor.IsControllerAction() &&
                   _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
