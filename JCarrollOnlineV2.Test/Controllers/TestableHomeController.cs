using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Services;

namespace JCarrollOnlineV2.Test.Controllers
{
    /// <summary>
    /// Testable version of HomeController that allows dependency injection
    /// </summary>
    public class TestableHomeController : HomeController
    {
        public TestableHomeController(JCarrollOnlineV2DbContext context, IRssService rssService) 
            : base(context, rssService)
        {
            // The base constructor already handles context and service initialization
            // No additional setup needed
        }
    }
}
