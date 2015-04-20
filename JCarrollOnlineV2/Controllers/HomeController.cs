using JCarrollOnlineV2;
using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        private IContext _data { get; set; }

        public HomeController() : this(null)
        {
        }

        public HomeController(IContext dataContext = null)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        public async Task<ActionResult> Index()
        {
            HomeViewModel vm = new HomeViewModel();
            vm.Message = "JCarrollOnlineV2 Home - Index";
            vm.MicropostCreateVM = new MicropostCreateViewModel();
            vm.MicropostFeedVM = new MicropostFeedViewModel();
            vm.UserStatsVM = new UserStatsViewModel();
            vm.UserInfoVM = new UserInfoViewModel();

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User !=  null && User.Identity.IsAuthenticated == true)
            {
                var user = await _data.Users.FindAsync(User.Identity.GetUserId());

                vm.UserInfoVM.InjectFrom(user);

                var userId = User.Identity.GetUserId();

                vm.MicropostFeedVM.MicropostFeedItems = new List<MicropostFeedItemViewModel>();

                var microposts = await _data.Microposts.Include("FollowedIds").Where(m => m.UserId == userId).ToListAsync();

                foreach (var item in microposts)
                {
                    MicropostFeedItemViewModel itemVm = new MicropostFeedItemViewModel();

                    itemVm.InjectFrom(item);
                    var emailTask = await _data.Users.FindAsync(item.UserId).ContinueWith((prevTask) => itemVm.Email = prevTask.Result.Email);
                    var userNameTask = await _data.Users.FindAsync(item.UserId).ContinueWith((prevTask) => itemVm.UserName = prevTask.Result.UserName);
                    
                    vm.MicropostFeedVM.MicropostFeedItems.Add(itemVm);
                }
                vm.RssFeedVM = await rss;
            }
            vm.PageContainer = "Home";
            return View(vm);
        }

        public ActionResult About()
        {
            AboutViewModel vm = new AboutViewModel();

            vm.Message = "About JCarrollOnlineV2";
            vm.PageContainer = "AboutPage";
            return View(vm);
        }

        public ActionResult Contact()
        {
            ContactViewModel vm = new ContactViewModel();

            vm.Message = "JCarrollOnlineV2 Contact";
            vm.PageContainer = "ContactPater";
            return View(vm);
        }

        public async Task<ActionResult> Welcome()
        {
            HomeViewModel vm = new HomeViewModel();
            vm.Message = "JCarrollOnlineV2 Home - Welcome";
            vm.PageContainer = "Welcome";
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Welcome", "_LayoutWelcome", vm);
            }
        }
   }
}