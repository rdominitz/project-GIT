using communication.Core;
using Constants;
using Newtonsoft.Json;
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
        public ActionResult Index(string message)
        {
            if (message != null)
            {
               ViewData["message"] = message;
            }
            Response.Cookies.Remove("testID");
            Response.Cookies.Remove("groupName");

            return View();
        }

        [HttpPost]
        public ActionResult Submit(string email, string password)
        {
            ViewBag.email = email;

            Tuple<string, int> ans = ServerWiring.getInstance().login(email, password);
            if (ans.Item1.Equals(Replies.SUCCESS))
            {
                //var json = JsonConvert.SerializeObject(ans.Item2);
                HttpCookie userCookie = new HttpCookie("userId", ans.Item2.ToString());
                Response.SetCookie(userCookie);
                //return View("main/index");
                return RedirectToAction("Index", "Main");
            }
            ViewBag.errorMessage = ans.Item1;
            return View("index");
        }

        [HttpPost]
        public ActionResult Register()
        {

            return View("register/index");
        }

        [HttpPost]
        public ActionResult Forgot()
        {

            return View("forgot/index");
        }


    }
}