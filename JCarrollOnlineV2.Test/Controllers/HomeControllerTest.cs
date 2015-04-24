using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCarrollOnlineV2;
using JCarrollOnlineV2.Controllers;
using System.Threading.Tasks;
using JCarrollOnlineV2.ViewModels;

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
            var result = await controller.Index() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.AreEqual("JCarrollOnlineV2 Home - Index", vm.Message);
        }

        [TestMethod]
        public async Task IndexWithOutUser()
        {
            HomeController controller = new HomeController();

            var result = await controller.Welcome() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            Assert.AreEqual("JCarrollOnlineV2 Home - Index", vm.PageTitle);

        }

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
