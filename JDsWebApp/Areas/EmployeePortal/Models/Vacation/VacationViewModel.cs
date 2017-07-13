using JDsWebApp.Areas.EmployeePortal.Models.EmployeePortalLogin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Areas.EmployeePortal.Models.Vacation
{

    public class VacationViewModel : Kendo.Mvc.UI.ISchedulerEvent
    {
        public int VacationEntryId { get; set; }

        public int EmployeeId { get; set; }

        public string EntryTitle { get; set; }

        public int NumHours { get; set; }

        public int? SignOffId { get; set; }

        public bool SignOffRequired { get; set; }

        public bool IsFullVacationDay { get; set; }   

        // Below properties are for the built-in scheduler event and shouldn't be messed with
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsAllDay { get; set; }

        public bool IsVacationTime { get; set; }

        public string SignOffName { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string StartTimezone { get; set; }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }

        public string RecurrenceException { get; set; }
       
    }
}
