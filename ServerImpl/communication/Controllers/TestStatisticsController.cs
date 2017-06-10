using communication.Core;
using communication.Models.TestStatistics;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class TestStatisticsController : Controller
    {
        //
        // GET: /TestStatistics/
        public ActionResult Index(int testId)
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
            HttpCookie groupCookie = Request.Cookies["groupName"];
            TestStatisticsData data = getData(Convert.ToInt32(cookie.Value), testId, groupCookie.Value);
            Dictionary<string, double> range = new Dictionary<string, double>();
            range["0-55"] = 0;
            range["56-70"] = 0;
            range["71-80"] = 0;
            range["81-90"] = 0;
            range["91-100"] = 0;
           
            foreach (Tuple<string, int> t in data.gradesInTest)
            {
                if (t.Item2 <= 55)
                {
                    range["0-55"]++;
                }
                else if (t.Item2 > 55 && t.Item2 <= 70)
                {
                    range["56-70"]++;
                }
                else if (t.Item2 > 70 && t.Item2 <= 80)
                {
                    range["71-80"]++;
                }
                else if (t.Item2 > 80 && t.Item2 <= 90)
                {
                    range["81-90"]++;
                }
                else if (t.Item2 > 90 && t.Item2 <= 100)
                {
                    range["91-100"]++;
                }
            }
            data.rangeCount = range;
            return View(data);
        }


        TestStatisticsData getData(int adminId, int testId, string groupName)
        {
            TestStatisticsData data = new TestStatisticsData();
            Tuple<string, List<Tuple<string, double>>> usersGrades = ServerWiring.getInstance().getGrades(adminId, testId, groupName);
            if (!usersGrades.Item1.Equals(Replies.SUCCESS))
            {
                return data;
            }

            data.gradesInTest = usersGrades.Item2;
            return data;
        }
	}
}