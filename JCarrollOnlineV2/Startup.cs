using Microsoft.Owin;
using NLog;
using Owin;

[assembly: OwinStartupAttribute(typeof(JCarrollOnlineV2.Startup))]
namespace JCarrollOnlineV2
{
    public partial class Startup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void Configuration(IAppBuilder app)
        {
            logger.Info("Starting application");
            ConfigureAuthentication(app);
        }
    }
}
