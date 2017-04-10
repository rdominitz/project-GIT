using communication.Core;
using communication.Models.GetQuestion;
using communication.Models.Questions;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class SelectQuestionsController : Controller
    {
        // GET: SelectQuestions
        [HttpGet]
        public ActionResult Index(List<Question> questions)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            List<QuestionData> questionsData = new List<QuestionData>();
            if (questions != null)
            {
                foreach (Question ques in questions)
                {
                    questionsData.Add(new QuestionData(ques));
                }
            
            return View(questionsData);
            }
            else
                return View();

        }
    }
}