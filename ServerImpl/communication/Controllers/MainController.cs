using communication.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class MainController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            string name = ServerWiring.getInstance().getUserName(Convert.ToInt32(cookie.Value));
            ViewBag.name = name;
            return View();
        }

       
    }
}