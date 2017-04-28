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
            return View(getData(Convert.ToInt32(cookie.Value)));
        }

        List<QuestionData> getData(int adminId)
        {
            List<QuestionData> data = new List<QuestionData>();
            Tuple<string, List<Question>> questions = ServerWiring.getInstance().getAllReleventQuestions(adminId);
            // verify success
            foreach (Question q in questions.Item2)
            {
                data.Add(new QuestionData(q));
            }
            return data;
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