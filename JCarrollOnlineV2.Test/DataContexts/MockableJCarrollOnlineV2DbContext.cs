using JCarrollOnlineV2.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace JCarrollOnlineV2.Test.DataContexts
{
    /// <summary>
    /// Mockable DbContext for unit testing with Moq
    /// </summary>
    public class MockableJCarrollOnlineV2DbContext : JCarrollOnlineV2DbContext
    {
        public MockableJCarrollOnlineV2DbContext() : base()
        {
            // Disable database initialization for testing
            Database.SetInitializer<MockableJCarrollOnlineV2DbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Don't call base to avoid model validation issues in tests
            if (modelBuilder == null)
            {
                throw new System.ArgumentNullException(nameof(modelBuilder));
            }
            // Minimal configuration - tests will set up DbSets directly
        }
    }
}
