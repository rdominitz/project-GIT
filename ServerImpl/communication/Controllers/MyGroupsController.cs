using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class MyGroupsController : Controller
    {
        // GET: MyGroups
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you are not logged in. please log in and then try again" });
            }
            string name = ServerWiring.getInstance().getUserName(Convert.ToInt32(cookie.Value));
            Boolean isAdmin = ServerWiring.getInstance().isAdmin(Convert.ToInt32(cookie.Value));
            Tuple<string, List<String>> groupsRes = ServerWiring.getInstance().getUsersGroups(Convert.ToInt32(cookie.Value));
            if (!groupsRes.Item1.Equals(Replies.SUCCESS))
            {
                // error
            }
            List<String> groups = groupsRes.Item2;
            Tuple<string, List<String>> groupsInvitationsRes = ServerWiring.getInstance().getUsersGroupsInvitations(Convert.ToInt32(cookie.Value));
            if (!groupsInvitationsRes.Item1.Equals(Replies.SUCCESS))
            {
                // error
            }
            List<String> groupsInvitations = groupsInvitationsRes.Item2;
            ViewBag.name = name;
            ViewBag.isAdmin = isAdmin;
            ViewData["groups"] = groups;
            ViewData["groupsInvitations"] = groupsInvitations;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string[] Groups)
        {
            HttpCookie userCookie = Request.Cookies["userId"];
            if (userCookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            List<string> list = new List<string>(Groups);
            string s = ServerWiring.getInstance().acceptUsersGroupsInvitations(Convert.ToInt32(userCookie.Value), list);
            if (!s.Equals(Replies.SUCCESS))
            {
                // error
            }
            return RedirectToAction("index");

        }
    }
}