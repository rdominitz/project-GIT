using communication.Core;
using communication.Models.GetQuestion;
using Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class CreateQuestionController : Controller
    {
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
            List<GetQuestionData> data = new List<GetQuestionData>();
            List<string> subjects = ServerWiring.getInstance().getAllSubjects();
            foreach (string subject in subjects)
            {
                List<string> list = ServerWiring.getInstance().getSubjectTopicsCreateAQuestion(subject);
                data.Add(new GetQuestionData(subject, list));
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult Submit(string subject, string[] topics, HttpPostedFileBase[] imgs, string freeText)
        {
            ViewBag.subject = subject;
            ViewData["topics"] = topics;
            ViewBag.freeText = freeText;

            ViewData["imgs"] = imgs;
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            removeCookie("testID");
            removeCookie("groupName");
            List<byte[]> allImgs = new List<byte[]>();

            foreach (HttpPostedFileBase img in imgs)
            {
                MemoryStream target = new MemoryStream();
                img.InputStream.CopyTo(target);
                byte[] dataImg = target.ToArray();
                allImgs.Add(dataImg);
            }

            string ans = ServerWiring.getInstance().createQuestion(Convert.ToInt32(cookie.Value), subject, topics.ToList(), allImgs, freeText);
            if (ans == Replies.SUCCESS)
            {
                return RedirectToAction("Index", "Main", new { message = "Question added successfully" });
            }

            return RedirectToAction("Index", "CreateQuestion", new { message = ans });
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