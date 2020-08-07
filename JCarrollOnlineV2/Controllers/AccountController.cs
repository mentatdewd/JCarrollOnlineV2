using JCarrollOnlineV2.EmailViewModels;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Account;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using Omu.ValueInjecter;
using RazorEngine.Templating;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            DeleteUserViewModel deleteUserViewModel = new DeleteUserViewModel();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            deleteUserViewModel.InjectFrom(user);

            if (user == null)
            {
                return View();
            }

            return View(deleteUserViewModel);
        }

        [HttpPost, ActionName("DeleteUser")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUserConfirmed(string userId)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View();
            }
            UserManager.Delete(user);
            return RedirectToAction("Index", "Users");
        }

        //
        // GET: /Account/JCarrollOnlineV2Service
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            _logger.Info("In Login(get)");

            return View(loginViewModel);
        }

        //
        // POST: /Account/JCarrollOnlineV2Service
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            _logger.Info("In Login(post)");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            SignInStatus result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            _logger.Info(string.Format(CultureInfo.InvariantCulture, "PasswordSignInAsync with UserName {0}, Password {1}, returned {2}", model.UserName, model.Password, result));

            switch (result)
            {
                case SignInStatus.Success:
                    if (!await UserManager.IsEmailConfirmedAsync((await UserManager.FindByNameAsync(model.UserName)).Id))
                    {
                        AuthenticationManager.SignOut();
                        ModelState.AddModelError("", "You need to confirm your email");
                        return View(model);
                    }

                    _logger.Info(string.Format(CultureInfo.InvariantCulture, "Redirecting to local {0}", returnUrl));

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", routeValues: new { ReturnUrl = returnUrl,  model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            SignInStatus result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel vm = new RegisterViewModel();

            return View(vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    Uri callbackUri = new Uri(Url.Action("ConfirmEmail", "Account", routeValues: new { userId = user.Id,  code }, protocol: Request.Url.Scheme));

//#if DEBUG
//                    var cleanUrl = callbackUri;
//#else
                    //var cleanUrl = new Uri(callbackUri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
                    //#endif

                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code });
                    await SendWelcomeEmail(user, callbackUri);
                    ApplicationUserViewModel appplicationUserViewModel = new ApplicationUserViewModel();

                    appplicationUserViewModel.InjectFrom(user);

                    //await SendWelcomeEmail(auVM, cleanUrl);

                    return RedirectToAction("RegistrationNotification", "Account");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult RegistrationNotification()
        {
            RegistrationNotificationViewModel registrationNotificationViewModel = new RegistrationNotificationViewModel();

            return View(registrationNotificationViewModel);
        }

        private async Task SendWelcomeEmail(ApplicationUser user, Uri callbackUrl)
        {
            UserWelcomeViewModel userWelcomeViewModel = GenerateViewModel(user, callbackUrl);

            await SendEmail(user, userWelcomeViewModel);
        }

        private UserWelcomeViewModel GenerateViewModel(ApplicationUser user, Uri callbackUrl)
        {
            UserWelcomeViewModel userWelcomeViewModel = new UserWelcomeViewModel();

            userWelcomeViewModel.TargetUser = user;
            userWelcomeViewModel.CallbackUrl = callbackUrl;
            return userWelcomeViewModel;
        }

        private async Task SendEmail(ApplicationUser user, UserWelcomeViewModel userWelcomeViewModel)
        {

            string templateFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            string templateFilePath = System.IO.Path.Combine(templateFolderPath, "UserWelcomePage.cshtml");
            IRazorEngineService templateService = RazorEngineService.Create();

            userWelcomeViewModel.Content = templateService.RunCompile(System.IO.File.ReadAllText(templateFilePath), "userWelcomeTemplatekey", null, userWelcomeViewModel);

            await SendEmailAsync(user.Id, userWelcomeViewModel);
        }

        public async Task SendEmailAsync(string userid, UserWelcomeViewModel userWelcomeViewModel)
        {
            IdentityMessage email = new IdentityMessage()
            {
                Body = userWelcomeViewModel.Content,
                Destination = userWelcomeViewModel.TargetUser.UserName + " " + userWelcomeViewModel.TargetUser.Email,
                Subject = "Welcome to JCarrollOnline"
            };

            await UserManager.EmailService.SendAsync(email);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            LoginConfirmationViewModel loginConfirmationViewModel = new LoginConfirmationViewModel();
            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);

            return View(result.Succeeded ? "ConfirmEmail" : "Error", loginConfirmationViewModel);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();

            return View(forgotPasswordViewModel);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await UserManager.FindByNameAsync(model.Email);
                ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ForgotPasswordConfirmationViewModel forgotPasswordConfirmationViewModel = new ForgotPasswordConfirmationViewModel();

                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation", forgotPasswordConfirmationViewModel);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string callbackUrl = Url.Action("ResetPassword", "Account", routeValues: new { userId = user.Id,  code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ForgotPasswordConfirmationViewModel forgotPasswordConfirmationViewModel = new ForgotPasswordConfirmationViewModel();

            return View(forgotPasswordConfirmationViewModel);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();

            return code == null ? View("Error") : View(resetPasswordViewModel);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            ResetPasswordConfirmationViewModel resetPasswordConfirmationViewModel = new ResetPasswordConfirmationViewModel();

            return View(resetPasswordConfirmationViewModel);
        }

        //
        // POST: /Account/ExternalLogin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            string userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            System.Collections.Generic.IList<string> userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            System.Collections.Generic.List<SelectListItem> factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", routeValues: new { Provider = model.SelectedProvider,  model.ReturnUrl,  model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("JCarrollOnlineV2Service");
            }

            // Sign in the user with this external login provider if the user already has a login
            SignInStatus result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    LoginViewModel loginViewModel = new LoginViewModel
                    {
                        ReturnUrl = returnUrl,
                        LoginProvider = loginInfo.Login.LoginProvider
                    };

                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                ExternalLoginInfo info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                ApplicationUser user = new ApplicationUser { UserName = model.SiteUserName, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            model.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = RedirectUri };

                if (UserId != null)
                {
                    properties.Dictionary[_xsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}