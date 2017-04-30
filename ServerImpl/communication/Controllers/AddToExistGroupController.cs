using communication.Core;
using communication.Models.Group;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AddToExistGroupController : Controller
    {
        // GET: AddToExistGroup
        [HttpGet]
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string inviteEmails, string emailContent)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            ViewBag.inviteEmails = inviteEmails;
            ViewBag.emailContent = emailContent;
            Tuple<string, string> groupName = ServerWiring.getInstance().getSavedGroup(Convert.ToInt32(cookie.Value));
            
            if (!groupName.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = groupName.Item1 });
            }
            string ans = ServerWiring.getInstance().inviteToGroup(Convert.ToInt32(cookie.Value), groupName.Item2, inviteEmails, emailContent);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "Invitation sent successfully" });
            }

            return RedirectToAction("Index", "ManageGroup", new { message = "Invalid email address entered. Please try again" });
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