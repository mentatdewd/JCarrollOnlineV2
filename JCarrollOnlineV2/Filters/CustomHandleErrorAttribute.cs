using JCarrollOnlineV2.ViewModels.Error;
using System.Web.Mvc;

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

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];

            // Map MVC HandleErrorInfo to our POCO
            ErrorInfo errorInfo = new ErrorInfo
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ExceptionType = filterContext.Exception?.GetType().Name,
                Message = filterContext.Exception?.Message
            };
            
            ErrorViewModel model = new ErrorViewModel(errorInfo);

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