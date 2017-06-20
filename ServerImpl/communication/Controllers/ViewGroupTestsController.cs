using communication.Core;
using communication.Models.GetTest;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class ViewGroupTestsController : Controller
    {
        //
        // GET: /ViewGroupTests/
        [HttpGet]
        public ActionResult Index(string message)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            List<GetTestData> tests = getData(Convert.ToInt32(cookie.Value));
            if (tests.Count != 0)
                return View(tests);
            return View();
        }

        private List<GetTestData> getData(int adminId)
        {
            List<GetTestData> data = new List<GetTestData>();
            HttpCookie groupCookie = Request.Cookies["groupName"];
            Tuple<string, List<Test>> tests = ServerWiring.getInstance().getGroupTests(adminId, groupCookie.Value);
            if (!tests.Item1.Equals(Replies.SUCCESS))
            {
                return data;
            }
            List<string> testStrings = new List<string>();
            foreach (Test test in tests.Item2)
            {
                testStrings.Add(test.ToString());
            }
            data.Add(new GetTestData(groupCookie.Value, testStrings));
            return data;
        }

        [HttpPost]
        public ActionResult Submit(string testDetails)
        {
            if (testDetails == null)
            {
                return RedirectToAction("Index", "ViewGroupTests", new { message = "Please select a test" });
            
            }
            ViewBag.testDetails = testDetails;
            String[] details = testDetails.Split(',');
            String[] TestIdArr = details[0].Split(':');
            String[] TestIdArr1 = TestIdArr[1].Split(' ');
           // int TestId = int.Parse(TestIdArr1[1]);
            HttpCookie testCookie = new HttpCookie("TestId", TestIdArr1[1]);
            Response.SetCookie(testCookie);

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            return RedirectToAction("Index", "TestStatistics");
        }

    }
}