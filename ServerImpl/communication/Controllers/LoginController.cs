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
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie != null)
            {
                ServerWiring.getInstance().logout(Convert.ToInt32(cookie.Value));
            }
            Response.Cookies.Remove("userId");

            return View();
        }

        [HttpPost]
        public ActionResult Submit(string email, string password)
        {
            ViewBag.email = email;

            Tuple<string, int> ans = ServerWiring.getInstance().login(email, password);
            if (ans.Item1.Equals(Replies.SUCCESS))
            {
               
                HttpCookie userCookie = new HttpCookie("userId", ans.Item2.ToString());
                Response.SetCookie(userCookie);
                
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