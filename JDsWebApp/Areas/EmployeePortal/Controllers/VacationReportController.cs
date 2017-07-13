using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Net;
using Antlr.Runtime;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeUpload;
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

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class VacationReportController : Controller
    {
        // GET: EmployeePortal/VacationReport
        public ActionResult Index()
        {
            return View();
        }

        #region Vacation Report

        [HttpGet]
        public ActionResult GetEmployees()
        {
            using (JDsDBEntities _db = new JDsDBEntities())
            {
                var employees = _db.Employees.OrderBy(x => x.Name)
                    .Select(x =>
                  new
                  {
                      EmployeeName = x.Name,
                      EmployeeId = x.EmployeeId,
                  }).ToList();

                return Json(employees, JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpGet]
        public ActionResult GetEmployeeVacationDetails(int empID)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var emp = db.Employees.FirstOrDefault(x => x.EmployeeId == empID);

                var vacations = db.Vacations.Where(x => x.EmployeeId == empID).OrderBy(x => x.Day).ToList();

                List<VacationModel> lvm = new List<VacationModel>();
                VacationReportModel vrm = new VacationReportModel();

                // Trim time off end of DateTime string fro StartDate
                string startDate = emp.HireDate.ToString();
                startDate = startDate.Remove(startDate.IndexOf(" "));

                vrm.StartDate = startDate;
                vrm.UsedVacationHours = VacationCalculator.CalculateHoursUsed(emp.EmployeeId);
                vrm.TotalAccumulatedVacationHours = VacationCalculator.CalculateVacationHours(emp.HireDate);
                vrm.RemainingVacationHours = VacationCalculator.CalculateVacationHours(emp.HireDate) - vrm.UsedVacationHours;

                vrm.UsedPersonalHours = VacationCalculator.CalculateHoursUsed(emp.EmployeeId, isVacationTime: false);
                vrm.TotalAccumulatedPersonalHours = VacationCalculator.CalculateVacationHours(emp.HireDate, isVacationTime: false);
                vrm.RemainingPersonalHours = VacationCalculator.CalculateVacationHours(emp.HireDate, isVacationTime: false) - vrm.UsedVacationHours;

                for (int i = 0; i < vacations.Count; i++)
                {
                    try
                    {
                        string vacDate = vacations[i].Day.ToString();
                        vacDate = vacDate.Remove(vacDate.IndexOf(" "));

                        if (vacations[i].SignOff == null)
                        {
                            lvm.Add(new VacationModel
                            {
                                VacationDate = vacDate,
                                ApprovedBy = "N/A",
                                NumHours = vacations[i].NumberOfVacationHoursUsed,
                                HourType = vacations[i].IsVacationTime ? "Vacation" : "Personal"
                            });
                        }
                        else
                        {
                            lvm.Add(new VacationModel
                            {
                                VacationDate = vacDate,
                                ApprovedBy = vacations[i].SignOff.Employee.Name,
                                NumHours = vacations[i].NumberOfVacationHoursUsed,
                                HourType = vacations[i].IsVacationTime ? "Vacation" : "Personal"
                            });
                        }

                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                }

                vrm.Vacations = lvm;

                return Json(vrm, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}