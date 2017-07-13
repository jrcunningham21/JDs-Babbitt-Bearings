using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.VacationReport
{
    public class VacationReportModel
    {
        public string StartDate { get; set; }
        public int TotalAccumulatedVacationHours { get; set; }
        public int UsedVacationHours { get; set; }
        public int RemainingVacationHours { get; set; }
        public int TotalAccumulatedPersonalHours { get; set; }
        public int UsedPersonalHours { get; set; }
        public int RemainingPersonalHours { get; set; }
        public List<VacationModel> Vacations { get; set; }
    }
}