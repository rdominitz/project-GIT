using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class SetUserAsAdminController : Controller
    {
        //
        // GET: /SetUserAsAdmin/
        public ActionResult Index(string message)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            return View();
        }


        [HttpPost]
        public ActionResult Submit(string userEmail)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.userEmail = userEmail;
            string ans = ServerWiring.getInstance().setUserAsAdmin(Convert.ToInt32(cookie.Value), userEmail);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = "User added as administrator successfully" });
            }
            return RedirectToAction("Index", "SetUserAsAdmin", new { message = ans });
        }
	}
}