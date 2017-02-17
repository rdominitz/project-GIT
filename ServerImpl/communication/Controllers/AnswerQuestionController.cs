﻿using communication.Core;
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
            
            Tuple<string, Question> q = ServerWiring.getInstance().getNextQuestion(Convert.ToInt32("100000"));
            List<string> topics = ServerWiring.getInstance().getSubjectTopics(q.Item2.subjectName);
            ViewBag.topics = topics;
            if(!q.Item1.Equals(Replies.SUCCESS))
            {
                return null;//todo redirect to 
            }
            for (int i = 0; i < q.Item2.images.Count; i++)
            {
                ViewData["Image" + i] = q.Item2.images.ElementAt(i);
            }
            return View();
        }
    }
}