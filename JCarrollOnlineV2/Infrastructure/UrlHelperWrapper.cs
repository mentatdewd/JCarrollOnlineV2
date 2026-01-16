using JCarrollOnlineV2.Interfaces;
using System;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Infrastructure
{
    /// <summary>
    /// Default implementation of IUrlHelperWrapper that wraps UrlHelper.
    /// </summary>
    public class UrlHelperWrapper : IUrlHelperWrapper
    {
        private readonly UrlHelper _urlHelper;

        public UrlHelperWrapper(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        public string Action(string actionName, string controllerName, object routeValues, string protocol)
        {
            return _urlHelper.Action(actionName, controllerName, routeValues, protocol);
        }

        public string Action(string actionName, string controllerName, object routeValues)
        {
            return _urlHelper.Action(actionName, controllerName, routeValues);
        }

        public string Action(string actionName, string controllerName)
        {
            return _urlHelper.Action(actionName, controllerName);
        }

        public string Action(string actionName)
        {
            return _urlHelper.Action(actionName);
        }

        public bool IsLocalUrl(string url)
        {
            return _urlHelper.IsLocalUrl(url);
        }
    }
}
