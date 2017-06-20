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
        public ActionResult Index(int TestId)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.TestId = TestId;
            List<QuestionData> questions = getData(Convert.ToInt32(cookie.Value), TestId);
            if (questions != null)
                return View(questions);
            return View();
        }

        List<QuestionData> getData(int adminId, int TestId)
        {
            List<QuestionData> data = new List<QuestionData>();
            Tuple<string, List<Question>> t = ServerWiring.getInstance().getTestQuestionsByTestId(adminId, TestId);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return data;
            }
            List<Question> testQuestions = t.Item2;

            if (testQuestions != null)
            {
                foreach (Question q in testQuestions)
                {
                    data.Add(new QuestionData(q));
                }
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
                return RedirectToAction("Index", "Administration", new { message = "The test was successfully added to the group" });
            }
            return RedirectToAction("Index", "AddTestToGroup", groupAndTest.Item2.Item1);
        }

        [HttpPost]
        public ActionResult Cancel()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            return RedirectToAction("Index", "ManageGroup");
        }
	}
}