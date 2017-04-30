using communication.Core;
using communication.Models.GetTest;
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
    public class RemoveQuestionController : Controller
    {
        //
        // GET: /RemoveQuestion/
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
            List<QuestionData> questions= getData(Convert.ToInt32(cookie.Value));
            if (questions != null)
                return View(questions);
            else
                return View();
        }

        List<QuestionData> getData(int adminId)
        {
            List<QuestionData> data = new List<QuestionData>();
            Tuple<string, List<Question>> questions = ServerWiring.getInstance().getAllReleventQuestions(adminId);
            if (questions.Item1 == Replies.SUCCESS)
            {
                foreach (Question q in questions.Item2)
                {
                    data.Add(new QuestionData(q));
                }
                return data;
            }
            else
            {
                return data;
            }
        }

        [HttpPost]
        public ActionResult Submit(int[] QuestionData)
        {

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            List<int> questionsIdsList = new List<int>();
            if (QuestionData == null)
            {
                return RedirectToAction("Index", "Administration", new { message = "Select at least one question to remove" });
            }
            questionsIdsList = QuestionData.ToList();
            ViewData["QuestionData"] = QuestionData;


            string ans = ServerWiring.getInstance().removeQuestions(Convert.ToInt32(cookie.Value), questionsIdsList);
            if (ans == Replies.SUCCESS)
            {
                return RedirectToAction("Index", "Main", new { message = "Questions removed successfully" });
            }


            return RedirectToAction("Index", "RemoveQuestion", new { message = ans });
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