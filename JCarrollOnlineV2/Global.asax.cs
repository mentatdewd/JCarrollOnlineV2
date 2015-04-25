using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace JCarrollOnlineV2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //const string adminRole = "Administrator";
        //string[] adminName = new string[1];
        //private bool AddRoleAndUser()
        //{
        //    adminName[0] = "John";
        //    JCarrollOnlineV2Db context = new JCarrollOnlineV2Db();
        //    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        //    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        //    if (!Roles.RoleExists(adminRole))
        //    {
        //        Roles.CreateRole(adminRole);
        //        Roles.AddUsersToRole(adminName, adminRole);
        //    }
            
        //    return true;
        //}
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AddRoleAndUser();
        }
    }
}
