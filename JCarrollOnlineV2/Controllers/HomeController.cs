using JCarrollOnlineV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel();
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