using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AddSubjectController : Controller
    {
        //
        // GET: /AddSubject/
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
        public ActionResult Submit(string subjectName)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.subjectName = subjectName;
            string ans = ServerWiring.getInstance().addSubject(Convert.ToInt32(cookie.Value), subjectName);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = "subject added successfully" });
            }
            return RedirectToAction("Index", "AddSubject", new { message = ans });
        }

	}
}