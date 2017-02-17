using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace communication.Controllers
{
    public class ImagesController : Controller
    {
        // GET: Images
        public ActionResult Image(string id)
        {
            var dir = Server.MapPath("/Images");
            var path = System.IO.Path.Combine(dir, id + ".jpg"); //validate the path for security or use other means to generate the path.
            return base.File(path, "image/jpeg");
        }
    }
}