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
        public ActionResult Index()
        {

            HttpCookie cookie = Request.Cookies["userId"];
            Tuple<string, Question> q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32(cookie.Value));
            HttpCookie questionCookie = new HttpCookie("questionId", q.Item2.QuestionId.ToString());
            Response.SetCookie(questionCookie);
            //Tuple<string, Question> q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32("100000"));
            List<string> topics = ServerWiring.getInstance().getSubjectTopics(q.Item2.subjectName);
            ViewBag.topics = topics;
            if(!q.Item1.Equals(Replies.SUCCESS))
            {
                return null;//todo redirect to 
            }

            List<String> lst = new List<String>();
            for (int i = 0; i < q.Item2.images.Count; i++)
            {
                //ViewData["Image" + i] = q.Item2.images.ElementAt(i);
                lst.Add(q.Item2.images.ElementAt(i));
            }
            ViewData["Images"] = lst;
            return View();
        }


        [HttpPost]
        public ActionResult Submit(string norm, int sure1,  string[] diagnosis, int[] sure2)
        {

            HttpCookie questionCookie = Request.Cookies["questionId"];
            HttpCookie userCookie = Request.Cookies["userId"];
            List<string> diagnosisList = diagnosis.ToList();
            List<int> sure2List = sure2.ToList();
            string ans = ServerWiring.getInstance().AnswerAQuestion(Convert.ToInt32(userCookie.Value), Convert.ToInt32(questionCookie.Value),norm.Equals("true"), sure1, diagnosisList, sure2List);
            if (ans.Equals("Show answer"))
            {
                bool hasMoreQuestions = ServerWiring.getInstance().
                return RedirectToAction("Index", "AnswerQuestion");
            }
            else
            {
                return View("index");
            }
        }
    }
}