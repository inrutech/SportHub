using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Context;
using SportHub.Repository.Model;
using SportHub.Service.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

using Zero.Core.Extensions;
using Zero.Core.Inject;
using Zero.Core.Result;

namespace SportHub.Filter
{
    /// <summary>
    /// 管理员验证过滤器，用于验证管理员身份并记录操作日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminValidAttribute() : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 在Action执行前后进行管理员身份验证和日志记录
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns>异步任务</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var service = context.HttpContext.RequestServices.GetService<AdminService>()!;

                var r = await service.ValidAuthAsync();

                if (r.OccurError)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new JsonResult(new { Code = ErrorCode.USER_CUSTOM, r.ErrorDesc });
                    return;
                }

                var method = context.HttpContext.Request.Method;

                var operateLock = method.Equals("POST", StringComparison.OrdinalIgnoreCase);
                var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

                var webCache = context.HttpContext.RequestServices.GetService<WebCache>()!;

                var coinContext = context.HttpContext.RequestServices.GetService<LoginContext<Admin>>()!;
                var key = webCache.RenderKey(Params.APP_KEY, "OperateLock", controllerActionDescriptor?.ControllerName,
                    controllerActionDescriptor?.ActionName, coinContext.LoginInfo.Id.ToString());

                if (operateLock)
                {
                    var requestId = webCache.Get<string>(key);
                    if (!requestId.IsNullOrEmpty())
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status423Locked;
                        context.Result = new JsonResult(new { Code = ErrorCode.USER_CUSTOM, ErrorDesc = "OPERATE LOCK" });
                        return;
                    }
                    webCache.SetInSeconds(key, controllerActionDescriptor?.Id, 3);
                }

                var executedContext = await next();
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<AdminValidAttribute>>();
                logger?.LogError(ex, "An exception occurred during admin validation.");
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { Code = ErrorCode.USER_CUSTOM, ErrorDesc = "Invalid Token" });
                return;
            }
        }
    }
}