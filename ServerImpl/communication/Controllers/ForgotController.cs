using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class ForgotController : Controller
    {
        // GET: Forgot
        [HttpGet]
        public ActionResult Index(string message)
        {
            ViewBag.message = message;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string email)
        {
            string ans = ServerWiring.getInstance().restorePassword(email);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Login");
            }
            //ViewBag.errorMessage = ans;
            return RedirectToAction("Index", "Forgot", new { message = ans });
        }

    }
}