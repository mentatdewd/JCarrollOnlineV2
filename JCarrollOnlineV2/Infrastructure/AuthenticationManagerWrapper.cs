using JCarrollOnlineV2.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Infrastructure
{
    /// <summary>
    /// Default implementation of IAuthenticationManagerWrapper that wraps IAuthenticationManager.
    /// </summary>
    public class AuthenticationManagerWrapper : IAuthenticationManagerWrapper
    {
        public AuthenticationManagerWrapper(IAuthenticationManager authenticationManager)
        {
            AuthenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
        }

        public void SignOut(params string[] authenticationTypes)
        {
            AuthenticationManager.SignOut(authenticationTypes);
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return AuthenticationManager.GetExternalLoginInfoAsync();
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf, string expectedValue)
        {
            return AuthenticationManager.GetExternalLoginInfoAsync(expectedXsrf, expectedValue);
        }

        public void Challenge(string provider, string redirectUri)
        {
            // Store the redirect URI in authentication properties
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = redirectUri };
            AuthenticationManager.Challenge(properties, provider);
        }

        public Task<bool> TwoFactorBrowserRememberedAsync(string userId)
        {
            return AuthenticationManager.TwoFactorBrowserRememberedAsync(userId);
        }

        public IAuthenticationManager AuthenticationManager { get; }
    }
}
