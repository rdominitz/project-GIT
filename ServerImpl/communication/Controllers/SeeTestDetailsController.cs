using communication.Core;
using communication.Models.Questions;
using communication.Models.Test;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class SeeTestDetailsController : Controller
    {
        //
        // GET: /SeeTestDetails/
        public ActionResult Index(int testId)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.testId = testId;
            return View(getData(Convert.ToInt32(cookie.Value), testId));
        }

        List<QuestionData> getData(int adminId, int testId)
        {

            Tuple<string, List<Question>> t = ServerWiring.getInstance().getTestQuestionsByTestId(adminId, testId);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                // error
            }
            List<Question> testQuestions = t.Item2;
            List<QuestionData> data = new List<QuestionData>();
            // verify success
            foreach(Question q in testQuestions)
            {
                data.Add(new QuestionData(q));
            }
            return data;
        }



        [HttpPost]
        public ActionResult Submit()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            Tuple<string, Tuple<string, int>> groupAndTest = ServerWiring.getInstance().getSavedGroupAndTest(Convert.ToInt32(cookie.Value));
            if (!groupAndTest.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = groupAndTest.Item1 });
            }
            
            string ans = ServerWiring.getInstance().addTestToGroup(Convert.ToInt32(cookie.Value), groupAndTest.Item2.Item1, groupAndTest.Item2.Item2);
            
            if (ans.Equals(Replies.SUCCESS))
            {
               return RedirectToAction("Index", "ManageGroup", new { message = "The test was successfully added to the group" });
            }
            return RedirectToAction("Index", "AddTestToGroup", groupAndTest.Item2.Item1);
        }
	}
}