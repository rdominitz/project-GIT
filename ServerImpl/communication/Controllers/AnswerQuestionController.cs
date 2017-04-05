using communication.Core;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AnswerQuestionController : Controller
    {
        // GET: AnswerQuestion
        public ActionResult Index(string message)
        {
            ViewBag.message = message;
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            Tuple<string, Question> q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32(cookie.Value));
            HttpCookie questionCookie = new HttpCookie("questionId", q.Item2.QuestionId.ToString());
            Response.SetCookie(questionCookie);
            //Tuple<string, Question> q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32("100000"));
            List<string> topics = ServerWiring.getInstance().getSubjectTopics(q.Item2.SubjectId);
            ViewBag.topics = topics;
            if(!q.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Login", new { message = q.Item1 });
            }

            List<String> lst = ServerWiring.getInstance().getQuestionImages(q.Item2.QuestionId);
            ViewData["Images"] = lst;

            List<string> subject_list = ServerWiring.getInstance().getSubjectTopics(q.Item2.SubjectId);
            subject_list.Remove(Constants.Topics.NORMAL);
            ViewData["subjects"] =  subject_list;
            return View();
        }


        [HttpPost]
        public ActionResult Submit(string norm, int sure1,  string[] diagnosis, int[] sure2)
        {

            HttpCookie questionCookie = Request.Cookies["questionId"];
            HttpCookie userCookie = Request.Cookies["userId"];
            if (userCookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            List<string> diagnosisList = new List<string>();
            List<int> sure2List = new List<int>();
            if (norm.Equals("false"))
            {
                 diagnosisList = diagnosis.ToList();
                 sure2List = sure2.ToList();
            }

            string ans = ServerWiring.getInstance().answerAQuestion(Convert.ToInt32(userCookie.Value), Convert.ToInt32(questionCookie.Value),norm.Equals("true"), sure1, diagnosisList, sure2List);
            if (ans.Equals("show answer"))
            {
                bool hasMoreQuestions = ServerWiring.getInstance().hasMoreQuestions(Convert.ToInt32(userCookie.Value));
                if (hasMoreQuestions)
                {
                    return RedirectToAction("Index", "ShowAnswers", new { hasMoreQuestions = false });
                }
                return RedirectToAction("Index", "ShowAnswers", new {hasMoreQuestions = true});
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}