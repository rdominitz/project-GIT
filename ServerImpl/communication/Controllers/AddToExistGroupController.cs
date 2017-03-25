﻿using communication.Core;
using communication.Models.Group;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class AddToExistGroupController : Controller
    {
        // GET: AddToExistGroup
        [HttpGet]
        public ActionResult Index(string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            ViewBag.message = message;
            
            return View(getData(Convert.ToInt32(cookie.Value)));
        }

        [HttpPost]
        public ActionResult Submit(string groupName, string inviteEmails, string emailContent)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }

            ViewBag.groupName = groupName;
            ViewBag.inviteEmails = inviteEmails;
            ViewBag.emailContent = emailContent;

            string ans = ServerWiring.getInstance().addToGroup(groupName, inviteEmails, emailContent);
            if (ans.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main");
            }
            return RedirectToAction("Index", "AddToExistGroup", new { message = ans });
        }

        List<GroupData> getData(int adminId)
        {
            List<GroupData> data = new List<GroupData>();
            List<string> groups = ServerWiring.getInstance().getAllAdminsGroups(adminId);
            foreach (string group in groups)
            {
                data.Add(new GroupData(group));
            }
            return data;
        }


    }
}