﻿using communication.Core;
using communication.Models.GetTest;
using Constants;
using Entities;
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
            removeCookie("TestId");
            removeCookie("groupName");
            ViewBag.message = message;
            return View(getData());
        }


        private List<GetTestData> getData()
        {
            List<GetTestData> data = new List<GetTestData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopicsGetAQuestion(subject);
                list.Remove(Constants.Topics.NORMAL);
                data.Add(new GetTestData(subject, list));
            }
            return data;
        }

        [HttpGet]
        public List<string> SubjectChanged(string subject)
        {
            return ServerWiring.getInstance().getSubjectTopicsGetAQuestion(subject);
        }

        [HttpPost]
        public ActionResult Submit(string subject, string[] topics)
        {

            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            List<string> topicsList = new List<string>();
            topicsList = topics.ToList();

            ViewBag.subject = subject;
            ViewData["topics"] = topics;


            string ans = ServerWiring.getInstance().createTest(Convert.ToInt32(cookie.Value), subject, topicsList);                
            if (ans == Replies.SUCCESS)
            {
                 return RedirectToAction("Index", "SelectQuestions");
            }


            return RedirectToAction("Index", "CreateTest", new { message = ans });
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