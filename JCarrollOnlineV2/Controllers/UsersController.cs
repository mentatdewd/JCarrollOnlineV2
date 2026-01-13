using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UsersController(IUserService userService, IEmailService emailService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // GET: Users
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();
                List<UserItemViewModel> users = await _userService.GetAllUsersAsync(currentUserId).ConfigureAwait(false);

                UsersIndexViewModel viewModel = new UsersIndexViewModel
                {
                    PageTitle = "Users"
                };

                foreach (UserItemViewModel user in users)
                {
                    viewModel.Users.Add(user);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading users index");
                TempData["Error"] = "An error occurred while loading users.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Users/SendMassEmail
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMassEmail(UsersIndexViewModel model)
        {
            ModelState.Remove("Users");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide both a subject and message.";
                return RedirectToAction("Index");
            }

            try
            {
                EmailResult result = await _emailService.SendMassEmailAsync(
                    model.EmailSubject,
                    model.EmailBody,
                    model.IsHtml
                ).ConfigureAwait(false);

                if (result.SuccessCount == 0)
                {
                    TempData["Error"] = "No emails were sent. Please check the logs.";
                }
                else
                {
                    TempData["Success"] = $"Mass email sent successfully to {result.SuccessCount} users." +
                        (result.FailureCount > 0 ? $" {result.FailureCount} emails failed." : "");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending mass email");
                TempData["Error"] = "Error sending mass email. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        // GET: Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string userId)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();
                userId = userId ?? currentUserId;

                UserDetailViewModel viewModel = await _userService.GetUserDetailsAsync(userId, currentUserId).ConfigureAwait(false);

                if (viewModel == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading user details for userId: {userId}");
                TempData["Error"] = "An error occurred while loading user details.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Follow([Bind(Include = "UserId")] UserItemViewModel followUser)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(followUser?.UserId))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Index");
            }

            try
            {
                string currentUserId = User.Identity.GetUserId();
                bool success = await _userService.FollowUserAsync(currentUserId, followUser.UserId).ConfigureAwait(false);

                if (!success)
                {
                    TempData["Error"] = "Unable to follow user.";
                }

                return RedirectToAction("Details", new { userid = followUser.UserId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error following user {followUser?.UserId}");
                TempData["Error"] = "An error occurred while trying to follow this user.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unfollow([Bind(Include = "UserId")] UserItemViewModel followUser)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(followUser?.UserId))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Index");
            }

            try
            {
                string currentUserId = User.Identity.GetUserId();
                bool success = await _userService.UnfollowUserAsync(currentUserId, followUser.UserId).ConfigureAwait(false);

                if (!success)
                {
                    TempData["Error"] = "Unable to unfollow user.";
                }

                return RedirectToAction("Details", new { userid = followUser.UserId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error unfollowing user {followUser?.UserId}");
                TempData["Error"] = "An error occurred while trying to unfollow this user.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserSettings([Bind(Include = "UserId,MicroPostEmailNotifications,MicroPostSmsNotifications")] UserItemViewModel userItemViewModel)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(userItemViewModel?.UserId))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Index");
            }

            try
            {
                bool success = await _userService.UpdateUserSettingsAsync(
                    userItemViewModel.UserId,
                    userItemViewModel.MicroPostEmailNotifications,
                    userItemViewModel.MicroPostSmsNotifications
                ).ConfigureAwait(false);

                if (!success)
                {
                    TempData["Error"] = "Unable to update settings.";
                }
                else
                {
                    TempData["Success"] = "Settings updated successfully.";
                }

                return RedirectToAction("Details", new { userid = userItemViewModel.UserId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error updating user settings for {userItemViewModel?.UserId}");
                TempData["Error"] = "An error occurred while updating settings.";
                return RedirectToAction("Index");
            }
        }

        // Following and Followed methods omitted for brevity - follow same pattern
    }
}
