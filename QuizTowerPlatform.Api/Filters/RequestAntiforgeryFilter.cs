using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace QuizTowerPlatform.Api.Filters
{
    /// <summary>
    /// Action filter dat Cross Site Request Forgery (xsrf) voorkomt door het RequestVerificationToken te valideren en in de response header te zetten.
    /// </summary>
    public class RequestAntiforgeryFilter : IActionFilter
    {
        private readonly IAntiforgery xsrf;
        public RequestAntiforgeryFilter(IAntiforgery xsrf)
        {
            this.xsrf = xsrf;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var method = request.Method.ToUpper();
            switch (method)
            {
                case "GET":
                case "HEAD":
                case "OPTION":
                case "TRACE":
                    return;
            }

            if ((context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo?.GetCustomAttributes(typeof(IgnoreAntiforgeryTokenAttribute), true)?.Any() ?? false)
                return;

            xsrf.ValidateRequestAsync(context.HttpContext).Wait();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var token = xsrf.GetAndStoreTokens(context.HttpContext).RequestToken;
            if (token != null)
            {
                context.HttpContext.Response.Cookies.Append("CSRF-TOKEN", token, new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false });
            }
            context.HttpContext.Response.Headers["X-CSRF-TOKEN"] = token;
            context.HttpContext.Response.Headers.AppendCommaSeparatedValues("Access-Control-Expose-Headers", "X-CSRF-TOKEN");
        }
    }
}
