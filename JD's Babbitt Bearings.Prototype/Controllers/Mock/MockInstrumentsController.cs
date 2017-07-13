using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockInstrumentsController : Controller
    {
        public ActionResult ManageInstruments()
        {
            return View();
        }

        public ActionResult AddEditInstrument()
        {
            return View();
        }
    }
}