using System.Web.Mvc;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var handleErrorInfo = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var model = new ErrorViewModel(handleErrorInfo);

            filterContext.Result = new ViewResult
            {
                ViewName = View,
                ViewData = new ViewDataDictionary<ErrorViewModel>(model),
                TempData = filterContext.Controller.TempData
            };

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}