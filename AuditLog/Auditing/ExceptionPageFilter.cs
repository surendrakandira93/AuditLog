using AuditLog.Core.Reflection;
using AuditLog.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;

namespace AuditLog.Auditing
{
    public class AbpExceptionPageFilter : IAsyncPageFilter
    {
       

        private readonly IErrorInfoBuilder _errorInfoBuilder;
        public AbpExceptionPageFilter(
            IErrorInfoBuilder errorInfoBuilder)
        {
            _errorInfoBuilder = errorInfoBuilder;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod == null)
            {
                await next();
                return;
            }

            var pageHandlerExecutedContext = await next();

            if (pageHandlerExecutedContext.Exception == null)
            {
                return;
            }

            var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                context.HandlerMethod.MethodInfo, new WrapResultAttribute()
            );

            if (wrapResultAttribute.LogError)
            {
                
            }

            HandleAndWrapException(pageHandlerExecutedContext, wrapResultAttribute);
        }

        protected virtual void HandleAndWrapException(PageHandlerExecutedContext context,
            WrapResultAttribute wrapResultAttribute)
        {
            if (!ActionResultHelper.IsObjectResult(context.HandlerMethod.MethodInfo.ReturnType))
            {
                return;
            }

            var displayUrl = context.HttpContext.Request.GetDisplayUrl();
            //if (_abpWebCommonModuleConfiguration.WrapResultFilters.HasFilterForWrapOnError(displayUrl,
            //    out var wrapOnError))
            //{
            //    context.HttpContext.Response.StatusCode = GetStatusCode(context, wrapOnError);

            //    if (!wrapOnError)
            //    {
            //        return;
            //    }

            //    HandleError(context);
            //    return;
            //}

            context.HttpContext.Response.StatusCode = GetStatusCode(context, wrapResultAttribute.WrapOnError);

            if (!wrapResultAttribute.WrapOnError)
            {
                return;
            }

            HandleError(context);
        }

        private void HandleError(PageHandlerExecutedContext context)
        {
            context.Result = new ObjectResult(
                new AjaxResponse(
                    _errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is AuthorizationException
                )
            );


            context.Exception = null; // Handled!
        }

        protected virtual int GetStatusCode(PageHandlerExecutedContext context, bool wrapOnError)
        {
            if (context.Exception is AuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is ValidationException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            if (wrapOnError)
            {
                return (int)HttpStatusCode.InternalServerError;
            }

            return context.HttpContext.Response.StatusCode;
        }
    }
}
