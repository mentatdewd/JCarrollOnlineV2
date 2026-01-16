using JCarrollOnlineV2.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System;
using System.Web;

namespace JCarrollOnlineV2.Infrastructure
{
    /// <summary>
    /// Default implementation of IHttpContextWrapper that wraps HttpContextBase.
    /// </summary>
    public class HttpContextWrapperImpl : IHttpContextWrapper
    {
        public HttpContextWrapperImpl(HttpContextBase httpContext)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public IOwinContext GetOwinContext()
        {
            return HttpContext.GetOwinContext();
        }

        public bool IsAuthenticated => HttpContext.User?.Identity?.IsAuthenticated ?? false;

        public string GetUserId()
        {
            return HttpContext.User?.Identity?.GetUserId();
        }

        public string GetUserName()
        {
            return HttpContext.User?.Identity?.Name;
        }

        public string GetRequestUrlScheme()
        {
            return HttpContext.Request?.Url?.Scheme ?? "http";
        }

        public HttpContextBase HttpContext { get; }
    }
}
