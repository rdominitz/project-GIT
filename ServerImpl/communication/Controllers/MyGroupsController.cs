﻿using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class MyGroupsController : Controller
    {
        // GET: MyGroups
        public ActionResult Index(string message)
        {
            HttpCookie cookie = Request.Cookies["userId"];
            if (cookie == null)
            {
                return RedirectToAction("Index", "Login", new { message = "you are not logged in. please log in and then try again" });
            }
            removeCookie("TestId");
            removeCookie("groupName");
            if (message != null)
            {
                ViewBag.message = message;
            }
            string name = ServerWiring.getInstance().getUserName(Convert.ToInt32(cookie.Value));
            Boolean isAdmin = ServerWiring.getInstance().isAdmin(Convert.ToInt32(cookie.Value));
            Tuple<string, List<String>> groupsRes = ServerWiring.getInstance().getUsersGroups(Convert.ToInt32(cookie.Value));
            if (!groupsRes.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = groupsRes.Item1 });
            }
            List<String> groups = groupsRes.Item2;
            Tuple<string, List<String>> groupsInvitationsRes = ServerWiring.getInstance().getUsersGroupsInvitations(Convert.ToInt32(cookie.Value));
            if (!groupsInvitationsRes.Item1.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = groupsInvitationsRes.Item1 });
            }
            List<String> groupsInvitations = groupsInvitationsRes.Item2;
            ViewBag.name = name;
            ViewBag.isAdmin = isAdmin;
            ViewData["groups"] = groups;
            ViewData["groupsInvitations"] = groupsInvitations;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string[] groupInvites, string submitButton)
        {
            switch (submitButton)
            {
                case "accept":
                    // delegate sending to another controller action
                    return (Submit_accept(groupInvites));
                case "decline":
                    // call another action to perform the cancellation
                    return (Submit_decline(groupInvites));
                default:
                    // If they've submitted the form without a submitButton, 
                    // just return the view again.
                    return (View());
            }
        }


        [HttpPost]
        public ActionResult Submit_accept(string[] groupInvites)
        {
            HttpCookie userCookie = Request.Cookies["userId"];
            if (userCookie == null)
            {

                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            if(groupInvites == null)
            {
                return RedirectToAction("index", "MyGroups", new { message = "you did not choose anything, please choose at least one group" });
            }
            List<string> list = new List<string>(groupInvites);
            string s = ServerWiring.getInstance().acceptUsersGroupsInvitations(Convert.ToInt32(userCookie.Value), list);
            if (!s.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = s });
            }
           
            return RedirectToAction("index", "MyGroups");
        }

        [HttpPost]
        public ActionResult Submit_decline(string[] groupInvites)
        {
            HttpCookie userCookie = Request.Cookies["userId"];
            if (userCookie == null)
            {

                return RedirectToAction("Index", "Login", new { message = "you were not logged in. please log in and then try again" });
            }
            if (groupInvites == null)
            {
                return RedirectToAction("index", "MyGroups", new { message = "you did not choose anything, please choose at least one group" });
            }
            List<string> list = new List<string>(groupInvites);
            string s = ServerWiring.getInstance().declineUsersGroupsInvitations(Convert.ToInt32(userCookie.Value), list);
            if (!s.Equals(Replies.SUCCESS))
            {
                return RedirectToAction("Index", "Main", new { message = s });
            }

            return RedirectToAction("index", "MyGroups");
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