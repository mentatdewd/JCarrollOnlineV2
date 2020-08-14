using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Manage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            string userId = User.Identity.GetUserId();
            ManageIndexViewModel model = new ManageIndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId).ConfigureAwait(false),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId).ConfigureAwait(false),
                Logins = await UserManager.GetLoginsAsync(userId).ConfigureAwait(false),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId).ConfigureAwait(false)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey)).ConfigureAwait(false);
            if (result.Succeeded)
            {
                Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        [HttpGet]
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(ManageAddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            if (model != null)
            {
                string code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number).ConfigureAwait(false);
                if (UserManager.SmsService != null)
                {
                    IdentityMessage message = new IdentityMessage
                    {
                        Destination = model.Number,
                        Body = "Your security code is: " + code
                    };
                    await UserManager.SmsService.SendAsync(message).ConfigureAwait(false);
                }
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model?.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true).ConfigureAwait(false);
            Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false).ConfigureAwait(false);
            Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            // Send an SMS through the SMS provider to verify the phone number
            return await Task.Run<ActionResult>(() =>
            {
                return phoneNumber == null ? View("Error") : View(new ManageVerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
            }).ConfigureAwait(false);
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(ManageVerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model != null)
            {
                IdentityResult result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            IdentityResult result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ManageChangePasswordViewModel manageChangePasswordViewModel = new ManageChangePasswordViewModel
            {
                Message = "Change Password",
                PageContainer = "ChangePassword"
            };

            return View(manageChangePasswordViewModel);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ManageChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model != null)
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
            }
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public ActionResult SetPassword()
        {
            ManageSetPasswordViewModel manageSetPasswordViewModel = new ManageSetPasswordViewModel
            {
                PageTitle = "Set Password",
                PageContainer = "SetPassword"
            };

            return View(manageSetPasswordViewModel);
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(ManageSetPasswordViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            Entities.ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            if (user == null)
            {
                return View("Error");
            }
            System.Collections.Generic.IList<UserLoginInfo> userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            System.Collections.Generic.List<AuthenticationDescription> otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(_xsrfKey, User.Identity.GetUserId()).ConfigureAwait(false);
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login).ConfigureAwait(false);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string _xsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            Entities.ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            return user != null && user.PasswordHash != null;
        }

        private bool HasPhoneNumber()
        {
            Entities.ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            return user != null && user.PhoneNumber != null;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}