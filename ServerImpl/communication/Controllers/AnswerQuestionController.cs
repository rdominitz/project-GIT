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
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ViewBag.message = message;
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            Tuple<string, Question> q = null;
            HttpCookie testCookie = Request.Cookies["testID"];
            HttpCookie groupCookie = Request.Cookies["groupName"];
            if(groupCookie == null || testCookie == null)
            { 
                q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32(cookie.Value));
            }
            else
            {
                 q = ServerWiring.getInstance().getNextQuestionGroupTest(Convert.ToInt32(cookie.Value),groupCookie.Value,Convert.ToInt32(testCookie.Value));
            }
            if (!q.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Login", new { message = q.Item1 });
            }
            HttpCookie questionCookie = new HttpCookie("questionId", q.Item2.QuestionId.ToString());
            Response.SetCookie(questionCookie);
            List<string> topics = ServerWiring.getInstance().getSubjectTopicsGetAQuestion(q.Item2.SubjectId);
            ViewBag.topics = topics;
            if (!q.Item2.text.Equals(""))
            {
                ViewData["text"] = q.Item2.text;
            }

            List<String> lst = ServerWiring.getInstance().getQuestionImages(q.Item2.QuestionId);
            ViewData["Images"] = lst;

            List<string> subject_list = ServerWiring.getInstance().getSubjectTopicsGetAQuestion(q.Item2.SubjectId);
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
                if(diagnosis == null || sure2 == null)
                {
                    return RedirectToAction("Index", new { message = "if you chosen that the diagnosis is abnormal than you must choose at least one diagnosis" });
                }
                 diagnosisList = diagnosis.ToList();
                 sure2List = sure2.ToList();
            }
            string ans = "";
            bool hasMoreQuestions = false;
            HttpCookie testCookie = Request.Cookies["testID"];
            HttpCookie groupCookie = Request.Cookies["groupName"];
            if (groupCookie == null || testCookie == null)
            {
                 ans = ServerWiring.getInstance().answerAQuestion(Convert.ToInt32(userCookie.Value), Convert.ToInt32(questionCookie.Value), norm.Equals("true"), sure1, diagnosisList, sure2List, true).Item1;
                 hasMoreQuestions = ServerWiring.getInstance().hasMoreQuestions(Convert.ToInt32(userCookie.Value));
            }
            else
            {
                ans = ServerWiring.getInstance().answerAQuestionGroupTest(Convert.ToInt32(userCookie.Value), groupCookie.Value, Convert.ToInt32(testCookie.Value), Convert.ToInt32(questionCookie.Value), norm.Equals("true"), sure1, diagnosisList, sure2List);
                hasMoreQuestions = ServerWiring.getInstance().hasMoreQuestionsGroupTest(Convert.ToInt32(userCookie.Value), groupCookie.Value, Convert.ToInt32(testCookie.Value));
            }
            if (ans.Equals(Replies.SHOW_ANSWER))
            {
                if (hasMoreQuestions)
                {
                    return RedirectToAction("Index", "ShowAnswers", new { hasMoreQuestions = false });
                }
                return RedirectToAction("Index", "ShowAnswers", new { hasMoreQuestions = true });
            }
            else if (ans.Equals(Replies.SUCCESS))
            {
                double grade = ServerWiring.getInstance().getTestGrade(Convert.ToInt32(userCookie.Value), groupCookie.Value, Convert.ToInt32(testCookie.Value));
                return RedirectToAction("Index", "Main", new { message = "your grade is "+grade});
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}