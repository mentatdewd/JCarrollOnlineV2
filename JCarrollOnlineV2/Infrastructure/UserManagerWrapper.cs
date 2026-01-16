using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Infrastructure
{
    /// <summary>
    /// Default implementation of IUserManagerWrapper that delegates to ApplicationUserManager.
    /// </summary>
    public class UserManagerWrapper : IUserManagerWrapper
    {
        private readonly ApplicationUserManager _userManager;

        public UserManagerWrapper(ApplicationUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            return _userManager.GeneratePasswordResetTokenAsync(userId);
        }

        public Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            return _userManager.ResetPasswordAsync(userId, token, newPassword);
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            return _userManager.GenerateEmailConfirmationTokenAsync(userId);
        }

        public Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            return _userManager.ConfirmEmailAsync(userId, token);
        }

        public Task<bool> IsEmailConfirmedAsync(string userId)
        {
            return _userManager.IsEmailConfirmedAsync(userId);
        }

        public Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(userId, currentPassword, newPassword);
        }

        public Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return _userManager.AddLoginAsync(userId, login);
        }

        public Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login)
        {
            return _userManager.RemoveLoginAsync(userId, login);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return _userManager.GetLoginsAsync(userId);
        }

        public Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            return _userManager.SetPhoneNumberAsync(userId, phoneNumber);
        }

        public Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token)
        {
            return _userManager.ChangePhoneNumberAsync(userId, phoneNumber, token);
        }

        public Task<string> GetPhoneNumberAsync(string userId)
        {
            return _userManager.GetPhoneNumberAsync(userId);
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber)
        {
            return _userManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
        }

        public Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            return _userManager.GetTwoFactorEnabledAsync(userId);
        }

        public Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            return _userManager.SetTwoFactorEnabledAsync(userId, enabled);
        }

        public Task SendEmailAsync(string userId, string subject, string body)
        {
            return _userManager.SendEmailAsync(userId, subject, body);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return _userManager.UpdateAsync(user);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            return _userManager.DeleteAsync(user);
        }

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            return _userManager.GetValidTwoFactorProvidersAsync(userId);
        }
    }
}
