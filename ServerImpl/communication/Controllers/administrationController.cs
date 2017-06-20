using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class administrationController : Controller
    {
        //
        // GET: /administration/
        [HttpGet]
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
            removeCookie("TestId");
            removeCookie("groupName");
            return View();
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