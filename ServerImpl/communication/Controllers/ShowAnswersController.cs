using communication.Core;
using communication.Models.ShowAnswers;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class ShowAnswersController : Controller
    {
        // GET: ShowAnswers
        public ActionResult Index(bool hasMoreQuestions)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            Tuple<string, List<Question>> q = ServerWiring.getInstance().getAnsweres(Convert.ToInt32(cookie.Value));

            List<ShowAnswersData> questions = new List<ShowAnswersData>();
            foreach(Question ques in q.Item2)
            {
                questions.Add(new ShowAnswersData(ques));
            }

            
             //ViewBag["images"] = pics;
            string next;
            if (hasMoreQuestions)
            {
                next = "Main";
            }
            else
            {
                next = "AnswerQuestion";
            }
            ViewData["next"] = next;
            return View(questions);
        }
    }
}