namespace JCarrollOnlineV2.Interfaces
{
    /// <summary>
    /// Wrapper interface for URL generation operations to enable unit testing.
    /// </summary>
    public interface IUrlHelperWrapper
    {
        /// <summary>
        /// Generates a fully qualified URL for an action method.
        /// </summary>
        string Action(string actionName, string controllerName, object routeValues, string protocol);

        /// <summary>
        /// Generates a URL for an action method.
        /// </summary>
        string Action(string actionName, string controllerName, object routeValues);

        /// <summary>
        /// Generates a URL for an action method.
        /// </summary>
        string Action(string actionName, string controllerName);

        /// <summary>
        /// Generates a URL for an action method.
        /// </summary>
        string Action(string actionName);

        /// <summary>
        /// Determines whether the specified URL is local to the application.
        /// </summary>
        bool IsLocalUrl(string url);
    }
}
