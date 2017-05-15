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
        public ActionResult Submit(int[] QuestionData, string reason)
        {

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            
            if (QuestionData == null)
            {
                return RedirectToAction("Index", "Administration", new { message = "Select one question to remove" });
            }
            int id = QuestionData[0];
            string reason1 = ViewBag.reason;
            if (reason1 == null)
                reason1 = "";
            List<Tuple<int, string>> questionsIdsAndResonsList = new List<Tuple<int,string>>();

            Tuple<int, string> temp = new Tuple<int, string>(id, reason1);
            questionsIdsAndResonsList.Add(temp);
            
            ViewData["QuestionData"] = QuestionData;


            string ans = ServerWiring.getInstance().removeQuestions(Convert.ToInt32(cookie.Value), questionsIdsAndResonsList);
            if (ans == Replies.SUCCESS)
            {
                return RedirectToAction("Index", "Administration", new { message = "Question removed successfully" });
            }


            return RedirectToAction("Index", "RemoveQuestion", new { message = "ans" });
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