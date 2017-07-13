using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeTimesheets;
using NLog;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class EmployeeTimesheetsController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        // GET: EmployeePortal/EmployeePortalTimesheets
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Timesheets()
        {
            TimesheetViewModel model = new TimesheetViewModel();
            model.Years = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;
            model.EmployeeId = int.Parse(this.HttpContext.Request.Cookies["EmployeeID"]?.Value ?? "0");
            model.Years.Add(new SelectListItem() {Text = currentYear.ToString(), Value = currentYear.ToString()});
            model.Years.Add(new SelectListItem() { Text = (currentYear - 1).ToString(), Value = (currentYear - 1).ToString() });
            model.Years.Add(new SelectListItem() { Text = (currentYear - 2).ToString(), Value = (currentYear - 2).ToString() });
            model.Employees = _db.Employees.Where(x => x.IsActive == true).OrderBy(x => x.Name)
                .Select(x => new SelectListItem() {Text = x.Name, Value = x.EmployeeId.ToString()}).ToList();
            model.CurrentPayPeriod = (int)Math.Floor(DateTime.Now.DayOfYear * 24.0/365.00);

            return View(model);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult LoadTimesheetForEmployee(int? EmployeeId, string PayPeriod, string Year)
        {
            return View("Timesheets");
        }
        
        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetEmployeeTimesheet(string Year, string Date, string EmployeeID)
        {
            int startMonth, startDay, year;

            string[] startSplit = Date.Split('/');
            startMonth = Int32.Parse(startSplit[0]);
            startDay = Int32.Parse(startSplit[1]);

            Int32.TryParse(Year, out year);

            DateTime sd = new DateTime(year, startMonth, startDay);

            return View();
        }

        [HttpGet]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetLatestPayPeriods()
        {
            DateTime endIndex = DateTime.Now;
            DateTime startIndex = DateTime.Now.AddDays(-200.0);

            // set the start day to the first
            if (startIndex.Day != Properties.Settings.Default.StartDayMonth1stPayPeriod)
            {
                var diff = DateTime.DaysInMonth(startIndex.Year, startIndex.Month) - startIndex.Day + 1;
                startIndex = startIndex.AddDays(diff);
            }

            _logger.Info("Start Day: {0}", startIndex.Day);

            if (endIndex.Day > Properties.Settings.Default.StartDayMonth2ndPayPeriod &&
                endIndex.Day < DateTime.DaysInMonth(endIndex.Year, endIndex.Month))
            {
                endIndex = endIndex.AddDays(DateTime.DaysInMonth(endIndex.Year, endIndex.Month) - endIndex.Day);
            }
            else if (endIndex.Day != Properties.Settings.Default.StartDayMonth1stPayPeriod)
            {
                endIndex = endIndex.AddDays(Properties.Settings.Default.StartDayMonth2ndPayPeriod - 1 - endIndex.Day);
            }

            var payperiods = new List<object>();

            do
            {
                if (startIndex.Day == Properties.Settings.Default.StartDayMonth1stPayPeriod)
                {
                    var p = string.Format("{0}  -  {1}",
                        startIndex.ToString("dd MMM"),
                        startIndex.AddDays(Properties.Settings.Default.StartDayMonth2ndPayPeriod - 2)
                            .ToString("dd MMM yyyy"));

                    payperiods.Add(new
                    {
                        StartDate = startIndex.Date.ToString(),
                        Period = p,
                    });

                    startIndex = startIndex.AddDays(Properties.Settings.Default.StartDayMonth2ndPayPeriod - 1);
                }

                if (DateTime.Compare(startIndex, endIndex) < 0 &&
                    startIndex.Day == Properties.Settings.Default.StartDayMonth2ndPayPeriod)
                {
                    var p = string.Format("{0}  -  {1}",
                        startIndex.ToString("dd MMM"),
                        startIndex.AddDays(DateTime.DaysInMonth(startIndex.Year, startIndex.Month) - startIndex.Day)
                            .ToString("dd MMM yyyy"));

                    payperiods.Add(new
                    {
                        StartDate = startIndex.Date.ToString(),
                        Period = p,
                    });

                    startIndex =
                        startIndex.AddDays(DateTime.DaysInMonth(startIndex.Year, startIndex.Month) - startIndex.Day + 1);
                }
            } while (DateTime.Compare(startIndex, endIndex) < 0);

            payperiods.Reverse();
            return Json(payperiods.Take(20), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TimesheetEntries(int employeeId, string payperiod, string Year)
        {
            using (var db = new JDsDBEntities())
            {
                // week should be 1 or 2
                var employee = db.Employees.Find(employeeId);
                if (employee == null)
                {
                    _logger.Error("Unable to retrieve timesheet details for EmployeeId={0}.", employeeId);
                    return PartialView("_TimesheetEntries", null);
                }

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
                PayPeriodEndDate = PayPeriodEndDate.AddDays(1.0);  // less than the AM of the following day 12AM.

                var viewmodel = new TimesheetViewModel
                {
                    EmployeeId = employeeId,
                    StartPayPeriod = PayPeriodStartDate.ToString(),
                    TimesheetEntries = new List<TimesheetEntryViewModel>()
                };

                var dbTimesheetEntries = db.TimesheetEntries.Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Day >= PayPeriodStartDate &&
                    x.Day < PayPeriodEndDate).ToList();

                var dbVacations = db.Vacations.Where(
                    x => x.EmployeeId == employeeId &&
                         x.Day >= PayPeriodStartDate &&
                         x.Day < PayPeriodEndDate).ToList();

                var difference = (PayPeriodEndDate - PayPeriodStartDate).Days;
                DateTime dateIterator = PayPeriodStartDate;

                for (int i = 0; i < difference; i++)
                {
                    var entry = new TimesheetEntryViewModel();
                    entry.Day = dateIterator.Date;
                    entry.Index = i;

                    var timeentry = dbTimesheetEntries.FirstOrDefault(x => x.Day.Date == entry.Day.Date);
                    if (timeentry != null)
                    {
                        entry.TimesheetEntryId = timeentry.TimesheetEntryId;
                        if (timeentry.StartTime.HasValue)
                            entry.StartTime = timeentry.StartTime.Value.ToString("h:mm:tt");
                        else
                            entry.StartTime = "";
                        if (timeentry.EndTime.HasValue)
                            entry.EndTime = timeentry.EndTime.Value.ToString("h:mm:tt");
                        else
                            entry.EndTime = "";

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


                CalculateTimesheetDisplayHourTotals(viewmodel);
                return PartialView("_TimesheetEntries", viewmodel);
            }
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult CalcTimesheetTotals(TimesheetViewModel model)
        {

            if (model == null || model.TimesheetEntries == null)
            {
                ModelState.AddModelError("Error", "The timesheet or entries model is null");
                return PartialView("_TimesheetEntries", model);
            }

            CalculateTimesheetDisplayHourTotals(model);
            
            return PartialView("_TimesheetEntries", model);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SaveTimesheet(TimesheetViewModel model)
        {
            // Server side validation
            if (!ModelState.IsValid)
            {
                return PartialView("_TimesheetEntries", model);
            }

            if (model?.TimesheetEntries == null)
            {
                ModelState.AddModelError("Error", "The timesheet or entries model is null");
                return PartialView("_TimesheetEntries", model);
            }

            // Gets the value of DatePicker and splits the dates into 2
            var dateRange = model.StartPayPeriod;
            var splitResult = dateRange.Split('-');

            // Convert payperiod & Year to DateTime
            int startMonth, startDay, endMonth, endDay, year;

            string[] startSplit = splitResult[0].Split('/');
            startMonth = Int32.Parse(startSplit[0]);
            startDay = Int32.Parse(startSplit[1]);

            string[] endSplit = splitResult[1].Split('/');
            endMonth = Int32.Parse(endSplit[0]);
            endDay = Int32.Parse(endSplit[1]);

            Int32.TryParse(model.Year, out year);

            // Make string into datetime
            DateTime PayPeriodStartDate = new DateTime(year, startMonth, startDay);
            DateTime PayPeriodEndDate = new DateTime(year, endMonth, endDay);

            CalculateTimesheetDisplayHourTotals(model);

            using (var db = new JDsDBEntities())
            {
                var employee = db.Employees.Find(model.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError("Critical Error", "The employee for this timesheet could not be found");
                    return PartialView("_TimesheetEntries", model);
                }

                // save or update the timesheet entries
                foreach (var item in model.TimesheetEntries)
                {
                    bool shouldAddNewEntry = false;

                    // first look by id
                    var entry =  db.TimesheetEntries.FirstOrDefault(x => x.TimesheetEntryId == item.TimesheetEntryId);

                    // it might be 0 still, so look up by emp / date
                    if (entry == null)
                    {
                        entry = db.TimesheetEntries.FirstOrDefault(x => x.EmployeeId == model.EmployeeId &&
                                                                         x.Day.Year == item.Day.Year &&
                                                                         x.Day.Month == item.Day.Month &&
                                                                         x.Day.Day == item.Day.Day);
                    }

                    if (entry == null)
                    {
                        entry = new TimesheetEntry();
                        shouldAddNewEntry = true;
                    }

                    entry.EmployeeId = model.EmployeeId;

                    entry.Day = item.Day;

                    entry.StartTime = ParseDateFromString(item.StartTime, item.Day);
                    entry.EndTime = ParseDateFromString(item.EndTime, item.Day);
                    
                    entry.WorkedThroughMealtime = item.IsSkipMeal;

                    // Calculate the number of hours worked, if possible
                    if (entry.StartTime.HasValue && entry.EndTime.HasValue)
                    {
                        var diff = entry.EndTime.Value - entry.StartTime.Value;
                        if (entry.WorkedThroughMealtime.HasValue && entry.WorkedThroughMealtime.Value == false)
                            diff = diff.Add(new TimeSpan(0, -30, 0));
                        entry.NumHoursWorked = (decimal)diff.TotalHours;
                    }
                    else
                        entry.NumHoursWorked = 0;

                    if (shouldAddNewEntry)
                    {
                        try
                        {
                            db.TimesheetEntries.Add(entry);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        var ts = db.Entry<TimesheetEntry>(entry);
                        ts.State = EntityState.Modified;
                    }
                }

                if (db.ChangeTracker.HasChanges())
                {
                    db.SaveChanges();
                }
            }

            // Regenerate the view models to include the natural keys.
            return TimesheetEntries(model.EmployeeId, model.StartPayPeriod, model.Year);
        }

        private static DateTime? ParseDateFromString(string s, DateTime entry)
        {
            DateTime? val;
            if (string.IsNullOrEmpty(s) || s == "::")
                return null;

            var toks = s.Split(new char[] { ':' });
            bool isPm = false;
            isPm = toks[2].ToLower() == "pm";
            int hourVal = string.IsNullOrEmpty(toks[0]) ? 0 : int.Parse(toks[0]);       // default to 0 for unspecified hours
            if (isPm && hourVal != 12)  // special case: don't mod noon hours
                hourVal += 12;
            // special case: midnight is 0
            if (!isPm && toks[0] == "12") hourVal = 0;
            int minVal = string.IsNullOrEmpty(toks[1]) ? 0 : int.Parse(toks[1]);    // default to 0 for unspecified minutes

            val = new DateTime(entry.Year, entry.Month, entry.Day, hourVal, minVal, 0);

            return val;
        }

        public static void CalculateTimesheetDisplayHourTotals(TimesheetViewModel model)
        {
            using (var db = new JDsDBEntities())
            {
                if (model == null) return;

                if ((model.TimesheetEntries == null) || (model.TimesheetEntries.Count == 0))
                {
                    model.TotalHours = string.Empty;
                    model.TotalStraight = string.Empty;
                    model.TotalOvertime = string.Empty;
                    model.TotalDoubleTime = string.Empty;
                    return;
                }

                // Weeks run Sunday->Saturday.  There may be some timesheet entries outside of this pay period range that will
                // affect how this pay period gets paid. Imagine September 2016 pay period 1st to the 15th.  September 1 is a Thursday. Sunday, August 28, is the start of the
                // week.  We need to take into account the hours worked from 28 Aug - 31 Aug to determine if the hours worked on 1 Sept - 3 Sept are overtime or straight time.

                // Find the Sunday closest to the start of the pay period
                DateTime startingSunday = FindClosestSunday(model.TimesheetEntries.First().Day);
                DateTime dayBeforePayPeriod = model.TimesheetEntries.First().Day.Subtract(new TimeSpan(1, 0, 0, 0));

                DateTime periodBegin = model.TimesheetEntries.First().Day;
                DateTime periodEnd = model.TimesheetEntries.Last().Day;

                // Total up the hours between the starting Sunday and the day before the pay period starts
                var previousEntries = db.TimesheetEntries.Where(x => x.EmployeeId == model.EmployeeId && 
                                                                x.Day >= startingSunday && 
                                                                x.Day <= dayBeforePayPeriod && 
                                                                x.StartTime.HasValue && 
                                                                x.EndTime.HasValue &&
                                                                x.WorkedThroughMealtime.HasValue);

                double previousTimeHours = 0.0;
                foreach (var prevEntry in previousEntries)
                {
                    var diff = prevEntry.EndTime.Value - prevEntry.StartTime.Value;
                    if (!prevEntry.WorkedThroughMealtime.Value) // if they didn't skip lunch, subtract 30 mins mealtime
                        diff = diff.Subtract(new TimeSpan(0, 30, 0));
                    previousTimeHours += diff.TotalHours;
                }

                double totalHours = 0;
                double totalDoubleTime = 0;
                double totalStraightTime = 0;
                double totalOverTime = 0;
                double hoursSoFarThisCalendarWeek = previousTimeHours;
                int availableSTHours = DaysRemainingInWeek(periodBegin, periodEnd) * 8;

                // Now go through each entry and allocate the hours accordingly
                foreach (var entry in model.TimesheetEntries)
                {
                    // If the entry is for a Sunday, reset the weekly hour counter
                    if (entry.Day.ToString("D").Contains("Sunday"))
                    {
                        hoursSoFarThisCalendarWeek = 0;
                        availableSTHours = DaysRemainingInWeek(entry.Day, periodEnd) * 8;
                    }

                    // Calculate the number of hours represented by this time entry
                    double hoursForEntry = CalculateHoursForEntry(entry);
                    entry.WorkedHours = String.Format("{0:0.##}", hoursForEntry);

                    // If it's a holiday, or a Sunday, add it there
                    if (IsDateAHolidayOrASunday(entry.Day))
                    {
                        totalDoubleTime += hoursForEntry;
                        hoursSoFarThisCalendarWeek -= hoursForEntry;    // at the end of the loop, we'll incorrectly incremement this variable. Decrementing here adjusts for that
                    }
                    // If 100% of this entry's hours go to OT, add it here
                    else if (hoursSoFarThisCalendarWeek >= availableSTHours)
                    {
                        totalOverTime += hoursForEntry;
                    }
                    // add to straight time. Maybe split the entry between straight and OT
                    else
                    {
                        if (hoursSoFarThisCalendarWeek < availableSTHours && hoursSoFarThisCalendarWeek + hoursForEntry >= availableSTHours)
                        {
                            // then we have to split the difference between OT and straight
                            double hoursAllocatedToOverTime = hoursSoFarThisCalendarWeek + hoursForEntry - availableSTHours;
                            totalOverTime += hoursAllocatedToOverTime;
                            totalStraightTime += hoursForEntry - hoursAllocatedToOverTime;
                        }
                        else
                        {
                            totalStraightTime += hoursForEntry;
                        }
                    }

                    // Always add to the total and to the hours so far this week
                    totalHours += hoursForEntry;
                    hoursSoFarThisCalendarWeek += hoursForEntry;

                }

                model.TotalHours = $"{totalHours:0.###}";
                model.TotalDoubleTime = $"{totalDoubleTime:0.###}";
                model.TotalStraight = $"{totalStraightTime:0.###}";
                model.TotalOvertime = $"{totalOverTime:0.###}";
            }
        }

        private static int DaysRemainingInWeek(DateTime currentDay, DateTime periodEnd)
        {
            return Math.Min(5, periodEnd.Day - currentDay.Day);
        }

        private static bool IsDateAHolidayOrASunday(DateTime targetDay)
        {
            var year = targetDay.Year;

            DateTime newYearsDay = new DateTime(year, 1, 1);

            DateTime memorialDay = new DateTime(year, 5, 31);
            DayOfWeek dayOfWeek = memorialDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                memorialDay = memorialDay.AddDays(-1);
                dayOfWeek = memorialDay.DayOfWeek;
            }

            DateTime fourthOfJuly = new DateTime(year, 7, 4);

            DateTime laborDay = new DateTime(year, 9, 1);
            dayOfWeek = laborDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                laborDay = laborDay.AddDays(1);
                dayOfWeek = laborDay.DayOfWeek;
            }

            var thanksgiving = (from day in Enumerable.Range(1, 30)
                                where new DateTime(year, 11, day).DayOfWeek == DayOfWeek.Thursday
                                select day).ElementAt(3);
            DateTime thanksgivivngDay = new DateTime(year, 11, thanksgiving);

            DateTime christmasDay = new DateTime(year, 12, 25);

            if (targetDay.DayOfWeek == DayOfWeek.Sunday ||
                targetDay.Date == newYearsDay ||
                targetDay.Date == memorialDay ||
                targetDay.Date == fourthOfJuly ||
                targetDay.Date == laborDay ||
                targetDay.Date == thanksgivivngDay ||
                targetDay.Date == christmasDay)
            {
                return true;
            }
            return false;
        }

        private static double CalculateHoursForEntry(TimesheetEntryViewModel entry)
        {
            try
            {
                // the time is in the format: h:mm:tt, or 1:45:PM for 1:45 PM
                var endTokens = entry.EndTime.Split(new char[] { ':' });
                var startTokens = entry.StartTime.Split(new char[] { ':' });
                int startHourMod = startTokens[2].ToLower() == "pm" ? 12 : 0;
                int endHourMod = endTokens[2].ToLower() == "pm" ? 12 : 0;

                // special case: midnight is 0 hours but comes in as 12
                if (startTokens[0] == "12" && startTokens[2].ToLower() == "am") startHourMod = -12;
                if (endTokens[0] == "12" && endTokens[2].ToLower() == "am") endHourMod = -12;

                // special case: midnight is 12, so don't adjust the hours
                if (startTokens[0] == "12" && startTokens[2].ToLower() == "pm") startHourMod = 0;
                if (endTokens[0] == "12" && endTokens[2].ToLower() == "pm") endHourMod = 0;

                DateTime start = new DateTime(entry.Day.Year, entry.Day.Month, entry.Day.Day, int.Parse(startTokens[0]) + startHourMod, int.Parse(startTokens[1]), 0);
                DateTime end = new DateTime(entry.Day.Year, entry.Day.Month, entry.Day.Day, int.Parse(endTokens[0]) + endHourMod, int.Parse(endTokens[1]), 0);
                var span = end - start;

                // If you failed to skip a meal (in other words, if you ate lunch) subtract 30 minutes
                if (!entry.IsSkipMeal)
                {
                    span = span.Subtract(new TimeSpan(0, 30, 0));
                }

                return span.TotalHours;
            }
            catch (Exception)
            {
                // There was a problem in parsing the string times
                return 0;
            }
        }

        private static DateTime FindClosestSunday(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            return date;
        }
    }
}