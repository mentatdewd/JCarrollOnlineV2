using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.EntityFramework;

namespace JCarrollOnlineV2.Test.Controllers
{
    /// <summary>
    /// Testable version of ForaController that allows dependency injection
    /// </summary>
    public class TestableForaController : ForaController
    {
        public TestableForaController(JCarrollOnlineV2DbContext context)
        {
            // Use reflection to set the private Data property
            var dataProperty = typeof(ForaController).GetProperty("Data",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dataProperty.SetValue(this, context);
        }
    }
}