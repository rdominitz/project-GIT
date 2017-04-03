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
        public ActionResult Submit(int testId)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            ViewBag.testId = testId;
            Tuple <string,string> groupName = ServerWiring.getInstance().getSavedGroup(Convert.ToInt32(cookie.Value));
            if (!groupName.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "There was a problem, please reconnect" });
            }
            string ans = ServerWiring.getInstance().addTestToGroup(Convert.ToInt32(cookie.Value), groupName.Item2, testId);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "The test was successfully added to the group" });
            }
            return RedirectToAction("Index", "AddTestToGroup", ViewBag.group);
        }
    }
}