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
            Tuple<string, List<Question>> q = ServerWiring.getInstance().getAnsweres(Convert.ToInt32(cookie.Value));

            List<ShowAnswersData> questions = new List<ShowAnswersData>();
            foreach(Question ques in q.Item2)
            {
                questions.Add(new ShowAnswersData(ques));
            }

           /* List<List<String>> pics = new List<List<string>>();
            for (int i = 0; i < q.Item2.Count; i++)
            {
                List<String> lst = new List<String>();
                for (int j = 0; j < q.Item2.ElementAt(i).images.Count; j++)
                { 
                    lst.Add(q.Item2.ElementAt(i).images.ElementAt(j).ImageId);
                }
                pics.Add(lst);
            }
            ViewBag["images"] = pics;*/
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