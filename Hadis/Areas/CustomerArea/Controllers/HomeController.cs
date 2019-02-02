using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hadis.Areas.CustomerArea.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: CustomerArea/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}