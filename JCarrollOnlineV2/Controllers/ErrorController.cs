using JCarrollOnlineV2.ViewModels.Error;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                PageTitle = "Error",
                PageContainer = "container-fluid"
            };
            return View("Error", errorViewModel);
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                PageTitle = "Page Not Found",
                PageContainer = "container-fluid"
            };
            return View("Error", errorViewModel);
        }
    }
}