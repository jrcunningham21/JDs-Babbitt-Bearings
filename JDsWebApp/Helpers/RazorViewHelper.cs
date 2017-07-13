﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JDsWebApp.Helpers
{
    public static class RazorViewHelper
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            try
            {

                controller.ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "";
        }
    }
}
