using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Test.DataContexts
{
    /// <summary>
    /// Fake DbContext for unit testing that doesn't require a database connection
    /// </summary>
    public class FakeJCarrollOnlineV2DbContext : JCarrollOnlineV2DbContext
    {
        public int SaveChangesCallCount { get; private set; }
        public int SaveChangesAsyncCallCount { get; private set; }

        public FakeJCarrollOnlineV2DbContext() : base()
        {
            // Disable database initialization for testing
            Database.SetInitializer<FakeJCarrollOnlineV2DbContext>(null);
            SaveChangesCallCount = 0;
            SaveChangesAsyncCallCount = 0;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Don't call base.OnModelCreating to avoid model validation issues
            // with Identity types that aren't being used in tests
            
            if (modelBuilder == null)
            {
                throw new System.ArgumentNullException(nameof(modelBuilder));
            }

            // Only configure the entities we're actually testing
            modelBuilder.Entity<Forum>()
                .HasKey(f => f.Id)
                .ToTable("Forum");

            modelBuilder.Entity<ForumModerator>()
                .HasKey(fm => fm.Id)
                .ToTable("ForumModerator");

            modelBuilder.Entity<ThreadEntry>()
                .HasKey(te => te.Id)
                .ToTable("ForumThreadEntry");

            modelBuilder.Entity<MicroPost>()
                .HasKey(mp => mp.Id)
                .ToTable("MicroPost");

            modelBuilder.Entity<BlogItem>()
                .HasKey(bi => bi.Id)
                .ToTable("BlogItem");

            modelBuilder.Entity<BlogItemComment>()
                .HasKey(bic => bic.Id)
                .ToTable("BlogItemComment");

            modelBuilder.Entity<Entities.NLog>()
                .HasKey(k => k.Id)
                .ToTable("NLog");

            modelBuilder.Entity<ChatMessage>()
                .HasKey(cm => cm.Id)
                .ToTable("ChatMessage");

            modelBuilder.Entity<ApplicationUser>()
                .HasKey(m => m.Id);

            // Configure Identity types that are exposed as DbSets
            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("IdentityUserLogin");

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("IdentityUserRole");

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>()
                .HasKey(r => r.Id)
                .ToTable("IdentityRole");

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim>()
                .HasKey(c => c.Id)
                .ToTable("IdentityUserClaim");
        }

        public override int SaveChanges()
        {
            // Track calls for testing
            SaveChangesCallCount++;
            
            // Simulate ID generation for new entities
            SimulateIdGeneration();
            
            return 1;
        }

        public override Task<int> SaveChangesAsync()
        {
            // Track calls for testing
            SaveChangesAsyncCallCount++;
            
            // Simulate ID generation for new entities
            SimulateIdGeneration();
            
            return Task.FromResult(1);
        }

        private void SimulateIdGeneration()
        {
            // Generate IDs for entities that don't have one (Id = 0)
            var addedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .ToList();

            foreach (var entry in addedEntities)
            {
                var entity = entry.Entity;
                var idProperty = entity.GetType().GetProperty("Id");
                
                if (idProperty != null && idProperty.PropertyType == typeof(int))
                {
                    var currentId = (int)idProperty.GetValue(entity);
                    if (currentId == 0)
                    {
                        // Find the max ID for this entity type
                        var dbSetProperty = GetType().GetProperties()
                            .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                               p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                               p.PropertyType.GetGenericArguments()[0] == entity.GetType());

                        if (dbSetProperty != null)
                        {
                            var dbSet = dbSetProperty.GetValue(this) as IEnumerable<object>;
                            if (dbSet != null)
                            {
                                var maxId = 0;
                                foreach (var item in dbSet)
                                {
                                    var itemId = (int)idProperty.GetValue(item);
                                    if (itemId > maxId)
                                    {
                                        maxId = itemId;
                                    }
                                }
                                idProperty.SetValue(entity, maxId + 1);
                            }
                        }
                    }
                }
            }
        }
    }
}
