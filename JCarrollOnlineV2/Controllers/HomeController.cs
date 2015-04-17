using JCarrollOnlineV2.ControllerHelpers;
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

            Task<RssFeedViewModel> rss = Forums.UpdateRssAsync();
            //vm.RssFeedVM = await Forums.UpdateRssAsync();

            var user = db.Users.Find(User.Identity.GetUserId());

            vm.UserInfoVM.InjectFrom(user);

            var userId = User.Identity.GetUserId();

            vm.MicropostFeedVM.MicropostFeedItems = new List<MicropostFeedItemViewModel>();

            var microposts = db.Microposts.Include("FollowedIds").Where(m => m.UserId == userId);
            foreach(var item in microposts)
            {
                MicropostFeedItemViewModel itemVm = new MicropostFeedItemViewModel();

                itemVm.InjectFrom(item);
                itemVm.Email = db.Users.Find(item.UserId).Email;
                itemVm.UserName = db.Users.Find(item.UserId).UserName;
                vm.MicropostFeedVM.MicropostFeedItems.Add(itemVm);
            }

            vm.PageContainer = "Home";
            //vm.RssFeedVM = rss.Wait();
            vm.RssFeedVM = await rss;
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