using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JCarrollOnlineV2.Startup))]
namespace JCarrollOnlineV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
