using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Interfaces
{
    /// <summary>
    /// Wrapper interface for UserManager methods that are commonly used in controllers.
    /// This allows for better testability of controllers that depend on UserManager.
    /// </summary>
    public interface IUserManagerWrapper
    {
        /// <summary>
        /// Creates a new user with the specified password.
        /// </summary>
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        /// <summary>
        /// Finds a user by username.
        /// </summary>
        Task<ApplicationUser> FindByNameAsync(string userName);

        /// <summary>
        /// Finds a user by email.
        /// </summary>
        Task<ApplicationUser> FindByEmailAsync(string email);

        /// <summary>
        /// Finds a user by ID.
        /// </summary>
        Task<ApplicationUser> FindByIdAsync(string userId);

        /// <summary>
        /// Generates a password reset token for a user.
        /// </summary>
        Task<string> GeneratePasswordResetTokenAsync(string userId);

        /// <summary>
        /// Resets a user's password using a token.
        /// </summary>
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        /// <summary>
        /// Generates an email confirmation token for a user.
        /// </summary>
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);

        /// <summary>
        /// Confirms a user's email using a token.
        /// </summary>
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        /// <summary>
        /// Checks if a user's email is confirmed.
        /// </summary>
        Task<bool> IsEmailConfirmedAsync(string userId);

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>
        /// Adds a login to a user.
        /// </summary>
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        /// <summary>
        /// Removes a login from a user.
        /// </summary>
        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login);

        /// <summary>
        /// Gets the logins for a user.
        /// </summary>
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

        /// <summary>
        /// Sets the phone number for a user.
        /// </summary>
        Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber);

        /// <summary>
        /// Changes a user's phone number using a verification token.
        /// </summary>
        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token);

        /// <summary>
        /// Gets the phone number for a user.
        /// </summary>
        Task<string> GetPhoneNumberAsync(string userId);

        /// <summary>
        /// Generates a change phone number token.
        /// </summary>
        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

        /// <summary>
        /// Gets whether two-factor authentication is enabled for a user.
        /// </summary>
        Task<bool> GetTwoFactorEnabledAsync(string userId);

        /// <summary>
        /// Sets whether two-factor authentication is enabled for a user.
        /// </summary>
        Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled);

        /// <summary>
        /// Sends an email to a user.
        /// </summary>
        Task SendEmailAsync(string userId, string subject, string body);

        /// <summary>
        /// Updates a user.
        /// </summary>
        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        /// <summary>
        ///     Deletes a user.
        /// </summary>
        Task<IdentityResult> DeleteAsync(ApplicationUser user);

        /// <summary>
        /// Asynchronously retrieves a list of valid two-factor authentication providers for the specified user.
        /// </summary>
        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);
    }
}
