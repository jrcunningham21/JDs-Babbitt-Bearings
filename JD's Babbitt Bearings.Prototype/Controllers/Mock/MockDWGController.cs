using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockDWGController : Controller
    {
        // GET: MockDWG
        public ActionResult ManageDWGs()
        {
            return View();
        }

        public ActionResult AddEditDWG()
        {
            return View();
        }
    }
}