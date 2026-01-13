using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Test.DataContexts;
using JCarrollOnlineV2.ViewModels.Fora;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Test.Controllers
{
    [TestClass]
    public class ForaControllerTest
    {
        private FakeJCarrollOnlineV2DbContext _fakeContext;
        private List<Forum> _testForumData;

        [TestInitialize]
        public void Setup()
        {
            // Initialize test data
            _testForumData = new List<Forum>
            {
                new Forum
                {
                    Id = 1,
                    Title = "General Discussion",
                    Description = "General topics",
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                new Forum
                {
                    Id = 2,
                    Title = "Technical Support",
                    Description = "Get help with technical issues",
                    CreatedAt = DateTime.Now.AddDays(-20),
                    UpdatedAt = DateTime.Now.AddHours(-5)
                },
                new Forum
                {
                    Id = 3,
                    Title = "Announcements",
                    Description = "Official announcements",
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                }
            };

            // Setup fake DbContext
            _fakeContext = new FakeJCarrollOnlineV2DbContext();

            FakeJCarrollOnlineV2Db<Forum> mockSet = new FakeJCarrollOnlineV2Db<Forum>();
            foreach (Forum forum in _testForumData)
            {
                mockSet.Add(forum);
            }

            // Use reflection to replace the Forum DbSet with our fake
            PropertyInfo forumProperty = typeof(JCarrollOnlineV2DbContext).GetProperty("Forum");
            forumProperty.SetValue(_fakeContext, mockSet);

            // Setup ForumThreadEntry DbSet to prevent database queries
            FakeJCarrollOnlineV2Db<ThreadEntry> mockThreadEntrySet = new FakeJCarrollOnlineV2Db<ThreadEntry>();
            PropertyInfo threadEntryProperty = typeof(JCarrollOnlineV2DbContext).GetProperty("ForumThreadEntry");
            threadEntryProperty.SetValue(_fakeContext, mockThreadEntrySet);
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithForaIndexViewModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ViewResult result = await controller.Index().ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForaIndexViewModel));
        }

        [TestMethod]
        public async Task Index_ReturnsAllForums_InViewModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ViewResult result = await controller.Index().ConfigureAwait(false) as ViewResult;
            ForaIndexViewModel model = result.Model as ForaIndexViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.HasCount(_testForumData.Count, model.ForaIndexItems);
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewResult_WithForum()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int validId = 1;

            // Act
            ViewResult result = await controller.Details(validId).ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Forum));
            Forum model = result.Model as Forum;
            Assert.AreEqual(validId, model.Id);
        }

        [TestMethod]
        public async Task Details_WithNullId_ReturnsBadRequest()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ActionResult result = await controller.Details(null).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            HttpStatusCodeResult statusResult = result as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusResult.StatusCode);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ReturnsHttpNotFound()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int invalidId = 999;

            // Act
            ActionResult result = await controller.Details(invalidId).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public void Create_Get_ReturnsViewResult_WithForaCreateViewModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForaCreateViewModel));
        }

        [TestMethod]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            ForaCreateViewModel viewModel = new ForaCreateViewModel
            {
                Title = "New Forum",
                Description = "New forum description"
            };

            // Act
            RedirectToRouteResult result = await controller.Create(viewModel).ConfigureAwait(false) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(1, _fakeContext.SaveChangesAsyncCallCount);
        }

        [TestMethod]
        public async Task Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            controller.ModelState.AddModelError("Title", "Title is required");
            ForaCreateViewModel viewModel = new ForaCreateViewModel
            {
                Description = "Description without title"
            };

            // Act
            ViewResult result = await controller.Create(viewModel).ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForaCreateViewModel));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Create_Post_SetsCreatedAtAndUpdatedAt()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            ForaCreateViewModel viewModel = new ForaCreateViewModel
            {
                Title = "New Forum",
                Description = "New forum description"
            };
            DateTime beforeCreate = DateTime.Now;

            // Act
            await controller.Create(viewModel).ConfigureAwait(false);

            // Assert
            Forum addedForum = _fakeContext.Forum.Local.Last();
            Assert.IsTrue(addedForum.CreatedAt >= beforeCreate);
            Assert.IsTrue(addedForum.UpdatedAt >= beforeCreate);
            Assert.AreEqual(addedForum.CreatedAt, addedForum.UpdatedAt);
        }

        #endregion

        #region Edit Tests

        [TestMethod]
        public async Task Edit_Get_WithValidId_ReturnsViewResult_WithForumEditViewModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int validId = 1;

            // Act - CAST validId to int? to resolve ambiguity
            ViewResult result = await controller.Edit((int?)validId).ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForumEditViewModel));
            ForumEditViewModel model = result.Model as ForumEditViewModel;
            Assert.AreEqual(validId, model.Id);
        }

        [TestMethod]
        public async Task Edit_Get_WithInvalidId_ReturnsHttpNotFound()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int invalidId = 999;

            // Act - CAST invalidId to int? to resolve ambiguity
            ActionResult result = await controller.Edit((int?)invalidId).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public async Task Edit_Get_WithNullId_ReturnsBadRequest()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ActionResult result = await controller.Edit((int?)null).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            HttpStatusCodeResult statusResult = result as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusResult.StatusCode);
        }

        [TestMethod]
        public async Task Edit_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            Forum forum = _testForumData.First();
            forum.Title = "Updated Title";

            // Act
            RedirectToRouteResult result = await controller.Edit(forum).ConfigureAwait(false) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(1, _fakeContext.SaveChangesAsyncCallCount);
        }

        [TestMethod]
        public async Task Edit_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            controller.ModelState.AddModelError("Title", "Title is required");
            Forum forum = _testForumData.First();

            // Act
            ViewResult result = await controller.Edit(forum).ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Forum));
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_Get_WithValidId_ReturnsViewResult_WithForumDeleteViewModel()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int validId = 1;

            // Act
            ViewResult result = await controller.Delete(validId).ConfigureAwait(false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ForumDeleteViewModel));
            ForumDeleteViewModel model = result.Model as ForumDeleteViewModel;
            Assert.AreEqual(validId, model.Id);
        }

        [TestMethod]
        public async Task Delete_Get_WithNullId_ReturnsBadRequest()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act
            ActionResult result = await controller.Delete(null).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            HttpStatusCodeResult statusResult = result as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusResult.StatusCode);
        }

        [TestMethod]
        public async Task Delete_Get_WithInvalidId_ReturnsHttpNotFound()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int invalidId = 999;

            // Act
            ActionResult result = await controller.Delete(invalidId).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_WithValidId_RemovesForumAndRedirectsToIndex()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int validId = 1;
            int initialCount = _fakeContext.Forum.Local.Count;

            // Act
            RedirectToRouteResult result = await controller.DeleteConfirmed(validId).ConfigureAwait(false) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.HasCount(initialCount - 1, _fakeContext.Forum.Local);
            Assert.AreEqual(1, _fakeContext.SaveChangesAsyncCallCount);
        }

        [TestMethod]
        public async Task DeleteConfirmed_RemovesCorrectForum()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();
            int idToDelete = 2;
            Forum forumToDelete = _testForumData.First(f => f.Id == idToDelete);

            // Act
            await controller.DeleteConfirmed(idToDelete).ConfigureAwait(false);

            // Assert
            Assert.DoesNotContain(forumToDelete, _fakeContext.Forum.Local);
            Assert.IsTrue(_fakeContext.Forum.Local.All(f => f.Id != idToDelete));
        }

        #endregion

        #region Dispose Tests

        [TestMethod]
        public void Dispose_CallsBaseDispose()
        {
            // Arrange
            ForaController controller = CreateControllerWithMockContext();

            // Act & Assert - Should not throw
            controller.Dispose();
        }

        #endregion

        #region Helper Methods

        private ForaController CreateControllerWithMockContext()
        {
            ForaController controller = new ForaController(_fakeContext);

            return controller;
        }

        #endregion
    }
}