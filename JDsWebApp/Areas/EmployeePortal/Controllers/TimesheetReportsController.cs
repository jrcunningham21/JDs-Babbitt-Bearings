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
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeTimesheets;
using JDsWebApp.Areas.EmployeePortal.Models.TimesheetReports;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class TimesheetReportsController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        // GET: EmployeePortal/TimesheetReports
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTimesheetReportsByDate(string startDate, string endDate, string YR)
        {
            int startMonth, startDay, endMonth, endDay, Year;

            string[] startSplit = startDate.Split('/');
            startMonth = Int32.Parse(startSplit[0]);
            startDay = Int32.Parse(startSplit[1]);

            string[] endSplit = endDate.Split('/');
            endMonth = Int32.Parse(endSplit[0]);
            endDay = Int32.Parse(endSplit[1]);

            Int32.TryParse(YR, out Year);

            DateTime sd = new DateTime(Year, startMonth, startDay);
            DateTime ed = new DateTime(Year, endMonth, endDay);


            using (JDsDBEntities db = new JDsDBEntities())
            {       
                try
                {
                    // In order to avoid repeating code, let's make a timesheet view model, populate it, make all necessary
                    // calculations in the existing employeetimesheetscontroller method. We'll make one entry per employee
                    Dictionary<decimal, TimesheetViewModel> vmMap = new Dictionary<decimal, TimesheetViewModel>();
                    var list = db.TimesheetEntries.Where(x => x.Day >= sd && x.Day <= ed).OrderBy(x => x.Day).ToList();
                    foreach (var entry in list)
                    {
                        if (!entry.EmployeeId.HasValue) continue;

                        // Check to see if this employee has an entry yet
                        if (!vmMap.ContainsKey(entry.EmployeeId.Value))
                        {
                            vmMap.Add(entry.EmployeeId.Value, new TimesheetViewModel());
                            vmMap[entry.EmployeeId.Value].TimesheetEntries = new List<TimesheetEntryViewModel>();
                        }

                        string startString = "";
                        if (entry.StartTime.HasValue)
                            startString = entry.StartTime.Value.ToString("h:mm:tt");
                        string endString = "";
                        if (entry.EndTime.HasValue)
                            endString = entry.EndTime.Value.ToString("h:mm:tt");
                        vmMap[entry.EmployeeId.Value].TimesheetEntries.Add(new TimesheetEntryViewModel()
                        {
                            Day = entry.Day,
                            StartTime = startString,
                            EndTime = endString,
                            IsSkipMeal = entry.WorkedThroughMealtime.HasValue ? entry.WorkedThroughMealtime.Value : false,
                        });
                    }

                    // update the totals for each vm
                    foreach(var entry in vmMap)
                    {
                        EmployeeTimesheetsController.CalculateTimesheetDisplayHourTotals(entry.Value);
                    }
                    
                    

                    // TODO: Calculate totals based on entries
                    var tr = db.TimesheetEntries.OrderBy(x => x.Day).ToList().Where(d => (d.Day >= sd) && (d.Day <= ed)).OrderBy(e => e.EmployeeId).Select(x => new
                    {
                        TimesheetEntryId = x.TimesheetEntryId,
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.Employee.Name,
                        Day = x.Day.ToString("ddd") + " " + x.Day.Month + "/" + x.Day.Day,
                        Date = x.Day.Date.ToString(),
                        NumHoursWorked = x.NumHoursWorked,
                        StartTime = x.StartTime.HasValue ? x.StartTime.Value.ToShortTimeString() : "-" ,
                        EndTime = x.EndTime.HasValue ? x.EndTime.Value.ToShortTimeString() :  "-",
                        WorkedThroughMealtime = x.WorkedThroughMealtime,
                        TotalHours = vmMap[x.EmployeeId.Value].TotalHours,
                        StraightTime = vmMap[x.EmployeeId.Value].TotalStraight,
                        Overtime = vmMap[x.EmployeeId.Value].TotalOvertime,
                        DoubleTime = vmMap[x.EmployeeId.Value].TotalDoubleTime,
                        //TotalHours = x.TotalHours,
                        //StraightTime = x.StraightTime,
                        //Overtime = x.OverTime,
                        //DoubleTime = x.DoubleTime
                    });

                    return Json(tr.ToList(), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [HttpPost]
        public ActionResult GetPayPeriodDates(string YR)
        {
            int ddYear;
            Int32.TryParse(YR, out ddYear);

            // Used to store set of Pay Period date ranges as strings
            List<string> dates = new List<string>();

            // Used to help iterate/store payperiod datetimes through the year
            DateTime payPeriodStart = new DateTime(ddYear, 1, 1);
            DateTime payPeriodEnd = new DateTime(ddYear, 1, 15);

            dates.Add(payPeriodStart.Month + "/" + payPeriodStart.Day + " - " + payPeriodEnd.Month + "/" + payPeriodEnd.Day);

            for (int i = 0; i < 23; i++)
            {
                if (payPeriodStart.Day == 1)
                {
                    var startday = 16;
                    var endday = new DateTime(ddYear, payPeriodStart.Month, DateTime.DaysInMonth(ddYear, payPeriodStart.Month)).Day;

                    payPeriodStart = new DateTime(ddYear, payPeriodStart.Month, startday);
                    payPeriodEnd = new DateTime(ddYear, payPeriodStart.Month, endday);

                    dates.Add(payPeriodStart.Month + "/" + payPeriodStart.Day + " - " + payPeriodEnd.Month + "/" + payPeriodEnd.Day);
                }
                else //payPeriodStart.Day == 16
                {
                    var month = (payPeriodStart.Month + 1);

                    payPeriodStart = new DateTime(ddYear, month, 1);
                    payPeriodEnd = new DateTime(ddYear, month, 15);

                    dates.Add(payPeriodStart.Month + "/" + payPeriodStart.Day + " - " + payPeriodEnd.Month + "/" + payPeriodEnd.Day);
                }

            }

            return Json(dates, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimesheetEntries(int[] employeeId, string payperiod, string Year)
        {
            PrintAllTimesheetsViewModel printvm = new PrintAllTimesheetsViewModel();
            printvm.EmployeeTimesheets = new List<TimesheetViewModel>();
            printvm.PayPeriod = payperiod;

            // It's possible there are no timesheets for the given pay period
            if (employeeId == null)
                employeeId = new int[0] ;

            //DateTime startDate;
            payperiod = payperiod.Replace("\"", "");

            // Gets the value of DatePicker and splits the dates into 2
            var dateRange = payperiod;
            var splitResult = dateRange.Split('-');

            // Convert payperiod & Year to DateTime
            int startMonth, startDay, endMonth, endDay, year;

            string[] startSplit = splitResult[0].Split('/');
            startMonth = Int32.Parse(startSplit[0]);
            startDay = Int32.Parse(startSplit[1]);

            string[] endSplit = splitResult[1].Split('/');
            endMonth = Int32.Parse(endSplit[0]);
            endDay = Int32.Parse(endSplit[1]);

            Int32.TryParse(Year, out year);

            // Make string into datetime
            DateTime PayPeriodStartDate = new DateTime(year, startMonth, startDay);
            DateTime PayPeriodEndDate = new DateTime(year, endMonth, endDay);

            for (int k = 0; k < employeeId.Count(); k++)
            {
                int empId = employeeId[k];

                var empName = _db.Employees.Where(e => e.EmployeeId == empId).Select(x => x.Name).FirstOrDefault();
                

                var viewmodel = new TimesheetViewModel
                {
                    EmployeeId = employeeId[k],
                    EmployeeName = empName,
                    StartPayPeriod = PayPeriodStartDate.ToString(),
                    Year = Year,
                    TimesheetEntries = new List<TimesheetEntryViewModel>()
                };                

                var dbTimesheetEntries = _db.TimesheetEntries.Where(x =>
                    x.EmployeeId == empId &&
                    x.Day >= PayPeriodStartDate &&
                    x.Day <= PayPeriodEndDate).ToList();

                var dbVacations = _db.Vacations.Where(
                    x => x.EmployeeId == empId &&
                         x.Day >= PayPeriodStartDate &&
                         x.Day <= PayPeriodEndDate).ToList();

                var difference = (PayPeriodEndDate - PayPeriodStartDate).Days;
                DateTime dateIterator = PayPeriodStartDate;

                for (int i = 0; i <= difference; i++)
                {
                    var entry = new TimesheetEntryViewModel();
                    entry.Day = dateIterator.Date;
                    entry.Index = i;

                    var timeentry = dbTimesheetEntries.FirstOrDefault(x => x.Day.Date == entry.Day.Date);
                    if (timeentry != null)
                    {

                        string startString = "";
                        if (timeentry.StartTime.HasValue)
                            startString = timeentry.StartTime.Value.ToString("h:mm:tt");
                        string endString = "";
                        if (timeentry.EndTime.HasValue)
                            endString = timeentry.EndTime.Value.ToString("h:mm:tt");

                        entry.TimesheetEntryId = timeentry.TimesheetEntryId;
                        entry.StartTime = startString;
                        entry.EndTime = endString;
                        entry.IsSkipMeal = timeentry.WorkedThroughMealtime.HasValue
                            ? timeentry.WorkedThroughMealtime.Value
                            : false;
                    }
                    else
                    {
                        entry.TimesheetEntryId = 0;
                        entry.StartTime = "";
                        entry.EndTime = "";
                        entry.IsSkipMeal = false;
                    }

                    var vacationentry = dbVacations.FirstOrDefault(x => x.Day.Date == entry.Day.Date);
                    if (vacationentry != null)
                    {
                        entry.VacationId = vacationentry.VacationId;
                        entry.VacationHours = vacationentry.NumberOfVacationHoursUsed == 0
                            ? string.Empty
                            : vacationentry.NumberOfVacationHoursUsed.ToString();
                        entry.IsVacationSignedOff = vacationentry.VacationSignOffId.HasValue;
                    }

                    dateIterator = dateIterator.AddDays(1.0);
                    viewmodel.TimesheetEntries.Add(entry);
                }

                EmployeeTimesheetsController.CalculateTimesheetDisplayHourTotals(viewmodel);
                try
                {
                    printvm.EmployeeTimesheets.Add(viewmodel);
                }
                catch (Exception ex)
                {
                    throw;
                }                
            }                        
            
            return PartialView("_PrintTemplatePartial", printvm);
        }
        
    }
}