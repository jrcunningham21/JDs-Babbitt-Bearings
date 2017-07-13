using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers
{
    public class JobDetailController : Controller
    {
        //
        // GET: /JobDetail/


        public ActionResult Index(int? id)
        {
            ViewBag.Title = id == null ? "New Job" : "Job Details";
            return View();
        }
    }
}
