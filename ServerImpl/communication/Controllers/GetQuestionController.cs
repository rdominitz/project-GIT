﻿using communication.Core;
using communication.Models.GetQuestion;
using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class GetQuestionController : Controller
    {
        [HttpGet]
        public ActionResult Index(string message)
        {
            ViewBag.message = message;
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            List<GetQuestionData> data = new List<GetQuestionData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopicsGetAQuestion(subject);
                list.Remove(Constants.Topics.NORMAL);
                data.Add(new GetQuestionData(subject, list));
            }
            // ViewBag.data = data;
            //ViewBag.subjects = subjects;
            return View(data);
        }

        [HttpPost]
        public ActionResult Submit(string subject, string topic)
        {
            ViewBag.subject = subject;
            ViewBag.topic = topic;

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            //string ans = ServerWiring.getInstance().getAutoGeneratedQuesstion(Convert.ToInt32("100000"), subject, topic);
            string ans = ServerWiring.getInstance().getAutoGeneratedQuesstion(Convert.ToInt32(cookie.Value), subject, topic);
            if (ans == Replies.SUCCESS)
            {
                return RedirectToAction("Index", "AnswerQuestion");
            }
            //ViewBag.errorMessage = ans;
            return RedirectToAction("Index", "GetQuestion", new { message = ans });
        }

        [HttpGet]
        public List<string> SubjectChanged(string subject)
        {
            return ServerWiring.getInstance().getSubjectTopicsGetAQuestion(subject);
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