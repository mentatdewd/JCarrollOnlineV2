using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Home;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHomeViewModelService _homeViewModelService;

        public HomeController(IHomeViewModelService homeViewModelService)
        {
            _homeViewModelService = homeViewModelService ?? throw new ArgumentNullException(nameof(homeViewModelService));
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [HttpGet]
        public async Task<ActionResult> Index(int? microPostPage)
        {
            _logger.Info("In Home/Index");

            if (User != null && User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();
                
                try
                {
                    HomeViewModel homeViewModel = await _homeViewModelService
                        .BuildAuthenticatedHomeViewModelAsync(currentUserId, microPostPage)
                        .ConfigureAwait(false);

                    _logger.Info("Navigating to homepage (authenticated)");
                    return View(homeViewModel);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error building authenticated home view model for user {currentUserId}");
                    
                    // If there's an error, sign them out and redirect to registration
                    AuthenticationManager.SignOut();
                    return RedirectToAction("Register", "Account");
                }
            }

            // Anonymous user
            HomeViewModel anonymousViewModel = await _homeViewModelService
                .BuildAnonymousHomeViewModelAsync()
                .ConfigureAwait(false);

            _logger.Info("Navigating to homepage (anonymous)");
            return View(anonymousViewModel);
        }

        [HttpGet]
        public ActionResult About()
        {
            AboutViewModel aboutViewModel = new AboutViewModel
            {
                Message = "About JCarrollOnlineV2",
                PageContainer = "AboutPage"
            };

            return View(aboutViewModel);
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ContactViewModel contactViewModel = new ContactViewModel
            {
                Message = "JCarrollOnlineV2 Contact",
                PageContainer = "ContactPater"
            };

            return View(contactViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Welcome()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Welcome",
                PageContainer = "Welcome"
            };

            bool isAuthenticated = Request?.IsAuthenticated ?? false;

            return await Task.Run<ActionResult>(() =>
            {
                return isAuthenticated ? RedirectToAction("Index", "Home") : (ActionResult)View("Welcome", "_LayoutWelcome", homeViewModel);
            }).ConfigureAwait(false);
        }
    }
}