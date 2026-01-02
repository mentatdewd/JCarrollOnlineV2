using JCarrollOnlineV2.DataContexts;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JCarrollOnlineV2
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc")]
    public class MvcApplication : System.Web.HttpApplication
    {
#pragma warning disable CA1822 // Mark members as static If this is declared static, IIS complains that there is no default document
        protected void Application_Start()
#pragma warning restore CA1822 // Mark members as static
        {
            // Enable TLS 1.2 and TLS 1.3 for HTTPS requests
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.Write(@"
<html><head><title>HTML Not Allowed</title>
<script language='JavaScript'><!--
function back() { history.go(-1); } //--></script></head>
<body style='font-family: Arial, Sans-serif;'>
<h1>Oops!</h1>
<p>I'm sorry, but HTML entry is not allowed on that page.</p>
<p>Please make sure that your entries do not contain 
any angle brackets like &lt; or &gt;.</p>
<p><a href='javascript:back()'>Go back</a></p>
</body></html>
");
                Response.End();
            }
        }
    }
}
