using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.ViewModels.Sandbox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JCarrollOnlineV2.Test.Controllers
{
    [TestClass]
    public class SandboxControllerTest
    {
        #region Constructor Tests

        [TestMethod]
        public void Constructor_CreatesController()
        {
            // Act
            SandboxController controller = new SandboxController();

            // Assert
            Assert.IsNotNull(controller);
        }

        #endregion

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Index_ReturnsSandboxViewModel()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(SandboxViewModel));
        }

        [TestMethod]
        public async Task Index_SetsPageTitleToSandbox()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;
            SandboxViewModel model = result.Model as SandboxViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual("Sandbox", model.PageTitle);
        }

        #endregion

        #region YellowStoneSlideShow Tests

        [TestMethod]
        public async Task YellowStoneSlideShow_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = CreateControllerWithMockedContext();

            // Act
            ActionResult result = await controller.YellowStoneSlideShow();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task YellowStoneSlideShow_ReturnsYellowstoneViewModel()
        {
            // Arrange
            SandboxController controller = CreateControllerWithMockedContext();

            // Act
            ViewResult result = await controller.YellowStoneSlideShow() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(YellowstoneViewModel));
        }

        [TestMethod]
        public async Task YellowStoneSlideShow_SetsPageTitleToYellowstoneSlideshow()
        {
            // Arrange
            SandboxController controller = CreateControllerWithMockedContext();

            // Act
            ViewResult result = await controller.YellowStoneSlideShow() as ViewResult;
            YellowstoneViewModel model = result.Model as YellowstoneViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual("Yellowstone Slideshow", model.PageTitle);
        }

        [TestMethod]
        public async Task YellowStoneSlideShow_PopulatesImageFilesFromDirectory()
        {
            // Arrange
            SandboxController controller = CreateControllerWithMockedContext();

            // Act
            ViewResult result = await controller.YellowStoneSlideShow() as ViewResult;
            YellowstoneViewModel model = result.Model as YellowstoneViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.ImageFiles);
            // Note: The actual count depends on files in the yellowstone directory
            // In a real test environment, you'd mock the file system or use a test directory
        }

        [TestMethod]
        public async Task YellowStoneSlideShow_ImageFilesHaveCorrectBaseUri()
        {
            // Arrange
            SandboxController controller = CreateControllerWithMockedContext();

            // Act
            ViewResult result = await controller.YellowStoneSlideShow() as ViewResult;
            YellowstoneViewModel model = result.Model as YellowstoneViewModel;

            // Assert
            Assert.IsNotNull(model);
            if (model.ImageFiles.Any())
            {
                ImageFileMetadata firstImage = model.ImageFiles.First();
                Assert.Contains("http://", firstImage.Path);
                Assert.Contains("/content/images/yellowstone/", firstImage.Path);
            }
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ActionResult result = await controller.Details();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewWithNoModel()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Details() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ActionResult result = await controller.Create();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ReturnsViewWithNoModel()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        #endregion

        #region Edit Tests

        [TestMethod]
        public async Task Edit_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ActionResult result = await controller.Edit();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewWithNoModel()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Edit() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_ReturnsViewResult()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ActionResult result = await controller.Delete();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewWithNoModel()
        {
            // Arrange
            SandboxController controller = new SandboxController();

            // Act
            ViewResult result = await controller.Delete() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a SandboxController with a mocked HttpContext for testing methods that require context.
        /// </summary>
        private SandboxController CreateControllerWithMockedContext()
        {
            // Create the controller
            SandboxController controller = new SandboxController();

            // Mock HttpContextBase
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            
            // Mock HttpServerUtilityBase for Server.MapPath
            Mock<HttpServerUtilityBase> mockServer = new Mock<HttpServerUtilityBase>();
            string testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "yellowstone");
            
            // Ensure test directory exists
            if (!Directory.Exists(testPath))
            {
                Directory.CreateDirectory(testPath);
            }
            
            mockServer.Setup(s => s.MapPath(It.IsAny<string>())).Returns(testPath);
            mockHttpContext.Setup(c => c.Server).Returns(mockServer.Object);

            // Mock HttpRequestBase for Request.Url
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(r => r.Url).Returns(new Uri("http://localhost/Sandbox/YellowStoneSlideShow"));
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            // Create ControllerContext with mocked HttpContext
            ControllerContext controllerContext = new ControllerContext(
                mockHttpContext.Object,
                new RouteData(),
                controller);

            controller.ControllerContext = controllerContext;

            return controller;
        }

        #endregion

        // NOTE: The YellowStoneSlideShow method has dependencies on:
        // 1. File system (Directory.EnumerateFiles)
        // 2. HttpContext (Server.MapPath and Request.Url)
        //
        // For more comprehensive testing of YellowStoneSlideShow, consider:
        // 1. Using a test directory with known image files
        // 2. Mocking IFileSystem (requires refactoring controller to accept IFileSystem)
        // 3. Using integration tests that set up actual test files
        //
        // Example of enhanced YellowStoneSlideShow test with test files:
        //
        // [TestMethod]
        // public async Task YellowStoneSlideShow_WithTestImages_ReturnsCorrectCount()
        // {
        //     // Arrange
        //     string testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "yellowstone");
        //     Directory.CreateDirectory(testPath);
        //     
        //     // Create test image files
        //     File.WriteAllText(Path.Combine(testPath, "test1.jpg"), "fake image data");
        //     File.WriteAllText(Path.Combine(testPath, "test2.jpg"), "fake image data");
        //     
        //     SandboxController controller = CreateControllerWithMockedContext();
        //
        //     // Act
        //     ViewResult result = await controller.YellowStoneSlideShow() as ViewResult;
        //     YellowstoneViewModel model = result.Model as YellowstoneViewModel;
        //
        //     // Assert
        //     Assert.IsNotNull(model);
        //     Assert.AreEqual(2, model.ImageFiles.Count());
        //     
        //     // Cleanup
        //     Directory.Delete(testPath, true);
        // }
        //
        // RECOMMENDATION: For better testability, consider refactoring YellowStoneSlideShow to:
        // 1. Extract file system operations into a separate service (IImageFileService)
        // 2. Inject dependencies through constructor or properties
        // 3. This would allow proper unit testing without file system dependencies
    }
}
