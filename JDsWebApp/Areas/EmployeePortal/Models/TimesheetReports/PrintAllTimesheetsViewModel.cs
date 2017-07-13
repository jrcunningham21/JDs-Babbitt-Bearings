using JDsWebApp.Areas.EmployeePortal.Models.EmployeeTimesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JDsWebApp.Areas.EmployeePortal.Models.TimesheetReports
{
    public class PrintAllTimesheetsViewModel
    {      
        public string PayPeriod { get; set; }
        public List<TimesheetViewModel> EmployeeTimesheets { get; set; }
    }
}