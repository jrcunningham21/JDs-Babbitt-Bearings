using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.EmployeeTimesheets
{
    public class TimesheetEntryViewModel
    {
        /// <summary>
        /// Because Kendogrid....
        /// </summary>
        public int Index { get; set; }

        [DisplayName("Timesheet Entry:")]
        public int TimesheetEntryId { get; set; }

        [DisplayName("Vaction Entry:")]
        public int VacationId { get; set; }

        [DisplayName("Day:")]
        public DateTime Day { get; set; }

        [DisplayName("Start:")]
        public string StartTime { get; set; }

        [DisplayName("End:")]
        public string EndTime { get; set; }

        [DisplayName("Skip Meal:")]
        public bool IsSkipMeal { get; set; }

        [DisplayName("Signed Off:")]
        public bool IsVacationSignedOff { get; set; }

        [DisplayName("Worked:")]
        public string WorkedHours { get; set; }

        [DisplayName("Vacation:")]
        public string VacationHours { get; set; }

        [DisplayName("Total Hours:")]
        public string TotalHours { get; set; }

        [DisplayName("Straight:")]
        public string StraightTime { get; set; }

        [DisplayName("Overtime:")]
        public string OverTime { get; set; }

        [DisplayName("Double Time:")]
        public string DoubleTime { get; set; }
    }
}