using Microsoft.Owin;
using System.Web;

namespace JCarrollOnlineV2.Interfaces
{
    /// <summary>
    /// Wrapper interface for HttpContext operations to enable unit testing.
    /// </summary>
    public interface IHttpContextWrapper
    {
        /// <summary>
        /// Gets the current OWIN context.
        /// </summary>
        IOwinContext GetOwinContext();

        /// <summary>
        /// Gets whether the current request is authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the current user's ID from claims.
        /// </summary>
        string GetUserId();

        /// <summary>
        /// Gets the current user's name.
        /// </summary>
        string GetUserName();

        /// <summary>
        /// Gets the request URL scheme (http/https).
        /// </summary>
        string GetRequestUrlScheme();

        /// <summary>
        /// Gets the base HttpContext for cases where direct access is needed.
        /// </summary>
        HttpContextBase HttpContext { get; }
    }
}
