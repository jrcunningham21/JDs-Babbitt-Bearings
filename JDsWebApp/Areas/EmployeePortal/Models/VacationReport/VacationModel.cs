using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.VacationReport
{
    public class VacationModel
    {
        public string VacationDate { get; set; }
        public string ApprovedBy { get; set; }
        public int NumHours { get; set; }
        public string HourType { get; set; }
    }
}