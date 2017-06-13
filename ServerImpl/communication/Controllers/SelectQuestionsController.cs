using communication.Core;
using communication.Models.GetQuestion;
using communication.Models.Questions;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class SelectQuestionsController : Controller
    {
        // GET: SelectQuestions
        [HttpGet]
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            List<Question> questions = ServerWiring.getInstance().getTestQuestions(Convert.ToInt32(cookie.Value));
            List<QuestionData> questionsData = new List<QuestionData>();
            if (questions != null)
            {
                foreach (Question ques in questions)
                {
                    questionsData.Add(new QuestionData(ques));
                }
            
            return View(questionsData);
            }
            else
                return View();
        }


        [HttpPost]
        public ActionResult Submit(string testName, int[] QuestionData)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            
            List<int> questionsIdsList = new List<int>();
            if (QuestionData == null)
            {
                return RedirectToAction("Index", "CreateTest", new { message = "Select at least one question" });
            }
            questionsIdsList = QuestionData.ToList();
            
            ViewBag.testName = testName;
            ViewData["QuestionData"] = QuestionData;


            string ans = ServerWiring.getInstance().createTest(Convert.ToInt32(cookie.Value), questionsIdsList, testName);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "ManageGroup", new { message = "The test was successfully created" });
            }
            ViewBag.message = ans;
            return View();
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