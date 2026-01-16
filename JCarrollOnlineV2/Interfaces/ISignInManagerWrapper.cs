using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Interfaces
{
    /// <summary>
    /// Wrapper interface for SignInManager methods that are not virtual and cannot be mocked directly.
    /// This allows for better testability of controllers that depend on SignInManager.
    /// </summary>
    public interface ISignInManagerWrapper
    {
        /// <summary>
        /// Signs in a user with a password.
        /// </summary>
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);

        /// <summary>
        /// Signs in a user.
        /// </summary>
        Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser);

        /// <summary>
        /// Signs in a user using external login information.
        /// </summary>
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

        /// <summary>
        /// Signs in a user using two-factor authentication.
        /// </summary>
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

        /// <summary>
        /// Checks if the user has been verified for two-factor authentication.
        /// </summary>
        Task<bool> HasBeenVerifiedAsync();

        /// <summary>
        /// Sends a two-factor authentication code to the user.
        /// </summary>
        Task<bool> SendTwoFactorCodeAsync(string provider);

        /// <summary>
        /// Gets the verified user ID for two-factor authentication.
        /// </summary>
        Task<string> GetVerifiedUserIdAsync();

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        void SignOut();
    }
}
