using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Interfaces
{
    /// <summary>
    /// Wrapper interface for OWIN Authentication Manager operations to enable unit testing.
    /// </summary>
    public interface IAuthenticationManagerWrapper
    {
        /// <summary>
        /// Signs out the current user for the specified authentication types.
        /// </summary>
        void SignOut(params string[] authenticationTypes);

        /// <summary>
        /// Gets external login information for the current request.
        /// </summary>
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        /// <summary>
        /// Gets external login information with an expected XSrf key.
        /// </summary>
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf, string expectedValue);

        /// <summary>
        /// Creates a challenge result for external authentication.
        /// </summary>
        /// <param name="provider">The authentication provider.</param>
        /// <param name="redirectUri">The redirect URI after authentication.</param>
        void Challenge(string provider, string redirectUri);

        /// <summary>
        /// Checks if two-factor authentication cookie is remembered for the current browser.
        /// </summary>
        Task<bool> TwoFactorBrowserRememberedAsync(string userId);

        /// <summary>
        /// Gets the underlying IAuthenticationManager for advanced scenarios.
        /// </summary>
        IAuthenticationManager AuthenticationManager { get; }
    }
}
