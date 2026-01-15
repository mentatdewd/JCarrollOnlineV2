using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.Services;

namespace JCarrollOnlineV2.Test.Controllers
{
    /// <summary>
    /// Testable version of HomeController that allows dependency injection
    /// </summary>
    public class TestableHomeController : HomeController
    {
        public TestableHomeController(IHomeViewModelService homeViewModelService) 
            : base(homeViewModelService)
        {
            // The base constructor already handles service initialization
            // No additional setup needed
        }
    }
}
