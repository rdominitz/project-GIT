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
    public class CreateTestController : Controller
    {
        // GET: CreateTest
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


        List<GetTestData> getData()
        {
            List<GetTestData> data = new List<GetTestData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopics(subject);
                list.Remove(Constants.Topics.NORMAL);
                data.Add(new GetTestData(subject, list));
            }
            return data;
        }

        [HttpGet]
        public List<string> SubjectChanged(string subject)
        {
            return ServerWiring.getInstance().getSubjectTopics(subject);

        }

        [HttpPost]
        public ActionResult Submit(string testName, string subject, string topics, int numberOfQuestions, int numberOfNormalQuestions)
        {

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.testName = testName;
            ViewBag.subject = subject;
            ViewBag.topics = topics;


            string ans = ServerWiring.getInstance().createTest(Convert.ToInt32(cookie.Value), testName, subject, topics);                
            if (ans == Replies.SUCCESS)
            {
                // redirect to chose questions 
                return RedirectToAction("Index", "Main");
            }


            return RedirectToAction("Index", "CreateTest", new { message = ans });
        }


    }
}