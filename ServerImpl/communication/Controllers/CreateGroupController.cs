using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class CreateGroupController : Controller
    {
        // GET: CreateGroup
        [HttpGet]
        public ActionResult Index(string message)
        {

            ViewBag.message = message;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string groupName, string inviteEmails, string emailContent)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            ViewBag.groupName =groupName;
            ViewBag.inviteEmails=inviteEmails;
            ViewBag.emailContent=emailContent;

            string ans = ServerWiring.getInstance().createGroup(Convert.ToInt32(cookie.Value), groupName, inviteEmails, emailContent);
            if (ans.Equals(Replies.SUCCESS))
            {
                //show messege  "Group created successfully"
                return RedirectToAction("Index", "Main");
            }
            return RedirectToAction("Index", "CreateGroup", new { message = ans });
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
