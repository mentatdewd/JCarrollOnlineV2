using System.Web.Mvc;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            var errorViewModel = new ErrorViewModel
            {
                PageTitle = "Error",
                PageContainer = "container-fluid"
            };
            return View("Error", errorViewModel);
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            var errorViewModel = new ErrorViewModel
            {
                PageTitle = "Page Not Found",
                PageContainer = "container-fluid"
            };
            return View("Error", errorViewModel);
        }
    }
}