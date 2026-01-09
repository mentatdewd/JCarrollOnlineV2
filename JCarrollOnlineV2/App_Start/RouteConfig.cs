using System.Web.Mvc;
using System.Web.Routing;

namespace JCarrollOnlineV2
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.Add(new ServiceRoute("Service", new ServiceHostFactory(), typeof(CalculatorService)));
            //Extra ignores to support WCF in ASP.NET MVC
            routes.IgnoreRoute("JCarrollOnlineV2.WCFService/{resource}.svc/{*pathInfo}");
            routes.IgnoreRoute("JCarrollOnlineV2.WCFService/{resource}.svc");

            // Ignore Unity WebGL build files
            routes.IgnoreRoute("Content/games/{*pathInfo}");
            routes.IgnoreRoute("Content/games/Build/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Welcome", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Google API Sign-in",
                url: "signin-google",
                defaults: new { controller = "Account", action = "Register" }
            );
        }
    }
}