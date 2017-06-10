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
        public ActionResult Index()
        {
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
	}
}