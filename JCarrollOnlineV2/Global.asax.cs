using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

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
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            //AddRoleAndUser();
        }
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<JCarrollOnlineV2Db>(null);

                try
                {
                    using (var context = new JCarrollOnlineV2Db())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("JCarrollOnlineV2Connection", "ApplicationUsers", "Id", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
