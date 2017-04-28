using communication.Core;
using communication.Models.GroupStatistics;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class GroupStatisticsController : Controller
    {
        // GET: GroupStatistics
        public ActionResult Index(string groupName, string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you are not logged in. please log in and then try again" });
            }
            if (Request.Cookies["testID"] != null)
            {
                var c = new HttpCookie("testID");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            if (message != null)
            {
                ViewBag.message = message;
            }

            

            string name = ServerWiring.getInstance().getUserName(Convert.ToInt32(cookie.Value));
            string group_name = groupName.Substring(0, groupName.LastIndexOf(GroupsMembers.CREATED_BY));
            GroupStatisticsData data = getData(Convert.ToInt32(cookie.Value), groupName);
            if(!data.message.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = data.message });
            }
            return View(data);
        }

        GroupStatisticsData getData(int id, string groupName)
        {
            GroupStatisticsData data = new GroupStatisticsData();
            //Tuple<string, List<Tuple<string,int,int>>> usersDoneTests = ServerWiring.getInstance().getPastGroupGrades(Convert.ToInt32(cookie.Value), groupName);
            List<Tuple<string, int, int>> list = new List<Tuple<string, int, int>>();
            list.Add(new Tuple<string, int, int>("test1", 15, 20));
            list.Add(new Tuple<string, int, int>("test2", 2, 5));
            list.Add(new Tuple<string, int, int>("test2", 11, 19));
            data.list = list;
            Tuple<string, List<Tuple<string, int, int>>> usersDoneTests = new Tuple<string, List<Tuple<string, int, int>>>(Replies.SUCCESS, list);
            data.message = usersDoneTests.Item1;
            if (!usersDoneTests.Item1.Equals(Replies.SUCCESS))
            {
                return data;
            }
            return data;
        }
       
    }
}