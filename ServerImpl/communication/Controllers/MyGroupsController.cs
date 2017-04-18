using communication.Core;
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
            List<String> groups = ServerWiring.getInstance().getUsersGroups(Convert.ToInt32(cookie.Value));
            List<String> groupsInvitations = ServerWiring.getInstance().getUsersGroupsInvitations(Convert.ToInt32(cookie.Value));
            ViewBag.name = name;
            ViewBag.isAdmin = isAdmin;
            ViewData["groups"] = groups;
            ViewData["groupsInvitations"] = groupsInvitations;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string [] Groups)
        {
            HttpCookie userCookie = Request.Cookies["userId"];
            if (userCookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            List<string> list = new List<string>(Groups);
           ServerWiring.getInstance().acceptUsersGroupsInvitations(Convert.ToInt32(userCookie.Value), list); 
           return RedirectToAction("index");
          
        }
    }
}