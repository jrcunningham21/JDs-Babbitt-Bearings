using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockTimesheetsController : Controller
    {
        public ActionResult TimesheetEntry()
        {
            return View();
        }

        public ActionResult TimesheetReport()
        {
            return View();
        }

    }
}