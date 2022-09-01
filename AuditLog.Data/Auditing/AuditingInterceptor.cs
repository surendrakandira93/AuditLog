using AuditLog.Core.Auditing;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Auditing
{
    internal class AuditingInterceptor : AbpInterceptorBase
    {
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditSerializer _auditSerializer;

        public AuditingInterceptor(
            IAuditingHelper auditingHelper,
            IAuditSerializer auditSerializer)
        {
            _auditingHelper = auditingHelper;
            _auditSerializer = auditSerializer;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                invocation.Proceed();
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
                if (invocation.ReturnValue != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(invocation.ReturnValue);
                }
                _auditingHelper.Save(auditInfo);
            }
        }

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
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

                await _auditingHelper.SaveAsync(auditInfo);
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();
            TResult result;

            try
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                result = await taskResult;

                if (result != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(result);
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

                await _auditingHelper.SaveAsync(auditInfo);
            }

            return result;
        }
    }
}
