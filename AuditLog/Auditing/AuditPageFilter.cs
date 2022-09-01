using AuditLog.Core.Auditing;
using AuditLog.Data.Auditing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AuditLog.Dto;
using AuditLog.Core;

namespace AuditLog.Auditing
{
    public class AuditPageFilter : IAsyncPageFilter
    {
        
        private readonly IAuditingHelper _auditingHelper;        
        private readonly IAuditSerializer _auditSerializer;

        public AuditPageFilter(IAuditingHelper auditingHelper,            
            IAuditSerializer auditSerializer)
        {            
            _auditingHelper = auditingHelper;         
            _auditSerializer = auditSerializer;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod == null || !ShouldSaveAudit(context))
            {
                await next();
                return;
            }

           
                var auditInfo = _auditingHelper.CreateAuditInfo(
                    context.HandlerInstance.GetType(),
                    context.HandlerMethod.MethodInfo,
                    context.GetBoundPropertiesAsDictionary()
                );

                var stopwatch = Stopwatch.StartNew();

                PageHandlerExecutedContext result = null;
                try
                {
                    result = await next();
                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
                }
                catch (Exception ex)
                {
                    auditInfo.Exception = ex;
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
                                if (objectResult.Value is AjaxResponse ajaxObjectResponse)
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxObjectResponse.Result);
                                }
                                else
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                                }
                                break;

                            case JsonResult jsonResult:
                                if (jsonResult.Value is AjaxResponse ajaxJsonResponse)
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxJsonResponse.Result);
                                }
                                else
                                {
                                    auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
                                }
                                break;

                            case ContentResult contentResult:
                                auditInfo.ReturnValue = contentResult.Content;
                                break;
                        }
                    }

                    await _auditingHelper.SaveAsync(auditInfo);
                }
            
        }

        private bool ShouldSaveAudit(PageHandlerExecutingContext actionContext)
        {
            return _auditingHelper.ShouldSaveAudit(actionContext.HandlerMethod.MethodInfo, true);
        }
    }
}
