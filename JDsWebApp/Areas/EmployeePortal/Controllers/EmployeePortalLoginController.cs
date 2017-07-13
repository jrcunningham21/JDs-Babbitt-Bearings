using JDsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeePortalLogin;
using NLog;
using JDsWebApp.Areas.Services;
using System.Security.Principal;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class EmployeePortalLoginController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // GET: EmployeePortal/EmployeePortalLogin
        public ActionResult Index()
        {
            return View(new EmployeeLoginViewModel());
        }

        [HttpPost]
        public ActionResult LogIn(EmployeeLoginViewModel model)
        {
            CookieHelper helper = new CookieHelper();
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            if (ValidateUser(model.Pin))
            {
                if (helper.GetCookie("EmployeeRole") == "Manager")
                    return RedirectToAction("EmployeeMgmt", "EmployeeManagement", new { area = "EmployeePortal" });
                else
                    return RedirectToAction("Vacation", "EmployeeManagement", new { area = "EmployeePortal" });
            }
            else
            {
                ModelState.AddModelError("Pin", "Please enter a valid PIN");
                return View("Index");
            }
        }

        public bool ValidateUser(string PIN)
        {
            if (string.IsNullOrWhiteSpace(PIN))
                return false;

            // Find the employee that matches the given PIN
            Employee employee = null;

            using (var db = new JDsDBEntities())
            {
                employee = db.Employees.FirstOrDefault(e => string.Equals(e.PIN, PIN.Trim()));

                //PIN validation against DB.
                if (employee == null)
                    return false;
                CookieHelper helper = new CookieHelper();
                // This is used to set the user identity name depending on if they
                // have management skill
                string role = "";
                if (employee != null)
                {
                    var theSkill = employee.Skills.FirstOrDefault(x => x.Name == "Management");
                    if (theSkill != null)
                    {
                        role = "Manager";
                    }
                    else
                    {
                        role = "Employee";
                    }

                    helper.SetCookie("EmployeeName", employee.Name);
                    helper.SetCookie("EmployeeRole", role);
                    helper.SetCookie("EmployeeID", employee.EmployeeId.ToString());
                }
            }

            if (employee != null)
                _logger.Trace("Employee '{0}' has logged in via the PIN entry.", employee.Name);           

            return true;
        }

        public ActionResult LogOut()
        {
            CookieHelper helper = new CookieHelper();
            helper.DeleteCookies();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}