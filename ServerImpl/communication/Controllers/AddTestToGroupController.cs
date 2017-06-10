using communication.Core;
using communication.Models.GetTest;
using communication.Models.Test;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AddTestToGroupController : Controller
    {
        // GET: AddTestToGroup
        [HttpGet]
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            List<GetTestData> tests = getData(Convert.ToInt32(cookie.Value));
            if (tests.Count!= 0)
                return View(tests);
            return View();
        }

        private List<GetTestData> getData(int adminId)
        {
            List<GetTestData> data = new List<GetTestData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                Tuple<string, List<Test>> tests = ServerWiring.getInstance().getAllTests(adminId, subject);
                List<string> testStrings = new List<string>();
                foreach (Test test in tests.Item2)
                {
                    testStrings.Add(test.ToString());
                }
                data.Add(new GetTestData(subject, testStrings));
            }
            return data;
        }

        [HttpPost]
        public ActionResult Submit(string testDetails)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            if (testDetails == null)
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "You must choose at least one test" });
            }
            ViewBag.testDetails = testDetails;
            String[] details = testDetails.Split(',');
            String[] testIdArr = details[0].Split(':');
            String[] testIdArr1 = testIdArr[1].Split(' ');
            int testId = int.Parse(testIdArr1[1]);
            HttpCookie groupCookie = Request.Cookies["groupName"];

            string ans = ServerWiring.getInstance().saveGroupAndTest(Convert.ToInt32(cookie.Value), groupCookie.Value, testId);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "SeeTestDetails", new { testId = testId });
            }
            return RedirectToAction("Index", "AddTestToGroup", ViewBag.group);
        }

        private void removeCookie(string s)
        {
            if (Request.Cookies[s] != null)
            {
                var c = new HttpCookie(s);
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
        }
    }
}