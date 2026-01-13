using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.EntityFramework;

namespace JCarrollOnlineV2.Test.Controllers
{
    /// <summary>
    /// Testable version of ForaController that allows dependency injection
    /// </summary>
    public class TestableForaController : ForaController
    {
        public TestableForaController(JCarrollOnlineV2DbContext context) : base(context)
        {
            // The base constructor already handles context initialization
            // No additional setup needed
        }
    }
}