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
    public class ManageGroupController : Controller
    {
        // GET: manageGroup
        [HttpGet]
        public ActionResult Index(string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            ViewBag.message = message;

            return View(getData(Convert.ToInt32(cookie.Value)));
        }

        [HttpPost]
        public ActionResult Submit(string groupName)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            string ans = ServerWiring.getInstance().removeGroup(Convert.ToInt32(cookie.Value), groupName);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main");
            }
            return RedirectToAction("Index", "ManageGroup", new { message = ans });
        }

       [HttpPost]
        public ActionResult saveChangeAndRedirectAddToExistGroup(string groupName)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            string ans = ServerWiring.getInstance().saveSelectedGroup(Convert.ToInt32(cookie.Value), groupName);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "AddToExistGroup");
            }
            return RedirectToAction("Index", "ManageGroup", new { message = ans });
        }

       [HttpPost]
       public ActionResult saveChangeAndRedirectAddTestToGroup(string groupName)
       {
           HttpCookie cookie = Request.Cookies["userId"];
           if (cookie == null)
           {
               return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
           }
           string ans = ServerWiring.getInstance().saveSelectedGroup(Convert.ToInt32(cookie.Value), groupName);
           if (ans.Equals(Replies.SUCCESS))
           {
               return RedirectToAction("Index", "AddTestToGroup");
           }
           return RedirectToAction("Index", "ManageGroup", new { message = ans });
       }
        
        List<GroupData> getData(int adminId)
        {
            List<GroupData> data = new List<GroupData>();
            Tuple<string, List<string>> groups = ServerWiring.getInstance().getAllAdminsGroups(adminId);
            // verify success
            foreach (string group in groups.Item2)
            {
                data.Add(new GroupData(group));
            }
            return data;
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