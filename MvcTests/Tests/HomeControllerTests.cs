using Microsoft.VisualStudio.TestTools.UnitTesting;
using Index_Manager_Web.Controllers;
using Index_Manager_Web.Models;
using System.Web.Mvc;
using MvcTests.Helpers;

namespace MvcTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod()]
        public void WindowsLogonTest()
        {
            //Setup the Model
            var model = new LoginModel()
            {
                Servername = "localhost",
                Authentication = "Windows"
            };

            //Register controller and mock the session
            HomeController controller = new HomeController();
            controller.SetFakeControllerContext();

            //Act
            RedirectToRouteResult routeResult = controller.Index(model) as RedirectToRouteResult;

            //Tests
            Assert.IsNotNull(routeResult);

            routeResult.RouteValues["action"].Equals("Databases");
            routeResult.RouteValues["controller"].Equals("Connected");

            Assert.AreEqual("Databases", routeResult.RouteValues["action"]);
            Assert.AreEqual("Connected", routeResult.RouteValues["controller"]);
        }

        [TestMethod()]
        public void SQLLogonTest()
        {
            //Setup the Model
            var model = new LoginModel()
            {
                Servername = "localhost",
                Authentication = "SQL",
                Username = "sa",
                Password = "sa"
            };

            //Register controller and mock the session
            HomeController controller = new HomeController();
            controller.SetFakeControllerContext();

            //Act
            RedirectToRouteResult routeResult = controller.Index(model) as RedirectToRouteResult;

            //Tests
            Assert.IsNotNull(routeResult);

            routeResult.RouteValues["action"].Equals("Databases");
            routeResult.RouteValues["controller"].Equals("Connected");

            Assert.AreEqual("Databases", routeResult.RouteValues["action"]);
            Assert.AreEqual("Connected", routeResult.RouteValues["controller"]);
        }

        [TestMethod()]
        public void LogonFail()
        {
            //Setup the Model
            var model = new LoginModel()
            {
                Servername = "localhost",
                Authentication = "SQL",
                Username = "sa",
                Password = "as"
            };

            //Register controller and mock the session
            HomeController controller = new HomeController();
            controller.SetFakeControllerContext();

            //Act
            ViewResult result = controller.Index(model) as ViewResult;

            //Tests
            Assert.IsTrue(result.ViewData.ModelState["LoginError"].Errors.Count == 1);
        }
    }
}
