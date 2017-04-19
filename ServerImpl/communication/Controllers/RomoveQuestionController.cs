using communication.Core;
using communication.Models.GetTest;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class RomoveQuestionController : Controller
    {
        //
        // GET: /RomoveQuestion/
        [HttpGet]
        public ActionResult Index(string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.message = message;
            return View(getData());
        }


        private List<GetTestData> getData()
        {
            List<GetTestData> data = new List<GetTestData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopics(subject);
                data.Add(new GetTestData(subject, list));
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


	}
}