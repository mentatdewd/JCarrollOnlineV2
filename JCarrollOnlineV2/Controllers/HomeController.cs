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
            ViewBag.PageContainer = "Home";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.PageContainer = "AboutPage";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.PageContainer = "ContactPater";
            return View();
        }

        public ActionResult Welcome()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "YourWelcome page";
                ViewBag.PageContainer = "WelcomePage";
                return View("Welcome", "_LayoutWelcome");
            }
        }
    }
}