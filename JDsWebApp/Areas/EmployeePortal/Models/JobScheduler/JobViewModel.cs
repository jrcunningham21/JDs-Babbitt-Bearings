using JDsDataModel;
using JDsDataModel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Areas.EmployeePortal.Models.JobScheduler
{
    public class JobViewModel : Kendo.Mvc.UI.ISchedulerEvent
    {
        public int JobID { get; set; }
        [Required]
        [Display(Name = "Customers")]
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Contacts")]
        public int ContactId { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public int? CustomerContactId { get; set; }
        [Display(Name = "Customer Job #")]
        public string CustomerJobNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        [Display(Name = "Recieved Date")]
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        [Display(Name = "Required Date")]
        public Nullable<System.DateTime> RequiredDate { get; set; }
        [Display(Name = "Estimated Ship Date")]
        public Nullable<System.DateTime> ShipByDate { get; set; }
        [Display(Name = "OT")]
        public Nullable<bool> OvertimeRequired { get; set; }
        public Nullable<bool> HoldForCustomerApproval { get; set; }
        public Nullable<bool> QuoteOnly { get; set; }
        public Nullable<bool> AllPartsRequireUT { get; set; }
        public Nullable<bool> AllPartsRequirePT { get; set; }
        public string Header { get; set; }
        public string JobNotes { get; set; }
        public CustomerViewModel Customer { get; set; }
        public ContactViewModel Contact { get; set; }
        public List<Part> Parts { get; set; }
        public string JobStatus { get; set; }
        public string PartStatus { get; set; }
        public int SortMethod { get; set; }


        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsAllDay { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string StartTimezone { get; set; }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }

        public string RecurrenceException { get; set; }
    }
}
