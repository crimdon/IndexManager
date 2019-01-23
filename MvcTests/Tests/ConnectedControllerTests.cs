using Microsoft.VisualStudio.TestTools.UnitTesting;
using Index_Manager_Web.Controllers;
using System.Web.Mvc;
using MvcTests.Helpers;
using Moq;
using System.Web;

namespace MvcTests.Tests
{
    [TestClass]
    public class ConnectedControllerTests
    {
        [TestMethod]
        public void GetDatabaseNames()
        {
            //Register controller and mock the session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();

            context.Setup(m => m.HttpContext.Session["servername"]).Returns("localhost");
            context.Setup(m => m.HttpContext.Session["connectionstring"]).Returns(DataHelpers.getConnectionString("localhost", "Windows", null, null));

            ConnectedController controller = new ConnectedController();

            controller.ControllerContext = context.Object;

            //Act
            var result = controller.Databases() as ViewResult;

            //Tests
            Assert.IsNotNull(result.ViewData["Databases"]);
        }
    }
}
