using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Infrastructure
{
    /// <summary>
    /// Default implementation of ISignInManagerWrapper that delegates to ApplicationSignInManager.
    /// </summary>
    public class SignInManagerWrapper : ISignInManagerWrapper
    {
        private readonly ApplicationSignInManager _signInManager;

        public SignInManagerWrapper(ApplicationSignInManager signInManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return _signInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }

        public Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            return _signInManager.SignInAsync(user, isPersistent, rememberBrowser);
        }

        public Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            return _signInManager.ExternalSignInAsync(loginInfo, isPersistent);
        }

        public Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            return _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
        }

        public Task<bool> HasBeenVerifiedAsync()
        {
            return _signInManager.HasBeenVerifiedAsync();
        }

        public Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            return _signInManager.SendTwoFactorCodeAsync(provider);
        }

        public Task<string> GetVerifiedUserIdAsync()
        {
            return _signInManager.GetVerifiedUserIdAsync();
        }

        public void SignOut()
        {
            _signInManager.AuthenticationManager.SignOut();
        }
    }
}
