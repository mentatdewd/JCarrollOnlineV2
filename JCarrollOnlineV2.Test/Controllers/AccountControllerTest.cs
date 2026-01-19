using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Infrastructure;
using JCarrollOnlineV2.Interfaces;
using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Test.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        // NOTE: Testing AccountController with Moq is challenging due to ASP.NET Identity's complex dependencies.
        // ApplicationUserManager and ApplicationSignInManager require extensive setup including:
        // - IUserStore<ApplicationUser>
        // - IAuthenticationManager (OWIN)
        // - HttpContextBase
        // - DataProtectorTokenProvider
        // 
        // These tests demonstrate the approach, but full POST method testing would require:
        // 1. Creating mock/fake implementations of Identity stores
        // 2. Mocking OWIN context and HttpContext
        // 3. Or using integration tests with a test database

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNoParameters_CreatesController()
        {
            // Act
            AccountController controller = new AccountController();

            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void Constructor_WithMockedDependencies_CreatesController()
        {
            // Arrange
            Mock<ApplicationUserManager> mockUserManager = CreateFullyMockedUserManager();
            Mock<ApplicationSignInManager> mockSignInManager = CreateFullyMockedSignInManager(mockUserManager.Object);
            Mock<EmailService1> mockEmailService = new Mock<EmailService1>();

            // Act
            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailService.Object);

            // Assert
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.UserManager);
            Assert.IsNotNull(controller.SignInManager);
        }

        #endregion

        #region Login GET Tests

        [TestMethod]
        public void Login_Get_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.Login(returnUrl: null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Login_Get_ReturnsLoginViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.Login(returnUrl: null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(LoginViewModel));
        }

        [TestMethod]
        public void Login_Get_WithReturnUrl_SetsReturnUrlInModel()
        {
            // Arrange
            AccountController controller = new AccountController();
            string returnUrl = "/Home/Index";

            // Act
            ViewResult result = controller.Login(returnUrl) as ViewResult;
            LoginViewModel model = result.Model as LoginViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(returnUrl, model.ReturnUrl);
        }

        [TestMethod]
        public void Login_Get_WithNullReturnUrl_CreatesViewModelWithNullReturnUrl()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.Login(null) as ViewResult;
            LoginViewModel model = result.Model as LoginViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.IsNull(model.ReturnUrl);
        }

        #endregion

        #region Register GET Tests

        [TestMethod]
        public void Register_Get_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.Register();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Register_Get_ReturnsRegisterViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.Register() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(RegisterViewModel));
        }

        #endregion

        #region RegistrationNotification Tests

        [TestMethod]
        public void RegistrationNotification_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.RegistrationNotification();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void RegistrationNotification_ReturnsCorrectViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.RegistrationNotification() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(RegistrationNotificationViewModel));
        }

        #endregion

        #region ForgotPassword GET Tests

        [TestMethod]
        public void ForgotPassword_Get_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ForgotPassword();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ForgotPassword_Get_ReturnsForgotPasswordViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.ForgotPassword() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForgotPasswordViewModel));
        }

        #endregion

        #region ForgotPasswordConfirmation Tests

        [TestMethod]
        public void ForgotPasswordConfirmation_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ForgotPasswordConfirmation();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ForgotPasswordConfirmation_ReturnsCorrectViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.ForgotPasswordConfirmation() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForgotPasswordConfirmationViewModel));
        }

        #endregion

        #region ResetPassword GET Tests

        [TestMethod]
        public void ResetPassword_Get_WithNullCode_ReturnsErrorView()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ResetPassword(code: null);

            // Assert
            Assert.IsNotNull(result);
            ViewResult viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Error", viewResult.ViewName);
        }

        [TestMethod]
        public void ResetPassword_Get_WithEmptyCode_ReturnsErrorView()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ResetPassword(string.Empty);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Error", viewResult.ViewName);
        }

        [TestMethod]
        public void ResetPassword_Get_WithValidCode_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();
            string code = "test-code-123";

            // Act
            ActionResult result = controller.ResetPassword(code);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.AreNotEqual("Error", viewResult.ViewName);
        }

        [TestMethod]
        public void ResetPassword_Get_WithValidCode_SetsCodeInViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();
            string code = "test-code-123";

            // Act
            ViewResult result = controller.ResetPassword(code) as ViewResult;
            ResetPasswordViewModel model = result.Model as ResetPasswordViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(code, model.Code);
            Assert.AreEqual("Reset password", model.PageTitle);
        }

        #endregion

        #region ResetPasswordConfirmation Tests

        [TestMethod]
        public void ResetPasswordConfirmation_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ResetPasswordConfirmation();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ResetPasswordConfirmation_ReturnsCorrectViewModel()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ViewResult result = controller.ResetPasswordConfirmation() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ResetPasswordConfirmationViewModel));
        }

        #endregion

        #region ExternalLoginFailure Tests

        [TestMethod]
        public void ExternalLoginFailure_ReturnsViewResult()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            ActionResult result = controller.ExternalLoginFailure();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        #endregion

        #region Helper Methods for Mocking (Examples)

        #endregion

        // NOTE: POST method tests would require extensive mocking infrastructure.
        // Here's what would be needed for each POST test:
        //
        // 1. Mock ApplicationUserManager with:
        //    - IUserStore<ApplicationUser>
        //    - IUserPasswordStore<ApplicationUser>
        //    - IUserEmailStore<ApplicationUser>
        //    - UserValidator
        //    - PasswordValidator
        //
        // 2. Mock ApplicationSignInManager with:
        //    - ApplicationUserManager
        //    - IAuthenticationManager
        //
        // 3. Mock HttpContext with:
        //    - HttpContextBase
        //    - IOwinContext
        //    - Request, Response, Session, etc.
        //
        // 4. For email-related tests:
        //    - Mock IIdentityMessageService
        //    - Mock URL generation
        //
        // RECOMMENDATION: For testing AccountController POST methods, consider:
        // - Integration tests with a test database
        // - Extracting business logic into testable service classes
        // - Using a framework like SpecFlow for BDD-style integration tests
        //
        // Example structure for Login POST test (would require full mock setup):
        //
        [TestMethod]
        public async Task Login_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            Mock<ApplicationUserManager> mockUserManager = CreateFullyMockedUserManager();
            Mock<ApplicationSignInManager> mockSignInManager = CreateFullyMockedSignInManager(mockUserManager.Object);
            Mock<EmailService1> mockEmailService = new Mock<EmailService1>();
            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailService.Object);
            controller.ModelState.AddModelError("UserName", "Required");
            LoginViewModel model = new LoginViewModel { Password = "Test123!" };

            // Act
            ViewResult result = await controller.Login(model, null) as ViewResult;



            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(LoginViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        #region Wrapper Interface Example Tests

        // NOTE: The following tests demonstrate how to use the wrapper interfaces
        // for testing scenarios that require mocking non-virtual methods.

        /// <summary>
        /// Example test showing how to use wrapper interfaces with the controller.
        /// This allows mocking of non-virtual methods like ExternalSignInAsync.
        /// </summary>
        [TestMethod]
        public void Constructor_WithWrapperInterfaces_CreatesController()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManagerWrapper = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManagerWrapper = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailServiceWrapper = new Mock<IEmailService1Wrapper>();

            // Act - Use the internal constructor that accepts wrapper interfaces
            AccountController controller = new AccountController(
                mockUserManagerWrapper.Object,
                mockSignInManagerWrapper.Object,
                mockEmailServiceWrapper.Object);

            // Assert
            Assert.IsNotNull(controller);
        }

        /// <summary>
        /// Example test showing how to use ALL wrapper interfaces for maximum testability.
        /// This allows complete control over HttpContext, Authentication, and URL generation.
        /// </summary>
        [TestMethod]
        public void Constructor_WithAllWrapperInterfaces_CreatesController()
        {
            // Arrange - Create all mocked wrappers
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IHttpContextWrapper> mockHttpContext = CreateMockedHttpContextWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuthManager = CreateMockedAuthenticationManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrlHelper = CreateMockedUrlHelperWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            // Act - Use the comprehensive constructor
            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockHttpContext.Object,
                mockAuthManager.Object,
                mockUrlHelper.Object,
                mockEmailService.Object);

            // Assert
            Assert.IsNotNull(controller);

            // Now you can test ANY controller method that uses:
            // - Identity managers (UserManager, SignInManager)
            // - HttpContext (User, Request, Response)
            // - Authentication (SignOut, GetExternalLoginInfo, etc.)
            // - URL generation (Url.Action for callbacks)
        }

        // Additional example: Testing a scenario that requires ExternalSignInAsync (non-virtual)
        // 
        // [TestMethod]
        // public async Task ExternalLogin_WithValidInfo_SignsInUser()
        // {
        //     // Arrange
        //     Mock<IUserManagerWrapper> mockUserManagerWrapper = CreateMockedUserManagerWrapper();
        //     Mock<ISignInManagerWrapper> mockSignInManagerWrapper = CreateMockedSignInManagerWrapper();
        //     
        //     // Configure specific behavior for this test
        //     mockSignInManagerWrapper
        //         .Setup(m => m.ExternalSignInAsync(It.IsAny<ExternalLoginInfo>(), false))
        //         .ReturnsAsync(SignInStatus.Success);
        //     
        //     // Create controller with wrappers
        //     AccountController controller = new AccountController(
        //         mockUserManagerWrapper.Object, 
        //         mockSignInManagerWrapper.Object);
        //     
        //     // Act & Assert
        //     // ... test logic here
        // }

        #endregion

        #region Helper Methods

        private Mock<ApplicationSignInManager> CreateFullyMockedSignInManager(ApplicationUserManager userManager)
        {
            // Mock the authentication manager
            Mock<IAuthenticationManager> mockAuthManager = new Mock<IAuthenticationManager>();

            // Create the SignInManager mock using ApplicationSignInManager, not the base SignInManager
            Mock<ApplicationSignInManager> mockSignInManager = new Mock<ApplicationSignInManager>(
                userManager,
                mockAuthManager.Object);

            // Setup common SignInManager methods with default behaviors
            mockSignInManager.Setup(m => m.PasswordSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            mockSignInManager.Setup(m => m.SignInAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Note: ExternalSignInAsync is not virtual and cannot be mocked
            // Tests that need external sign-in should use integration testing or a wrapper interface

            mockSignInManager.Setup(m => m.TwoFactorSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            // Note: The following methods are not virtual and cannot be mocked:
            // - HasBeenVerifiedAsync()
            // - SendTwoFactorCodeAsync()
            // - GetVerifiedUserIdAsync()
            // Tests that need these should use integration testing or a wrapper interface

            return mockSignInManager;
        }

        /// <summary>
        /// Creates a mock of ISignInManagerWrapper that can mock all methods (including non-virtual ones).
        /// Use this for tests that need to mock methods like ExternalSignInAsync, HasBeenVerifiedAsync, etc.
        /// </summary>
        private Mock<ISignInManagerWrapper> CreateMockedSignInManagerWrapper()
        {
            Mock<ISignInManagerWrapper> mockWrapper = new Mock<ISignInManagerWrapper>();

            // Setup common methods with default behaviors
            mockWrapper.Setup(m => m.PasswordSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            mockWrapper.Setup(m => m.SignInAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            mockWrapper.Setup(m => m.ExternalSignInAsync(
                    It.IsAny<ExternalLoginInfo>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            mockWrapper.Setup(m => m.TwoFactorSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInStatus.Success);

            mockWrapper.Setup(m => m.HasBeenVerifiedAsync())
                .ReturnsAsync(true);

            mockWrapper.Setup(m => m.SendTwoFactorCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockWrapper.Setup(m => m.GetVerifiedUserIdAsync())
                .ReturnsAsync("mock-user-id");

            mockWrapper.Setup(m => m.SignOut());

            return mockWrapper;
        }

        private Mock<ApplicationUserManager> CreateFullyMockedUserManager()
        {
            // Mock the user store
            Mock<IUserStore<ApplicationUser>> mockUserStore = new Mock<IUserStore<ApplicationUser>>();

            // Create the UserManager mock with the mocked store
            Mock<ApplicationUserManager> mockUserManager = new Mock<ApplicationUserManager>(
                mockUserStore.Object);

            // Setup common methods with default behaviors
            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockUserManager.Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("mock-reset-token");

            mockUserManager.Setup(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("mock-confirmation-token");

            mockUserManager.Setup(m => m.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            return mockUserManager;
        }

        /// <summary>
        /// Creates a mock of IUserManagerWrapper that can mock all methods.
        /// Use this for comprehensive testing of methods that may not be virtual on UserManager.
        /// </summary>
        private Mock<IUserManagerWrapper> CreateMockedUserManagerWrapper()
        {
            Mock<IUserManagerWrapper> mockWrapper = new Mock<IUserManagerWrapper>();

            // Setup common methods with default behaviors
            mockWrapper.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockWrapper.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockWrapper.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            mockWrapper.Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("mock-reset-token");

            mockWrapper.Setup(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("mock-confirmation-token");

            mockWrapper.Setup(m => m.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockWrapper.Setup(m => m.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.AddLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.RemoveLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.GetLoginsAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<UserLoginInfo>());

            mockWrapper.Setup(m => m.SetPhoneNumberAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.ChangePhoneNumberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.GetPhoneNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((string)null);

            mockWrapper.Setup(m => m.GenerateChangePhoneNumberTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("mock-phone-token");

            mockWrapper.Setup(m => m.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            mockWrapper.Setup(m => m.SetTwoFactorEnabledAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(IdentityResult.Success);

            mockWrapper.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockWrapper.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            return mockWrapper;
        }

        /// <summary>
        /// Creates a mock of IHttpContextWrapper for testing.
        /// </summary>
        private Mock<IHttpContextWrapper> CreateMockedHttpContextWrapper()
        {
            Mock<IHttpContextWrapper> mockWrapper = new Mock<IHttpContextWrapper>();

            // Setup common properties with default behaviors
            mockWrapper.Setup(m => m.IsAuthenticated).Returns(true);
            mockWrapper.Setup(m => m.GetUserId()).Returns("test-user-id");
            mockWrapper.Setup(m => m.GetUserName()).Returns("testuser");
            mockWrapper.Setup(m => m.GetRequestUrlScheme()).Returns("https");

            return mockWrapper;
        }

        /// <summary>
        /// Creates a mock of IAuthenticationManagerWrapper for testing.
        /// </summary>
        private Mock<IAuthenticationManagerWrapper> CreateMockedAuthenticationManagerWrapper()
        {
            Mock<IAuthenticationManagerWrapper> mockWrapper = new Mock<IAuthenticationManagerWrapper>();

            // Setup common methods with default behaviors
            mockWrapper.Setup(m => m.SignOut(It.IsAny<string[]>()));

            mockWrapper.Setup(m => m.GetExternalLoginInfoAsync())
                .ReturnsAsync((ExternalLoginInfo)null);

            mockWrapper.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((ExternalLoginInfo)null);

            mockWrapper.Setup(m => m.Challenge(It.IsAny<string>(), It.IsAny<string>()));

            mockWrapper.Setup(m => m.TwoFactorBrowserRememberedAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            return mockWrapper;
        }

        /// <summary>
        /// Creates a mock of IUrlHelperWrapper for testing.
        /// </summary>
        private Mock<IUrlHelperWrapper> CreateMockedUrlHelperWrapper()
        {
            Mock<IUrlHelperWrapper> mockWrapper = new Mock<IUrlHelperWrapper>();

            // Setup IsLocalUrl to return true for URLs starting with "/"
            mockWrapper.Setup(m => m.IsLocalUrl(It.IsAny<string>()))
                .Returns<string>(url => !string.IsNullOrEmpty(url) && url.StartsWith("/") && !url.StartsWith("//"));

            return mockWrapper;
        }

        #endregion

        #region Login POST Tests

        [TestMethod]
        public async Task Login_Post_WithValidCredentialsAndConfirmedEmail_ReturnsRedirectResult()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrlHelper = CreateMockedUrlHelperWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com"
            };

            mockSignInManager
                .Setup(m => m.PasswordSignInAsync("testuser", "Test123!", false, false))
                .ReturnsAsync(SignInStatus.Success);

            mockUserManager
                .Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync(testUser.Id))
                .ReturnsAsync(true);

            // Use the full constructor to provide all needed wrappers
            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,  // httpContextWrapper not needed for this test
                null,  // authenticationManagerWrapper not needed for this test
                mockUrlHelper.Object,
                mockEmailService.Object);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "Test123!",
                RememberMe = false
            };

            // Act
            ActionResult result = await controller.Login(model, "/Home/Index");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mockSignInManager.Verify(m => m.PasswordSignInAsync("testuser", "Test123!", false, false), Times.Once);
        }

        [TestMethod]
        public async Task Login_Post_WithUnconfirmedEmail_ReturnsViewWithError()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com"
            };

            mockSignInManager
                .Setup(m => m.PasswordSignInAsync("testuser", "Test123!", false, false))
                .ReturnsAsync(SignInStatus.Success);

            mockUserManager
                .Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync(testUser.Id))
                .ReturnsAsync(false);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,
                mockAuth.Object,
                null,
                mockEmailService.Object);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "Test123!",
                RememberMe = false
            };

            // Act
            ViewResult result = await controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
            mockAuth.Verify(m => m.SignOut(It.IsAny<string[]>()), Times.Once);
        }

        [TestMethod]
        public async Task Login_Post_WithLockedOutAccount_ReturnsLockoutView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.PasswordSignInAsync("testuser", "Test123!", false, false))
                .ReturnsAsync(SignInStatus.LockedOut);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            LoginViewModel model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "Test123!",
                RememberMe = false
            };

            // Act
            ViewResult result = await controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Lockout", result.ViewName);
        }

        [TestMethod]
        public async Task Login_Post_WithRequiresVerification_RedirectsToSendCode()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.PasswordSignInAsync("testuser", "Test123!", true, false))
                .ReturnsAsync(SignInStatus.RequiresVerification);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            LoginViewModel model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "Test123!",
                RememberMe = true
            };

            // Act
            ActionResult result = await controller.Login(model, "/Home/Index");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("SendCode", redirectResult.RouteValues["action"]);
        }

        [TestMethod]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.PasswordSignInAsync("testuser", "WrongPassword", false, false))
                .ReturnsAsync(SignInStatus.Failure);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            LoginViewModel model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "WrongPassword",
                RememberMe = false
            };

            // Act
            ViewResult result = await controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
        }

        #endregion

        #region Register POST Tests

        [TestMethod]
        public async Task Register_Post_WithValidModel_RedirectsToRegistrationNotification()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrl = CreateMockedUrlHelperWrapper();
            Mock<IHttpContextWrapper> mockHttp = CreateMockedHttpContextWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser createdUser = null;
            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((user, pass) =>
                {
                    createdUser = user;
                    createdUser.Id = "new-user-id";
                });

            mockUserManager
                .Setup(m => m.GenerateEmailConfirmationTokenAsync("new-user-id"))
                .ReturnsAsync("confirmation-token");

            mockUrl
                .Setup(m => m.Action("ConfirmEmail", "Account", It.IsAny<object>(), "https"))
                .Returns("https://localhost/Account/ConfirmEmail?userId=new-user-id&code=token");

            mockHttp
                .Setup(m => m.GetRequestUrlScheme())
                .Returns("https");

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockHttp.Object,
                null,
                mockUrl.Object,
                mockEmailService.Object);

            RegisterViewModel model = new RegisterViewModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // Act
            ActionResult result = await controller.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("RegistrationNotification", redirectResult.RouteValues["action"]);

            mockUserManager.Verify(m => m.CreateAsync(
                It.Is<ApplicationUser>(u => u.UserName == "newuser" && u.Email == "newuser@example.com"),
                "Test123!"), Times.Once);
        }

        [TestMethod]
        public async Task Register_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            controller.ModelState.AddModelError("Password", "Password is required");

            RegisterViewModel model = new RegisterViewModel
            {
                UserName = "newuser",
                Email = "newuser@example.com"
            };

            // Act
            ViewResult result = await controller.Register(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(RegisterViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);

            // Verify CreateAsync was never called
            mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task Register_Post_WithDuplicateUser_ReturnsViewWithErrors()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed("Username already exists"));

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            RegisterViewModel model = new RegisterViewModel
            {
                UserName = "existinguser",
                Email = "existing@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // Act
            ViewResult result = await controller.Register(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(RegisterViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        #endregion

        #region VerifyCode Tests

        [TestMethod]
        public async Task VerifyCode_Get_WhenNotVerified_ReturnsErrorView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.HasBeenVerifiedAsync())
                .ReturnsAsync(false);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.VerifyCode("Email", "/Home/Index", false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task VerifyCode_Get_WhenVerified_ReturnsViewWithModel()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.HasBeenVerifiedAsync())
                .ReturnsAsync(true);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.VerifyCode("Email", "/Home/Index", true) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(VerifyCodeViewModel));
            VerifyCodeViewModel model = result.Model as VerifyCodeViewModel;
            Assert.AreEqual("Email", model.Provider);
            Assert.AreEqual("/Home/Index", model.ReturnUrl);
            Assert.IsTrue(model.RememberMe);
        }

        [TestMethod]
        public async Task VerifyCode_Post_WithValidCode_RedirectsToReturnUrl()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrlHelper = CreateMockedUrlHelperWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.TwoFactorSignInAsync("Email", "123456", true, false))
                .ReturnsAsync(SignInStatus.Success);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null, // httpContextWrapper not needed for this test
                null, // authenticationManagerWrapper not needed for this test
                mockUrlHelper.Object,
                mockEmailService.Object);

            VerifyCodeViewModel model = new VerifyCodeViewModel
            {
                Provider = "Email",
                Code = "123456",
                RememberMe = true,
                RememberBrowser = false,
                ReturnUrl = "/Home/Index"
            };

            // Act
            ActionResult result = await controller.VerifyCode(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mockSignInManager.Verify(m => m.TwoFactorSignInAsync("Email", "123456", true, false), Times.Once);
            mockUrlHelper.Verify(m => m.IsLocalUrl("/Home/Index"), Times.Once);
        }

        [TestMethod]
        public async Task VerifyCode_Post_WithInvalidCode_ReturnsViewWithError()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.TwoFactorSignInAsync("Email", "wrong", false, false))
                .ReturnsAsync(SignInStatus.Failure);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            VerifyCodeViewModel model = new VerifyCodeViewModel
            {
                Provider = "Email",
                Code = "wrong",
                RememberMe = false,
                RememberBrowser = false,
                ReturnUrl = "/Home/Index"
            };

            // Act
            ViewResult result = await controller.VerifyCode(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
        }

        #endregion

        #region ConfirmEmail Tests

        [TestMethod]
        public async Task ConfirmEmail_WithValidToken_ReturnsConfirmEmailView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockUserManager
                .Setup(m => m.ConfirmEmailAsync("user-id-123", It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.ConfirmEmail("user-id-123", "valid-token") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ConfirmEmail", result.ViewName);
            Assert.IsInstanceOfType(result.Model, typeof(LoginConfirmationViewModel));
            mockUserManager.Verify(m => m.ConfirmEmailAsync("user-id-123", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task ConfirmEmail_WithInvalidToken_ReturnsErrorView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockUserManager
                .Setup(m => m.ConfirmEmailAsync("user-id-123", It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed("Invalid token"));

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.ConfirmEmail("user-id-123", "invalid-token") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task ConfirmEmail_WithNullUserId_ReturnsErrorView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.ConfirmEmail(null, "some-code") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
            mockUserManager.Verify(m => m.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ConfirmEmail_WithNullCode_ReturnsErrorView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            // Act
            ViewResult result = await controller.ConfirmEmail("user-id", null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
            mockUserManager.Verify(m => m.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ForgotPassword Tests

        [TestMethod]
        public async Task ForgotPassword_Post_WithValidEmail_RedirectsToConfirmation()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrl = CreateMockedUrlHelperWrapper();
            Mock<IHttpContextWrapper> mockHttp = CreateMockedHttpContextWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "user-id-123",
                Email = "test@example.com",
                UserName = "testuser"
            };

            mockUserManager
                .Setup(m => m.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync("user-id-123"))
                .ReturnsAsync(true);

            mockUserManager
                .Setup(m => m.GeneratePasswordResetTokenAsync("user-id-123"))
                .ReturnsAsync("reset-token");

            mockUrl
                .Setup(m => m.Action("ResetPassword", "Account", It.IsAny<object>(), "https"))
                .Returns("https://localhost/Account/ResetPassword?userId=user-id-123&code=token");

            mockHttp
                .Setup(m => m.GetRequestUrlScheme())
                .Returns("https");

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockHttp.Object,
                null,
                mockUrl.Object,
                mockEmailService.Object);

            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            // Act
            ActionResult result = await controller.ForgotPassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("ForgotPasswordConfirmation", redirectResult.RouteValues["action"]);

            mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync("user-id-123"), Times.Once);
        }

        [TestMethod]
        public async Task ForgotPassword_Post_WithNonExistentEmail_RedirectsToConfirmation()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockUserManager
                .Setup(m => m.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((ApplicationUser)null);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                Email = "nonexistent@example.com"
            };

            // Act
            ViewResult result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPasswordConfirmation", result.ViewName);

            // Should not generate token for non-existent user
            mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ForgotPassword_Post_WithUnconfirmedEmail_RedirectsToConfirmation()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "user-id-123",
                Email = "test@example.com"
            };

            mockUserManager
                .Setup(m => m.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync("user-id-123"))
                .ReturnsAsync(false);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            // Act
            ViewResult result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPasswordConfirmation", result.ViewName);

            // Should not generate token for unconfirmed email
            mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ForgotPassword_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            controller.ModelState.AddModelError("Email", "Email is required");

            ForgotPasswordViewModel model = new ForgotPasswordViewModel();

            // Act
            ViewResult result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForgotPasswordViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        #endregion

        #region ResetPassword Tests

        [TestMethod]
        public async Task ResetPassword_Post_WithValidData_RedirectsToConfirmation()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "user-id-123",
                Email = "test@example.com"
            };

            mockUserManager
                .Setup(m => m.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.ResetPasswordAsync("user-id-123", It.IsAny<string>(), "NewPassword123!"))
                .ReturnsAsync(IdentityResult.Success);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                Code = "valid-reset-token"
            };

            // Act
            ActionResult result = await controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("ResetPasswordConfirmation", redirectResult.RouteValues["action"]);

            mockUserManager.Verify(m => m.ResetPasswordAsync("user-id-123", It.IsAny<string>(), "NewPassword123!"), Times.Once);
        }

        [TestMethod]
        public async Task ResetPassword_Post_WithInvalidToken_ReturnsViewWithErrors()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ApplicationUser testUser = new ApplicationUser
            {
                Id = "user-id-123",
                Email = "test@example.com"
            };

            mockUserManager
                .Setup(m => m.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(testUser);

            mockUserManager
                .Setup(m => m.ResetPasswordAsync("user-id-123", It.IsAny<string>(), "NewPassword123!"))
                .ReturnsAsync(IdentityResult.Failed("Invalid token"));

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                Code = "invalid-token"
            };

            // Act
            ViewResult result = await controller.ResetPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ResetPasswordViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task ResetPassword_Post_WithNonExistentUser_RedirectsToConfirmation()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockUserManager
                .Setup(m => m.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((ApplicationUser)null);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = "nonexistent@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                Code = "some-code"
            };

            // Act
            ActionResult result = await controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("ResetPasswordConfirmation", redirectResult.RouteValues["action"]);

            // Should not attempt reset for non-existent user
            mockUserManager.Verify(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ResetPassword_Post_WithNullCode_ReturnsViewWithError()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                Code = null
            };

            // Act
            ViewResult result = await controller.ResetPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            mockUserManager.Verify(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SendCode Tests

        [TestMethod]
        public async Task SendCode_Post_WithValidProvider_RedirectsToVerifyCode()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.SendTwoFactorCodeAsync("Email"))
                .ReturnsAsync(true);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            SendCodeViewModel model = new SendCodeViewModel
            {
                SelectedProvider = "Email",
                ReturnUrl = "/Home/Index",
                RememberMe = true
            };

            // Act
            ActionResult result = await controller.SendCode(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("VerifyCode", redirectResult.RouteValues["action"]);
            mockSignInManager.Verify(m => m.SendTwoFactorCodeAsync("Email"), Times.Once);
        }

        [TestMethod]
        public async Task SendCode_Post_WithFailedSend_ReturnsErrorView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockSignInManager
                .Setup(m => m.SendTwoFactorCodeAsync("Email"))
                .ReturnsAsync(false);

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);

            SendCodeViewModel model = new SendCodeViewModel
            {
                SelectedProvider = "Email",
                ReturnUrl = "/Home/Index",
                RememberMe = false
            };

            // Act
            ViewResult result = await controller.SendCode(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task SendCode_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null, null, null, mockEmailService.Object);
            controller.ModelState.AddModelError("SelectedProvider", "Required");

            SendCodeViewModel model = new SendCodeViewModel();

            // Act
            ViewResult result = await controller.SendCode(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            mockSignInManager.Verify(m => m.SendTwoFactorCodeAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region LogOff Tests

        [TestMethod]
        public void LogOff_SignsOutUserAndRedirectsToHome()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,
                mockAuth.Object,
                null,
                mockEmailService.Object);

            // Act
            ActionResult result = controller.LogOff();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);

            mockAuth.Verify(m => m.SignOut(It.IsAny<string[]>()), Times.Once);
        }

        #endregion

        #region ExternalLogin Tests

        [TestMethod]
        public async Task ExternalLoginCallback_WithNewUser_CreatesAccountAndRedirects()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ExternalLoginInfo loginInfo = new ExternalLoginInfo
            {
                Email = "external@example.com",
                Login = new UserLoginInfo("Google", "google-id-123")
            };

            mockAuth
                .Setup(m => m.GetExternalLoginInfoAsync())
                .ReturnsAsync(loginInfo);

            mockSignInManager
                .Setup(m => m.ExternalSignInAsync(loginInfo, false))
                .ReturnsAsync(SignInStatus.Failure);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,
                mockAuth.Object,
                null,
                mockEmailService.Object);

            // Act
            ViewResult result = await controller.ExternalLoginCallback("/Home/Index") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ExternalLoginConfirmation", result.ViewName);
            Assert.IsInstanceOfType(result.Model, typeof(ExternalLoginConfirmationViewModel));

            ExternalLoginConfirmationViewModel model = result.Model as ExternalLoginConfirmationViewModel;
            Assert.AreEqual("external@example.com", model.Email);
        }

        [TestMethod]
        public async Task ExternalLoginCallback_WithExistingUser_RedirectsToReturnUrl()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IUrlHelperWrapper> mockUrlHelper = CreateMockedUrlHelperWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            ExternalLoginInfo loginInfo = new ExternalLoginInfo
            {
                Email = "external@example.com",
                Login = new UserLoginInfo("Google", "google-id-123")
            };

            mockAuth
                .Setup(m => m.GetExternalLoginInfoAsync())
                .ReturnsAsync(loginInfo);

            mockSignInManager
                .Setup(m => m.ExternalSignInAsync(loginInfo, false))
                .ReturnsAsync(SignInStatus.Success);

            mockUrlHelper
                .Setup(m => m.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,
                mockAuth.Object,
                mockUrlHelper.Object,
                mockEmailService.Object);

            // Act
            ActionResult result = await controller.ExternalLoginCallback("/Home/Index");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mockSignInManager.Verify(m => m.ExternalSignInAsync(loginInfo, false), Times.Once);
        }

        [TestMethod]
        public async Task ExternalLoginCallback_WithNoLoginInfo_RedirectsToLogin()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockAuth
                .Setup(m => m.GetExternalLoginInfoAsync())
                .ReturnsAsync((ExternalLoginInfo)null);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                null,
                mockAuth.Object,
                null,
                mockEmailService.Object);

            // Act
            ActionResult result = await controller.ExternalLoginCallback("/Home/Index");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.AreEqual("JCarrollOnlineV2Service", redirectResult.RouteValues["action"]);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_WithValidModel_CreatesUserAndRedirects()
        {
            // Arrange
            Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
            Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
            Mock<IAuthenticationManagerWrapper> mockAuth = CreateMockedAuthenticationManagerWrapper();
            Mock<IHttpContextWrapper> mockHttp = CreateMockedHttpContextWrapper();
            Mock<IUrlHelperWrapper> mockUrl = CreateMockedUrlHelperWrapper();
            Mock<IEmailService1Wrapper> mockEmailService = new Mock<IEmailService1Wrapper>();

            mockHttp.Setup(m => m.IsAuthenticated).Returns(false);

            ExternalLoginInfo loginInfo = new ExternalLoginInfo
            {
                Email = "external@example.com",
                Login = new UserLoginInfo("Google", "google-id-123")
            };

            mockAuth
                .Setup(m => m.GetExternalLoginInfoAsync())
                .ReturnsAsync(loginInfo);

            ApplicationUser createdUser = null;
            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((user, pass) =>
                {
                    createdUser = user;
                    createdUser.Id = "new-user-id";
                });

            mockUserManager
                .Setup(m => m.AddLoginAsync("new-user-id", It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            mockSignInManager
                .Setup(m => m.SignInAsync(It.IsAny<ApplicationUser>(), false, false))
                .Returns(Task.CompletedTask);

            AccountController controller = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockHttp.Object,
                mockAuth.Object,
                mockUrl.Object,
                mockEmailService.Object);

            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel
            {
                Email = "external@example.com",
                SiteUserName = "externaluser"
            };

            // Act
            ActionResult result = await controller.ExternalLoginConfirmation(model, "/Home/Index");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mockUserManager.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == "externaluser"), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(m => m.AddLoginAsync("new-user-id", It.IsAny<UserLoginInfo>()), Times.Once);
            mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<ApplicationUser>(), false, false), Times.Once);
        }

        #endregion
    }
}






