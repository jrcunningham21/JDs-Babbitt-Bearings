using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers.Mock
{
    public class MockPhotoController : Controller
    {
        // GET: MockPhoto
        public ActionResult ManagePhotos()
        {
            return View();
        }

        public ActionResult TakePhoto()
        {
            return View();
        }
    }
}