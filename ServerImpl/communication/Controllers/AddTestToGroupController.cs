using communication.Core;
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
            removeCookie("groupName");

            return View(getData(Convert.ToInt32(cookie.Value)));
        }

        List<TestData> getData(int adminId)
        {
            List<TestData> data = new List<TestData>();
            Tuple<string, List<Test>> tests = ServerWiring.getInstance().getAllTests(adminId);
            // verify success
            foreach (Test test in tests.Item2)
            {
                data.Add(new TestData(test.TestId, test.ToString()));
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

            ViewBag.testDetails = testDetails;
            String[] details = testDetails.Split(',');
            String[] testIdArr = details[0].Split(':');
            String[] testIdArr1 = testIdArr[1].Split(' ');
            int testId = int.Parse(testIdArr1[1]);

            Tuple<string, string> groupName = ServerWiring.getInstance().getSavedGroup(Convert.ToInt32(cookie.Value));
            if (!groupName.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "There was a problem, please reconnect" });
            }

            string ans = ServerWiring.getInstance().saveGroupAndTest(Convert.ToInt32(cookie.Value), groupName.Item2, testId);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "SeeTestDetails", new { testId = testId });
            }

            return RedirectToAction("Index", "AddTestToGroup", ViewBag.group);


           // string ans = ServerWiring.getInstance().addTestToGroup(Convert.ToInt32(cookie.Value), groupName.Item2, testId);
            //if (ans.Equals(Replies.SUCCESS))
            //{
             //   return RedirectToAction("Index", "ManageGroup", new { message = "The test was successfully added to the group" });
           // }
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