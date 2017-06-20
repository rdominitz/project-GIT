using communication.Core;
using communication.Models.GetQuestion;
using communication.Models.GetTest;
using communication.Models.Subjects;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AddTopicController : Controller
    {
        // GET: /AddTopic/
        [HttpGet]
        public ActionResult Index(string message)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("TestId");
            removeCookie("groupName");
            return View(getData());
        }

        private List<GetQuestionData> getData()
        {
            List<GetQuestionData> data = new List<GetQuestionData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopicsGetAQuestion(subject);
                data.Add(new GetQuestionData(subject, list));
            }
            return data;
        }

        [HttpPost]
        public ActionResult Submit(string subject, string topicName)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.topicName = topicName;
            ViewBag.subject = subject;
            string ans = ServerWiring.getInstance().addTopic(Convert.ToInt32(cookie.Value),subject, topicName);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Administration", new { message = "Topic added successfully" });
            }
            return RedirectToAction("Index", "AddTopic", new { message = ans });
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