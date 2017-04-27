using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using communication.Controllers;
using System.Web.Mvc;
using Entities;
using communication.Core;
using System.Web;
using Moq;
using System.Web.Routing;
using System.Net;
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using System.Net.Http;

namespace communicationTests
{
    [TestClass]
    public class LoginTests
    {



        [TestInitialize]
        public void TestInitialize()
        {
            ServerWiring.initServer(new FakeMedTrainDBContext());

            // We need to setup the Current HTTP Context as follows:            

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest("", "http://localhost/", "");

            // Step 2: Setup the HTTP Response
            var httpResponce = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer =
                new HttpSessionStateContainer("id",
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                .GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });

            // Step 4: Assign the Context
            HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void SuccessfulLogin_Test()
        {
            var controller = new LoginController();


            /* var request = new Mock<HttpRequestBase>();
             request.SetupGet(x => x.Request).Returns(request.Object);
             controller.ControllerContext = new ControllerContext(request.Object, new RouteData(), controller);*/

            var request = new Mock<HttpRequestBase>() { CallBase = true };
            // Not working - IsAjaxRequest() is static extension method and cannot be mocked
            // request.Setup(x => x.IsAjaxRequest()).Returns(true /* or false */);
            // use this
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection {
        {"X-Requested-With", "XMLHttpRequest"}
                });

            var response = new Mock<HttpResponseBase>() { CallBase = true };
            response.CallBase = true;
            // Not working - IsAjaxRequest() is static extension method and cannot be mocked
            // request.Setup(x => x.IsAjaxRequest()).Returns(true /* or false */);
            // use this
            response.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection {
        {"X-Response-With", "XMLHttpResponse"}
                });
            response.SetupGet(x => x.Cookies).Returns(new HttpCookieCollection());
            //response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).;


            var context = new Mock<HttpContextBase>() { CallBase = true };
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            context.SetupGet(x => x.Response.Cookies).Returns(new HttpCookieCollection());


            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var result = controller.Submit("user@gmail.com", "password") as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void SuccessfulLogin_Test2()
        {
            var mockController = new Mock<LoginController>();
            var controller = new LoginController();
            controller.ActionInvoker.InvokeAction(controller.ControllerContext, "Submit");
            mockController.Object.Submit("", "");
            //mockController.Verify(mockController.Object.ActionInvoker.InvokeAction(LoginController,"Submit")
            //var controller = new LoginController();
            //controller.Request.Cookies.Add
            var result = controller.Submit("user@gmail.com", "password") as ViewResult;
            //Assert.()
        }
    }
}
