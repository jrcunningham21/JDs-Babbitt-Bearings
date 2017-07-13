using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace JDsWebApp.Areas.EmployeePortal.Models.EmployeeTimesheets
{
    public class TimesheetViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string StartPayPeriod { get; set; }

        public string Year { get; set; }

        public List<TimesheetEntryViewModel> TimesheetEntries { get; set; }

        public List<SelectListItem> Years { get; set; }

        public List<SelectListItem> Employees { get; set; }

        public List<SelectListItem> PayPeriods { get; set; }

        public int CurrentPayPeriod { get; set; }

        #region Display Summary...

        [DisplayName("Total Hours:")]
        public string TotalHours { get; set; }

        [DisplayName("Straight:")]
        public string TotalStraight { get; set; }

        [DisplayName("Overtime:")]
        public string TotalOvertime { get; set; }

        [DisplayName("Double Time:")]
        public string TotalDoubleTime { get; set; }


        #endregion

    }
}