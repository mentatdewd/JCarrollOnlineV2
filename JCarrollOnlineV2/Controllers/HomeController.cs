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
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
        
        public async Task<ActionResult> Index()
        {
            HomeViewModel vm = new HomeViewModel();
            vm.MicropostCreateVM = new MicropostCreateViewModel();
            vm.MicropostFeedVM = new MicropostFeedViewModel();
            vm.UserStatsVM = new UserStatsViewModel();
            vm.UserInfoVM = new UserInfoViewModel();

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User.Identity.IsAuthenticated == true)
            {
                var user = await db.Users.FindAsync(User.Identity.GetUserId());

                vm.UserInfoVM.InjectFrom(user);

                var userId = User.Identity.GetUserId();

                vm.MicropostFeedVM.MicropostFeedItems = new List<MicropostFeedItemViewModel>();

                var microposts = await db.Microposts.Include("FollowedIds").Where(m => m.UserId == userId).ToListAsync();

                foreach (var item in microposts)
                {
                    MicropostFeedItemViewModel itemVm = new MicropostFeedItemViewModel();

                    itemVm.InjectFrom(item);
                    var emailTask = await db.Users.FindAsync(item.UserId).ContinueWith((prevTask) => itemVm.Email = prevTask.Result.Email);
                    var userNameTask = await db.Users.FindAsync(item.UserId).ContinueWith((prevTask) => itemVm.UserName = prevTask.Result.UserName);
                    
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

            vm.Message = "Your application description page.";
            vm.PageContainer = "AboutPage";
            return View(vm);
        }

        public ActionResult Contact()
        {
            ContactViewModel vm = new ContactViewModel();

            vm.Message = "Your contact page.";
            vm.PageContainer = "ContactPater";
            return View(vm);
        }

        public ActionResult Welcome()
        {
            HomeViewModel vm = new HomeViewModel();
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