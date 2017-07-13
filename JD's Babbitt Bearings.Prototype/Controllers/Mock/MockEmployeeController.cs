using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockEmployeeController : Controller
    {
        // GET: MockEmployee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEditEmployee()
        {
            return View();
        }
    }
}