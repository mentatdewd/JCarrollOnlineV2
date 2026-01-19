using Autofac;
using Autofac.Integration.Mvc;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Services;
using NLog;
using System.Web.Mvc;

namespace JCarrollOnlineV2
{
    public static class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register DbContext with per-request lifetime
            builder.RegisterType<JCarrollOnlineV2DbContext>()
                .AsSelf()
                .InstancePerRequest();

            // Register services
            builder.RegisterType<UserService>()
                .As<IUserService>()
                .InstancePerRequest();

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .InstancePerRequest();

            builder.RegisterType<RssService>()
                .As<IRssService>()
                .InstancePerRequest();

            // Register ViewModel services
            builder.RegisterType<BlogViewModelService>()
                .As<IBlogViewModelService>()
                .InstancePerRequest();

            builder.RegisterType<MicroPostViewModelService>()
                .As<IMicroPostViewModelService>()
                .InstancePerRequest();

            builder.RegisterType<UserStatsViewModelService>()
                .As<IUserStatsViewModelService>()
                .InstancePerRequest();

            builder.RegisterType<HomeViewModelService>()
                .As<IHomeViewModelService>()
                .InstancePerRequest();

            builder.RegisterType<EmailService1>()
                .As<IEmailService1>()
                .InstancePerRequest();

            // Register NLog
            builder.Register(c => LogManager.GetCurrentClassLogger())
                .As<ILogger>()
                .InstancePerDependency();

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}