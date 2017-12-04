using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = await controller.Index(null) as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.AreEqual("JCarrollOnlineV2 Home - Index", vm.Message);
        }

        //[TestMethod]
        //public async Task IndexWithOutUser()
        //{
        //    HomeController controller = new HomeController();

        //    var result = await controller.Welcome() as ViewResult;
        //    HomeViewModel bfiVM = (HomeViewModel)result.Model;

        //    Assert.AreEqual("JCarrollOnlineV2 Home - Index", bfiVM.PageTitle);

        //}

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;
            AboutViewModel vm = (AboutViewModel)result.Model;

            // Assert
            Assert.AreEqual("About JCarrollOnlineV2", vm.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;
            ContactViewModel vm = (ContactViewModel)result.Model;
            // Assert
            Assert.AreEqual("JCarrollOnlineV2 Contact", vm.Message);
        }
    }
}
