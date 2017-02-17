using Entities;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class BaseController : Controller
    {
        protected static IServer server = new ServerImpl(new MedTrainDBContext());
    }
}