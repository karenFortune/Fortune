using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Layout()
        {
            ViewData["cargo"] = 1;
            ViewBag.Nombre = "Karen";
            return View();
        }
    }
}