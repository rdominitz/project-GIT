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
        public ActionResult Index(string group)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.group = group;

            return View(getData(Convert.ToInt32(cookie.Value)));
        }

        List<TestData> getData(int adminId)
        {
            List<TestData> data = new List<TestData>();
            Tuple<string, List<Test>> tests = ServerWiring.getInstance().getAllTests(adminId);
            // verify success
            foreach (Test test in tests.Item2)
            {
                data.Add(new TestData(test.ToString()));
            }
            return data;
        }

        [HttpPost]
        public ActionResult Submit(string groupName, int testId)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            ViewBag.groupName = groupName;
            ViewBag.testId = testId;

            string ans = ServerWiring.getInstance().addTestToGroup(Convert.ToInt32(cookie.Value), groupName, testId);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "The test was successfully added to the group" });
            }
            return RedirectToAction("Index", "AddTestToGroup", ViewBag.group);
        }
    }
}