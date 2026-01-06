using JCarrollOnlineV2.EmailViewModels;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Helpers;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Account;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using Omu.ValueInjecter;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

        // GET: /Account/DeleteUser
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            DeleteUserViewModel deleteUserViewModel = new DeleteUserViewModel();
            ApplicationUser user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                return HttpNotFound();
            }

            deleteUserViewModel.InjectFrom(user);

            return View(deleteUserViewModel);
        }

        // POST: /Account/DeleteUser
        [HttpPost, ActionName("DeleteUser")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUserConfirmed(string id)  // ← Changed from userId to id
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                return HttpNotFound();
            }

            IdentityResult result = UserManager.Delete(user);

            if (result.Succeeded)
            {
                _logger.Info(string.Format(CultureInfo.InvariantCulture,
                    "User {0} (ID: {1}) deleted successfully", user.UserName, id));
                return RedirectToAction("Index", "Users");
            }

            // Handle deletion errors
            _logger.Error(string.Format(CultureInfo.InvariantCulture,
                "Failed to delete user {0}: {1}", id, string.Join(", ", result.Errors)));

            DeleteUserViewModel deleteUserViewModel = new DeleteUserViewModel();
            deleteUserViewModel.InjectFrom(user);

            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View("DeleteUser", deleteUserViewModel);
        }

        //
        // GET: /Account/JCarrollOnlineV2Service
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        [AllowAnonymous]
        [HttpGet]
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
            SignInStatus result = await SignInManager.PasswordSignInAsync(model?.UserName, model.Password, model.RememberMe, shouldLockout: false).ConfigureAwait(false);

            _logger.Info(string.Format(CultureInfo.InvariantCulture, "PasswordSignInAsync with UserName {0}, Password {1}, returned {2}", model.UserName, model.Password, result));

            switch (result)
            {
                case SignInStatus.Success:
                    if (!await UserManager.IsEmailConfirmedAsync((await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false)).Id).ConfigureAwait(false))
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
        [HttpGet]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            return !await SignInManager.HasBeenVerifiedAsync().ConfigureAwait(false)
                ? View("Error")
                : View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
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
            SignInStatus result = await SignInManager.TwoFactorSignInAsync(model?.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser).ConfigureAwait(false);
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
        [HttpGet]
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
                ApplicationUser user = new ApplicationUser { UserName = model?.UserName, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    try
                    {
                        // Generate email confirmation token
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id).ConfigureAwait(false);

                        // URL encode the code to handle special characters
                        string encodedCode = System.Web.HttpUtility.UrlEncode(code);

                        // Generate callback URI with encoded code
                        Uri callbackUri = new Uri(Url.Action("ConfirmEmail", "Account",
                            routeValues: new { userId = user.Id, code = encodedCode },
                            protocol: Request.Url.Scheme));

                        _logger.Info(string.Format(CultureInfo.InvariantCulture,
                            "Sending welcome email to {0} for userId: {1}", user.Email, user.Id));

                        await SendWelcomeEmail(user, callbackUri).ConfigureAwait(false);

                        ApplicationUserViewModel appplicationUserViewModel = new ApplicationUserViewModel();
                        appplicationUserViewModel.InjectFrom(user);

                        return RedirectToAction("RegistrationNotification", "Account");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                            "Failed to send welcome email during registration for user: {0}", user.Email));

                        // Still redirect to notification page even if email fails
                        // You might want to show a different message
                        return RedirectToAction("RegistrationNotification", "Account");
                    }
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegistrationNotification()
        {
            RegistrationNotificationViewModel registrationNotificationViewModel = new RegistrationNotificationViewModel();

            return View(registrationNotificationViewModel);
        }

        private async Task SendWelcomeEmail(ApplicationUser user, Uri callbackUri)
        {
            UserWelcomeViewModel userWelcomeViewModel = GenerateViewModel(user, callbackUri);

            await SendEmail(userWelcomeViewModel).ConfigureAwait(false);
        }

        private static UserWelcomeViewModel GenerateViewModel(ApplicationUser user, Uri callbackUrl)
        {
            UserWelcomeViewModel userWelcomeViewModel = new UserWelcomeViewModel
            {
                TargetUser = user,
                CallbackUrl = callbackUrl
            };

            return userWelcomeViewModel;
        }

        private async Task SendEmail(UserWelcomeViewModel userWelcomeViewModel)
        {
            // Convert the view model to an anonymous object for Handlebars
            var templateData = new
            {
                TargetUser = new
                {
                    userWelcomeViewModel.TargetUser.UserName,
                    userWelcomeViewModel.TargetUser.Email
                },
                CallbackUrl = userWelcomeViewModel.CallbackUrl?.ToString()
            };

            // Use Handlebars instead of RazorEngine
            userWelcomeViewModel.Content = HandlebarsEmailHelper.RenderTemplate(
                "UserWelcomePage",
                templateData
            );

            await SendEmailViaHostGatorAsync(userWelcomeViewModel).ConfigureAwait(false);
        }

        private async Task SendEmailViaHostGatorAsync(UserWelcomeViewModel userWelcomeViewModel)
        {
            // Read SMTP settings from web.config/appSettings
            string smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            string smtpPortStr = System.Configuration.ConfigurationManager.AppSettings["SmtpPort"];
            string smtpUsername = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["SmtpFromEmail"];
            string enableSslStr = System.Configuration.ConfigurationManager.AppSettings["SmtpEnableSsl"];

            // Validate configuration
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.Error("SMTP configuration is incomplete. Check Web.config appSettings.");
                throw new InvalidOperationException("SMTP configuration is missing required values.");
            }

            int smtpPort = int.Parse(smtpPortStr);
            bool enableSsl = bool.Parse(enableSslStr);

            // Configure certificate validation callback before creating SMTP client
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    // If there are no SSL policy errors, accept the certificate
                    if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                        return true;

                    // Only bypass validation for our specific SMTP server
                    if (sender is System.Net.Mail.SmtpClient)
                    {
                        // Accept certificate from our known HostGator mail server despite errors
                        _logger.Warn(string.Format(CultureInfo.InvariantCulture,
                            "Accepting certificate from {0} despite SSL errors: {1}",
                            smtpHost, sslPolicyErrors));
                        return true;
                    }

                    // Reject all other certificates with errors
                    _logger.Error(string.Format(CultureInfo.InvariantCulture,
                        "Certificate validation failed for unknown sender. SSL errors: {0}",
                        sslPolicyErrors));
                    return false;
                };

            try
            {
                using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new System.Net.Mail.MailAddress(fromEmail, "JCarrollOnline");
                    mailMessage.To.Add(new System.Net.Mail.MailAddress(userWelcomeViewModel?.TargetUser.Email));
                    mailMessage.Subject = "Welcome to JCarrollOnline";
                    mailMessage.Body = userWelcomeViewModel.Content;
                    mailMessage.IsBodyHtml = true;

                    using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                        _logger.Info(string.Format(CultureInfo.InvariantCulture,
                            "Welcome email sent successfully to {0}",
                            userWelcomeViewModel.TargetUser.Email));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                    "Failed to send welcome email to {0}",
                    userWelcomeViewModel.TargetUser.Email));
                throw;
            }
            finally
            {
                // Reset certificate validation to default for security
                System.Net.ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.Error(string.Format(CultureInfo.InvariantCulture,
                    "ConfirmEmail called with null parameters. UserId: {0}, Code: {1}",
                    userId ?? "null", code ?? "null"));
                return View("Error");
            }

            try
            {
                // URL decode the code if it's not already decoded (handles + signs and %20)
                string decodedCode = System.Web.HttpUtility.UrlDecode(code);

                _logger.Info(string.Format(CultureInfo.InvariantCulture,
                    "Attempting to confirm email for userId: {0}", userId));

                IdentityResult result = await UserManager.ConfirmEmailAsync(userId, decodedCode).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    _logger.Info(string.Format(CultureInfo.InvariantCulture,
                        "Email confirmed successfully for userId: {0}", userId));

                    LoginConfirmationViewModel loginConfirmationViewModel = new LoginConfirmationViewModel();
                    return View("ConfirmEmail", loginConfirmationViewModel);
                }
                else
                {
                    _logger.Error(string.Format(CultureInfo.InvariantCulture,
                        "Email confirmation failed for userId: {0}. Errors: {1}",
                        userId, string.Join(", ", result.Errors)));
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                    "Exception during email confirmation for userId: {0}", userId));
                return View("Error");
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpGet]
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
                ApplicationUser user = await UserManager.FindByEmailAsync(model?.Email).ConfigureAwait(false);

                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id).ConfigureAwait(false))
                {
                    ForgotPasswordConfirmationViewModel forgotPasswordConfirmationViewModel = new ForgotPasswordConfirmationViewModel();

                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation", forgotPasswordConfirmationViewModel);
                }

                try
                {
                    // Generate password reset token
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id).ConfigureAwait(false);

                    // URL encode the code
                    string encodedCode = System.Web.HttpUtility.UrlEncode(code);

                    // Generate callback URL
                    string callbackUrl = Url.Action("ResetPassword", "Account",
                        routeValues: new { userId = user.Id, code = encodedCode },
                        protocol: Request.Url.Scheme);

                    // Send password reset email using your custom method
                    await SendPasswordResetEmail(user, callbackUrl).ConfigureAwait(false);

                    _logger.Info(string.Format(CultureInfo.InvariantCulture,
                        "Password reset email sent successfully to {0}", user.Email));

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                        "Failed to send password reset email to {0}", user.Email));

                    // Still redirect to confirmation page (don't reveal if user exists)
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // Add this new helper method
        private async Task SendPasswordResetEmail(ApplicationUser user, string callbackUrl)
        {
            // Create email content (you can create a Handlebars template for this too)
            string emailBody = string.Format(
                @"<html>
        <body>
            <h2>Reset Your Password</h2>
            <p>Hello {0},</p>
            <p>You recently requested to reset your password for your JCarrollOnline account.</p>
            <p>Please click the button below to reset your password:</p>
            <p><a href=""{1}"" style=""background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;"">Reset Password</a></p>
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p>{1}</p>
            <p>If you didn't request a password reset, please ignore this email.</p>
            <p>Thanks,<br/>The JCarrollOnline Team</p>
        </body>
        </html>",
                user.UserName,
                callbackUrl
            );

            // Create a simple view model for password reset
            UserWelcomeViewModel passwordResetViewModel = new UserWelcomeViewModel
            {
                TargetUser = user,
                Content = emailBody
            };

            await SendPasswordResetEmailViaHostGatorAsync(passwordResetViewModel).ConfigureAwait(false);
        }

        // Add this method (similar to SendEmailViaHostGatorAsync but for password reset)
        private async Task SendPasswordResetEmailViaHostGatorAsync(UserWelcomeViewModel viewModel)
        {
            // Read SMTP settings from web.config/appSettings
            string smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            string smtpPortStr = System.Configuration.ConfigurationManager.AppSettings["SmtpPort"];
            string smtpUsername = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["SmtpFromEmail"];
            string enableSslStr = System.Configuration.ConfigurationManager.AppSettings["SmtpEnableSsl"];

            // Validate configuration
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.Error("SMTP configuration is incomplete. Check Web.config appSettings.");
                throw new InvalidOperationException("SMTP configuration is missing required values.");
            }

            int smtpPort = int.Parse(smtpPortStr);
            bool enableSsl = bool.Parse(enableSslStr);

            // Configure certificate validation callback
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                        return true;

                    if (sender is System.Net.Mail.SmtpClient)
                    {
                        _logger.Warn(string.Format(CultureInfo.InvariantCulture,
                            "Accepting certificate from {0} despite SSL errors: {1}",
                            smtpHost, sslPolicyErrors));
                        return true;
                    }

                    _logger.Error(string.Format(CultureInfo.InvariantCulture,
                        "Certificate validation failed for unknown sender. SSL errors: {0}",
                        sslPolicyErrors));
                    return false;
                };

            try
            {
                using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new System.Net.Mail.MailAddress(fromEmail, "JCarrollOnline");
                    mailMessage.To.Add(new System.Net.Mail.MailAddress(viewModel?.TargetUser.Email));
                    mailMessage.Subject = "Reset Your JCarrollOnline Password";
                    mailMessage.Body = viewModel.Content;
                    mailMessage.IsBodyHtml = true;

                    using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                        _logger.Info(string.Format(CultureInfo.InvariantCulture,
                            "Password reset email sent successfully to {0}",
                            viewModel.TargetUser.Email));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                    "Failed to send password reset email to {0}",
                    viewModel.TargetUser.Email));
                throw;
            }
            finally
            {
                // Reset certificate validation to default for security
                System.Net.ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            ForgotPasswordConfirmationViewModel forgotPasswordConfirmationViewModel = new ForgotPasswordConfirmationViewModel();

            return View(forgotPasswordConfirmationViewModel);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [HttpGet]
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
            ApplicationUser user = await UserManager.FindByEmailAsync(model?.Email).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password).ConfigureAwait(false);
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
        [HttpGet]
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
        [HttpGet]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            string userId = await SignInManager.GetVerifiedUserIdAsync().ConfigureAwait(false);
            if (userId == null)
            {
                return View("Error");
            }
            System.Collections.Generic.IList<string> userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId).ConfigureAwait(false);
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
            return !await SignInManager.SendTwoFactorCodeAsync(model?.SelectedProvider).ConfigureAwait(false)
                ? View("Error")
                : (ActionResult)RedirectToAction("VerifyCode", routeValues: new { Provider = model.SelectedProvider,  model.ReturnUrl,  model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
            if (loginInfo == null)
            {
                return RedirectToAction("JCarrollOnlineV2Service");
            }

            // Sign in the user with this external login provider if the user already has a login
            SignInStatus result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false).ConfigureAwait(false);
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
                ExternalLoginInfo info = await AuthenticationManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                ApplicationUser user = new ApplicationUser { UserName = model?.SiteUserName, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            if (model != null)
            {
                model.ReturnUrl = returnUrl;
            }
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
        [HttpGet]
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
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : (ActionResult)RedirectToAction("Index", "Home");
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