using communication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants;

namespace communication.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult Index(string groupName, string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you are not logged in. please log in and then try again" });
            }
            if (message != null)
            {
                ViewBag.message = message;
            }

            Response.Cookies.Remove("testID");
            Response.Cookies.Remove("groupName");

            string name = ServerWiring.getInstance().getUserName(Convert.ToInt32(cookie.Value));
            string group_name = groupName.Substring(0, groupName.LastIndexOf(GroupsMembers.CREATED_BY));
            ViewBag.group_name = group_name;
            HttpCookie groupCookie = new HttpCookie("groupName", groupName);
            Response.SetCookie(groupCookie);
            Tuple<string, List<Tuple<string, int>>> unFinishedTests = ServerWiring.getInstance().getUnfinishedTests(Convert.ToInt32(cookie.Value), groupName);
            //Tuple<string, List<Tuple<string, int>>> finishedTests = ServerWiring.getInstance().getFinishedTests(Convert.ToInt32(cookie.Value), groupName);
            if (!unFinishedTests.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = unFinishedTests.Item1 });
            }
            ViewBag.unFinishedTests = unFinishedTests.Item2;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string testID)
        {

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            HttpCookie groupCookie = Request.Cookies["groupName"];
            if (groupCookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "a cookie has been deleted, please try again" });
            }
            HttpCookie testCookie = new HttpCookie("testID", testID);
            Response.SetCookie(testCookie);
            /*string ans = ServerWiring.getInstance().getTest(Convert.ToInt32(cookie.Value), Convert.ToInt32(testID), groupCookie.Value);
            if (ans == Replies.SUCCESS)
            {
                return RedirectToAction("Index", "AnswerQuestion");
            }*/

            return RedirectToAction("Index", "AnswerQuestion");
            //return RedirectToAction("Index", "Group", new { groupName = groupCookie.Value, message = ans });
        }
    }
}