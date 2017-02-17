using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string email, string password)
        {
            ViewBag.email = email;

            Tuple<string, int> ans = ServerWiring.getInstance().login(email, password);
            if (ans.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.errorMessage = ans.Item1;
            return View("index");
        }
    }
}