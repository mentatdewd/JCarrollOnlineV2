using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Test.DataContexts;
using JCarrollOnlineV2.ViewModels.Fora;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Test.Controllers
{
    [TestClass]
    public class ForaControllerIntegrationTest
    {
        private FakeJCarrollOnlineV2DbContext _testContext;
        private List<Forum> _testForumData;

        [TestInitialize]
        public void Setup()
        {
            // Setup test context with fake DbSets
            _testContext = new FakeJCarrollOnlineV2DbContext();
            
            // Initialize test data
            _testForumData = new List<Forum>
            {
                new Forum
                {
                    Id = 1,
                    Title = "Integration Test Forum 1",
                    Description = "First test forum",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                new Forum
                {
                    Id = 2,
                    Title = "Integration Test Forum 2",
                    Description = "Second test forum",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now
                }
            };

            // Setup fake Forum DbSet
            var fakeForumSet = new FakeJCarrollOnlineV2Db<Forum>();
            foreach (var forum in _testForumData)
            {
                fakeForumSet.Add(forum);
            }

            // Replace the Forum DbSet with our fake
            var forumProperty = typeof(JCarrollOnlineV2DbContext).GetProperty("Forum");
            forumProperty.SetValue(_testContext, fakeForumSet);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _testContext?.Dispose();
        }

        [TestMethod]
        public async Task FullCRUD_Workflow_CreatesEditsAndDeletesForum()
        {
            // Arrange
            var controller = new TestableForaController(_testContext);
            
            // CREATE
            var createViewModel = new ForaCreateViewModel
            {
                Title = "CRUD Test Forum",
                Description = "Forum for testing full CRUD workflow"
            };

            // Act - Create
            var createResult = await controller.Create(createViewModel).ConfigureAwait(false) as RedirectToRouteResult;
            
            // Assert - Create
            Assert.IsNotNull(createResult);
            var createdForum = _testContext.Forum.Local.FirstOrDefault(f => f.Title == "CRUD Test Forum");
            Assert.IsNotNull(createdForum);
            int createdForumId = createdForum.Id;

            // EDIT
            createdForum.Title = "Updated CRUD Test Forum";
            createdForum.Description = "Updated description";

            // Act - Edit
            var editResult = await controller.Edit(createdForum).ConfigureAwait(false) as RedirectToRouteResult;

            // Assert - Edit
            Assert.IsNotNull(editResult);
            var editedForum = _testContext.Forum.Local.FirstOrDefault(f => f.Id == createdForumId);
            Assert.IsNotNull(editedForum);
            Assert.AreEqual("Updated CRUD Test Forum", editedForum.Title);
            Assert.AreEqual("Updated description", editedForum.Description);

            // DELETE
            // Act - Delete
            var deleteResult = await controller.DeleteConfirmed(createdForumId).ConfigureAwait(false) as RedirectToRouteResult;

            // Assert - Delete
            Assert.IsNotNull(deleteResult);
            var deletedForum = _testContext.Forum.Local.FirstOrDefault(f => f.Id == createdForumId);
            Assert.IsNull(deletedForum);
        }

        [TestMethod]
        public async Task Index_WithMultipleForums_ReturnsCorrectCount()
        {
            // Arrange
            var controller = new TestableForaController(_testContext);

            // Act
            var result = await controller.Index().ConfigureAwait(false) as ViewResult;
            var model = result.Model as ForaIndexViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.HasCount(_testForumData.Count, model.ForaIndexItems);
        }

        [TestMethod]
        public async Task Details_ReturnsCorrectForumData()
        {
            // Arrange
            var controller = new TestableForaController(_testContext);
            var expectedForum = _testForumData.First();

            // Act
            var result = await controller.Details(expectedForum.Id).ConfigureAwait(false) as ViewResult;
            var model = result.Model as Forum;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(expectedForum.Id, model.Id);
            Assert.AreEqual(expectedForum.Title, model.Title);
            Assert.AreEqual(expectedForum.Description, model.Description);
        }
    }
}