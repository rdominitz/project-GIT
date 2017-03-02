using communication.Core;
using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace communication.Controllers
{
    public class RegisterController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string email, string fname, string lname, string password, string level)
        {
            ViewBag.email = email;
            ViewBag.fname = fname;
            ViewBag.lname = lname;

            Tuple<string, int> ans = ServerWiring.getInstance().register(email, password, convert(level), fname, lname);
            if(ans.Item1.Equals(Replies.SUCCESS))
            {
               
               
                return RedirectToAction("Index", "Main");
            }
            ViewBag.errorMessage = ans.Item1;
            return View("index");
        }

        private string convert(string level)
        {
            switch (level)
            {
                case "PreMedicalstudent":
                    return "Pre-Medical student";
                case "Medicalstudent1styear":
                    return "Medical student - 1st year";
                case "Medicalstudent2ndyear":
                    return "Medical student - 2nd year";
                case "Medicalstudent3rdyear":
                    return "Medical student - 3rd year";
                case "Medicalstudent4thyear":
                    return "Medical student - 4th year +";
                case "ResidentPGY1":
                    return "Resident PGY 1";
                case "ResidentPGY2":
                    return "Resident PGY 2";
                case "ResidentPGY3":
                    return "Resident PGY 3";
                case "ResidentPGY4":
                    return "Resident PGY 4";
                case "ResidentPGY5":
                    return "Resident PGY 5";
                case "ResidentPGY6":
                    return "Resident PGY 6";
                case "ResidentPGY7":
                    return "Resident PGY 7";
                case "Fellow":
                    return "Fellow";
                default:
                    return "Attending";
            }
        }
    }
}
