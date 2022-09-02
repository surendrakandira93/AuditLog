using AuditLog.Core.Reflection;
using AuditLog.Dto;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using AuditLog.Core;
using AuditLog.Data.Auditing;

namespace AuditLog.Auditing
{
    public class ExceptionFilter : IExceptionFilter
    {      

        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IAuditingHelper _auditingHelper;
        public ExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAuditingHelper auditingHelper)
        {
            _errorInfoBuilder = errorInfoBuilder;
            _auditingHelper = auditingHelper;
        }

        public void OnException(ExceptionContext context)
        {
            //if (!context.ActionDescriptor.IsControllerAction())
            //{
            //    return;
            //}

            var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(context.ActionDescriptor.GetMethodInfo(), new WrapResultAttribute());           

            HandleAndWrapException(context, wrapResultAttribute);
        }

        protected virtual void HandleAndWrapException(ExceptionContext context, WrapResultAttribute wrapResultAttribute)
        {
            //if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            //{
            //    return;
            //}

            //var displayUrl = context.HttpContext.Request.GetDisplayUrl();
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

        private void HandleError(ExceptionContext context)
        {
            
            context.Result = new ObjectResult(
                new AjaxResponse(
                    _errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is AuthorizationException
                )
            );
            context.Exception = null; // Handled!
        }

        protected virtual int GetStatusCode(ExceptionContext context, bool wrapOnError)
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
