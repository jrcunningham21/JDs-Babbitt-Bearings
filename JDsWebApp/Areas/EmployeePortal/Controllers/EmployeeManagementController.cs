using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Net;
using Antlr.Runtime;
using JDsDataModel;
using JDsWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeManagement;
using NLog;
using JDsWebApp.Areas.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using JDsWebApp.Areas.EmployeePortal.Models.Vacation;
using JDsWebApp.Helpers;
using JDsWebApp.Areas.EmployeePortal.Models.VacationReport;
using System.Threading.Tasks;
using System.Web.Security;
using JDsDataModel.ViewModels;
using JDSignOff = JDsDataModel.SignOff;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class VacationSignoffResponse
    {
        public bool RequiresSignOff { get; set; }
        public string Reason { get; set; }
    }

    public class EmployeeManagementController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        public EmployeeManagementController()
        {
            _vacationService = new SchedulerVacationService();
        }

        // GET: EmployeePortal/EmployeeManagement
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Vacation()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult VacationReport()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TimesheetsReport()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BillingReport()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Skills()
        {
            return View();
        }

        #region Employee Management
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult EmployeeMgmt()
        {
            return View();
        }

        [HttpGet]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetEmployees()
        {
            var employees = _db.Employees.Where(x => x.IsActive.HasValue && x.IsActive.Value).OrderBy(x => x.Name)
                    .Select(x =>
                  new
                  {
                      EmployeeName = x.Name,
                      EmployeeId = x.EmployeeId,
                  }).ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult EmployeeDetails(int id = 0)
        {
            var viewmodel = new EmployeeViewModel();

            if (id == 0)
            {
                viewmodel.EmployeeId = 0;
                viewmodel.AddressId = 0;
                viewmodel.Name = string.Empty;
                viewmodel.Email = string.Empty;
                viewmodel.HireDate = DateTime.Today;
                viewmodel.VacationHoursEarned = null;
                viewmodel.VacationHoursUsed = null;
                viewmodel.EmergencyContact = string.Empty;
                viewmodel.EmergencyPhone = string.Empty;
                viewmodel.Phone = string.Empty;
                viewmodel.Notes = string.Empty;
                viewmodel.PIN = string.Empty;
                viewmodel.AddressLine1 = string.Empty;
                viewmodel.AddressLine2 = string.Empty;
                viewmodel.City = string.Empty;
                viewmodel.State = string.Empty;
                viewmodel.Zip = string.Empty;
                viewmodel.Country = string.Empty;
            }
            else
            {
                // Existing employee
                var employee = _db.Employees.Find(id);

                if (employee == null)
                {
                    _logger.Error("Employee details for EmployeeId={0} not found.", id);
                }
                else
                {
                    viewmodel.EmployeeId = employee.EmployeeId;
                    viewmodel.AddressId = employee.AddressId.HasValue ? employee.AddressId.Value : 0;
                    viewmodel.Name = employee.Name;
                    viewmodel.Email = employee.Email;
                    viewmodel.HireDate = employee.HireDate;
                    viewmodel.VacationHoursEarned = VacationCalculator.CalculateVacationHours(viewmodel.HireDate);
                    viewmodel.VacationHoursUsed = VacationCalculator.CalculateHoursUsed(employee.EmployeeId);
                    viewmodel.EmergencyContact = employee.EmergencyContact;
                    viewmodel.EmergencyPhone = employee.EmergencyPhone;
                    viewmodel.Phone = employee.Phone;
                    viewmodel.Notes = employee.Notes;
                    viewmodel.PIN = employee.PIN;

                    var address = _db.Addresses.Find(viewmodel.AddressId);

                    if (address != null)
                    {
                        viewmodel.AddressLine1 = address.AddressLine1;
                        viewmodel.AddressLine2 = address.AddressLine2;
                        viewmodel.City = address.City;
                        viewmodel.State = address.State;
                        viewmodel.Zip = address.Zip;
                        viewmodel.Country = address.Country;
                    }
                }
            }

            return PartialView("_EmployeeDetails", viewmodel);
        }


        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SaveEmployee(EmployeeViewModel model)
        {
            // Server side validation
            if (!ModelState.IsValid)
            {
                return PartialView("_EmployeeDetails", model);
            }

            // Create new or edit existing
            var employee = model.EmployeeId == 0 ? new Employee() : _db.Employees.Find(model.EmployeeId);
            employee.IsActive = true;

            // Sanity check, care should be taken with included .js and client submits
            // Prevent multiple submits of the same record
            if (model.EmployeeId == 0)
            {
                // If it looks like a duplicate, seems like a duplicate, it must be a duplicate right.
                var dup = _db.Employees.Include("Address").FirstOrDefault(x => x.Name.Equals(model.Name));

                if (dup != null)
                {
                    string msg = string.Format("Duplicate employee record for '{0}' found, save failed to prevent duplicate record of the same employee.", model.Name);
                    _logger.Error(msg);
                    // TODO: inform the client UI of this perhaps.
                    ModelState.AddModelError("", msg);
                    return PartialView("_EmployeeDetails", model);
                }

                // Check for duplicate PINs. This is a new one, so there should be 0 matches
                var dupPin = _db.Employees.FirstOrDefault(x => x.PIN == model.PIN);
                if (dupPin != null)
                {
                    string msg = string.Format("Duplicate PIN '{0}' found, save failed to prevent duplicate PIN.", model.Name);
                    _logger.Error(msg);
                    ModelState.AddModelError("", msg);
                    // TODO: inform the client UI of this perhaps.
                    return PartialView("_EmployeeDetails", model);
                }
            }
            else
            {
                // This is an edit; make sure they're not setting this PIN to an existing PIN
                var dupPins = _db.Employees.Where(x => x.PIN == model.PIN).ToList();
                if (dupPins.Count == 1 && dupPins.First().EmployeeId == model.EmployeeId)
                {
                    // We're OK since this isn't an edit to the PIN
                }
                else if (dupPins.Count == 0)
                {
                    // We're also OK since we're changing this to an unused PIN
                }
                else
                {
                    // We've tried to set the PIN to somebody else's PIN, this is bad.
                    string msg = string.Format("Duplicate PIN '{0}' found, save failed to prevent duplicate PIN.", model.Name);
                    _logger.Error(msg);
                    ModelState.AddModelError("", msg);
                    // TODO: inform the client UI of this perhaps.
                    return PartialView("_EmployeeDetails", model);
                }
            }

            if (employee == null)
            {
                _logger.Error("Unable to save EmployeeID={0} because the employee record was not found.", model.EmployeeId);
                return PartialView("_EmployeeDetails", new EmployeeViewModel());
            }

            var address = model.AddressId == 0 ? new Address() : _db.Addresses.Find(model.AddressId);

            if (address == null)
            {
                address = new Address();
            }
            
            employee.Address = address;
            employee.Name = string.IsNullOrWhiteSpace(model.Name) ? string.Empty : model.Name.Trim();
            employee.Email = string.IsNullOrWhiteSpace(model.Email) ? string.Empty : model.Email.Trim();
            employee.HireDate = model.HireDate;
            employee.EmergencyContact = string.IsNullOrWhiteSpace(model.EmergencyContact) ? string.Empty : model.EmergencyContact.Trim();
            employee.EmergencyPhone = string.IsNullOrWhiteSpace(model.EmergencyPhone) ? string.Empty : model.EmergencyPhone.Trim();
            employee.Phone = string.IsNullOrWhiteSpace(model.Phone) ? string.Empty : model.Phone.Trim();
            employee.Notes = string.IsNullOrWhiteSpace(model.Notes) ? string.Empty : model.Notes.Trim();
            employee.PIN = model.PIN;

            address.AddressLine1 = string.IsNullOrWhiteSpace(model.AddressLine1) ? string.Empty : model.AddressLine1.Trim();
            address.AddressLine2 = string.IsNullOrWhiteSpace(model.AddressLine2) ? string.Empty : model.AddressLine2.Trim();
            address.City = string.IsNullOrWhiteSpace(model.City) ? string.Empty : model.City.Trim();
            address.State = string.IsNullOrWhiteSpace(model.State) ? string.Empty : model.State.Trim();
            address.Zip = string.IsNullOrWhiteSpace(model.Zip) ? string.Empty : model.Zip.Trim();
            address.Country = string.Empty;

            if (model.AddressId == 0)
            {
                _db.Addresses.Add(address);
            }
            else
            {
                var addressEntry = _db.Entry<Address>(address);
                addressEntry.State = EntityState.Modified;
            }

            if (model.EmployeeId == 0)
            {
                _db.Employees.Add(employee);
            }
            else
            {
                var employeeEntry = _db.Entry<Employee>(employee);
                employeeEntry.State = EntityState.Modified;
            }

            // Need to catch or watch for duplicate pins here
            // This method gets called multiple times and can violate the duplicity constraint on the PIN
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                // is ok
                _logger.Trace("Error saving: ", e.ToString());
                ModelState.AddModelError("", e.Message);
            }

            _logger.Trace("Updated employee information for EmployeeId={0} '{1}'", employee.EmployeeId, employee.Name);

            return EmployeeDetails(employee.EmployeeId);
        }
        
        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult DeleteEmployee(int id)
        {
            var employee = _db.Employees.Find(id);

            if (employee == null)
            {
                string error = string.Format("Delete employee for EmployeeId={0} not found.", id);
                _logger.Error(error);
                return Content(error);
            }
            else
            {
                _logger.Trace("EmployeeId={0} '{1}' marked as inactive", employee.EmployeeId, employee.Name);
                employee.IsActive = false;
                var entry = _db.Entry<Employee>(employee);
                entry.State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Content("");
        }
        #endregion

        #region Vacation scheduler
        private SchedulerVacationService _vacationService;        
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
             return Json(_vacationService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public virtual JsonResult DoesVacationRequireSignoff(string startStr, string endStr, int employeeId, bool isFullVacationDay, bool isVacationHours = true, int vacationID = 0)
        {
            try
            {
                return Json(VacationRequiresSignoff(startStr, endStr, employeeId, isFullVacationDay, isVacationHours, vacationID), JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                // there was a problem
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        private static VacationSignoffResponse VacationRequiresSignoff(string startStr, string endStr, int employeeId, bool isFullVacationDay, bool isVacationHours = true, int vacationID = 0)
        {
            if (employeeId == 0)
            {
                CookieHelper helper = new CookieHelper();
                employeeId = int.Parse(helper.GetCookie("EmployeeID"));
            }

            DateTime start = DateTime.Parse(startStr);
            DateTime end = DateTime.Parse(endStr);

            // Vacation requires sign off IF
            // user would have negative hours OR
            // requested vacation starts within the next 10 calendar days  OR
            // there are already 2 people with vacation scheduled on that day    
            using (var db = new JDsDBEntities())
            {
                // Check to see if the vacation isn't scheduled enough in advance
                var tenDaysFromNow = DateTime.Today.AddDays(10);
                if (tenDaysFromNow > start)
                {
                    var result = new VacationSignoffResponse()
                    {
                        RequiresSignOff = true,
                        Reason = "Not enough notice for vacation."
                    };
                    return result;
                }

                // Find overlapping vacations
                var overlapCount = db.Vacations.Count(x => x.VacationId != vacationID && x.StartDate <= end && x.EndDate >= start);
                if (overlapCount >= 2)
                {
                    var result = new VacationSignoffResponse()
                    {
                        RequiresSignOff = true,
                        Reason = "2 or more employees scheduled for vacation."
                    };
                    return result;
                }

                Employee employee = db.Employees.Single(x => x.EmployeeId == employeeId);
                int vacationHours = 0;

                if (!VacationCalculator.HasAvaliableVacationHours(employeeId, start, end, isVacationHours, out vacationHours, vacationID))
                {
                    var result = new VacationSignoffResponse()
                    {
                        RequiresSignOff = true,
                        Reason = "Not enough earned hours for this vacation."
                    };
                    return result;
                }

            }

            var isOK = new VacationSignoffResponse()
            {
                RequiresSignOff = false,
                Reason = ""
            };

            return isOK;
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public virtual JsonResult Destroy([DataSourceRequest] DataSourceRequest request, VacationViewModel task)
        {
            if (ModelState.IsValid)
            {
                _vacationService.Delete(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public virtual JsonResult Create([DataSourceRequest] DataSourceRequest request, VacationViewModel task)
        {
            _logger.Trace("EmployeeManagementController - Create Vacation entry called");
            if (ModelState.IsValid)
            {
                _logger.Trace("Vacation Insert - Model is valid");

                // check to see if the signoff was required - if so, grab the last one and update the frickin task model
                var response = VacationRequiresSignoff(task.Start.ToString(), task.End.ToString(), task.EmployeeId, task.IsFullVacationDay);
                if (response.RequiresSignOff)
                {
                    // there was just barely a signoff added, associate it to this vacation request.
                    // Note that there is a small chance of a nasty race condition bug.
                    using (var db = new JDsDBEntities())
                    {
                        var lastSignOff = db.SignOffs.OrderByDescending(x => x.SignOffId).FirstOrDefault();
                        if (lastSignOff != null)
                            task.SignOffId = lastSignOff.SignOffId;
                    }
                }

                _vacationService.Insert(task, ModelState);
            }
            else
            {
                _logger.Trace("Vacation Insert - Model is NOT valid");
            }

            task.End = task.End.AddMilliseconds(1);

            return Json(new[] { task }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, VacationViewModel task)
        {
            if (ModelState.IsValid)
            {
                _vacationService.Update(task, ModelState);
            }

            task.End = task.End.AddMilliseconds(1);

            return Json(new[] { task }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult GetEmployeesForVacationDropdown()
        {
            using (var db = new JDsDBEntities())
            {
                var employees = db.Employees.Where(x => x.IsActive.HasValue && x.IsActive.Value).OrderBy(x => x.Name)
                    .Select(x =>
                  new
                  {
                      EmployeeName = x.Name,
                      EmployeeId = x.EmployeeId,
                  }).ToList();

                return Json(employees, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult GetVacationHoursRemaining(int id, string startDateString, string endDateString,
            bool isFullVacation = true, bool isVacationTime = true, int vacationID = 0)
        {
            using (var db = new JDsDBEntities())
            {
                Employee employee = db.Employees.SingleOrDefault(x => x.EmployeeId == id);

                if (employee?.HireDate != null)
                {
                    DateTime endVacationDateTime = DateTime.Parse(endDateString);
                    DateTime startVacationDateTime = DateTime.Parse(startDateString);

                    int earnedVacationHours = VacationCalculator.CalculateVacationHours(employee.HireDate,
                        endVacationDateTime, isVacationTime);

                    int hoursUsed = VacationCalculator.CalculateHoursUsed(id, endVacationDateTime, vacationID,
                        isVacationTime);

                    int vacationTimeInHours = VacationCalculator.GetNumberOfVacationHoursUsedForPossibleVacation(id,
                        startVacationDateTime, endVacationDateTime, isFullVacation);

                    return Json(earnedVacationHours - hoursUsed - vacationTimeInHours, JsonRequestBehavior.AllowGet);
                }

                return Json(0, JsonRequestBehavior.AllowGet);
            }    
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public string GetRole()
        {
            CookieHelper helper = new CookieHelper();
            var x = helper.GetCookie("EmployeeRole");
            return x;
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public string GetId()
        {
            CookieHelper helper = new CookieHelper();
            return helper.GetCookie("EmployeeId");
        }

        #endregion

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult SignOff(string skillName)
        {
            SignOffViewModel model = new SignOffViewModel()
            {
                SkillName = skillName
            };

            return PartialView(model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult SignOff(SignOffViewModel model)
        {
            try
            {
                Employee employee = _db.Employees.FirstOrDefault(x => x.PIN == model.Pin);

                if (employee != null)
                {
                    bool hasSkill = employee.Skills.FirstOrDefault(s => s.Name == model.SkillName) != null ? true : false;

                    if (hasSkill)
                    {
                        JDSignOff signOff = new JDSignOff()
                        {
                            EmployeeId = employee.EmployeeId,
                            RequiredSkillId = employee.Skills.Single(s => s.Name == model.SkillName).SkillId,
                            SignOffDate = DateTime.Now
                        };

                        _db.SignOffs.Add(signOff);
                        _db.SaveChanges();
                        model.SignOffId = signOff.SignOffId;

                        return Json(new { result = true, signOffId=model.SignOffId });
                    }
                    else
                    {
                        ModelState.AddModelError("SignOffError", "Missing required skill.");
                    }
                }
                else
                {
                    ModelState.AddModelError("SignOffError", "Employee not found.");
                }
                return PartialView(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("SignOffError", "Error has occured.");
                return PartialView(model);
            }
        }
    }
}