using Microsoft.Owin;
using NLog;
using Owin;

[assembly: OwinStartupAttribute(typeof(JCarrollOnlineV2.Startup))]
namespace JCarrollOnlineV2
{
    public static partial class Startup
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static void Configuration(IAppBuilder app)
        {
            _logger.Info("Starting application");
            ConfigureAuthentication(app);
            app.MapSignalR();
        }
    }
}
