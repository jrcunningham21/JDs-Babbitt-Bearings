using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockCustomerController : Controller
    {
        // GET: MockCustomer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEditCustomer()
        {
            return View();
        }

        public ActionResult CustomerJobPortal()
        {
            return View();
        }
    }
}