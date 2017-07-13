using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class JobViewModel
    {
        [Display(Name = "JD's Job #")]
        public int JobID { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Contacts")]
        public int ContactId { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public int? CustomerContactId { get; set;}
        [Display(Name = "Customer Job #")]
        public string CustomerJobNumber { get; set; }
        [Display(Name = "Purchase Order #")]
        public string PurchaseOrderNumber { get; set; }
        [Display(Name = "Recieved Date")]
        [Required(ErrorMessage = "Please enter a valid date")]
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        [Display(Name = "Required Date")]
        [Required(ErrorMessage = "Please enter a valid date")]
        public Nullable<System.DateTime> RequiredDate { get; set; }
        [Display(Name = "Estimated Ship Date")]
        [Required (ErrorMessage = "Please enter a valid date")]
        public Nullable<System.DateTime> ShipByDate { get; set; }
        [Display(Name = "Required Time Type")]
        public Nullable<bool> OvertimeRequired { get; set; }
        [Display(Name = "Hold for Customer Approval")]
        public Nullable<bool> HoldForCustomerApproval { get; set; }
        [Display(Name = "Quote Only")]
        public Nullable<bool> QuoteOnly { get; set; }
        [Display(Name = "All Parts Require PT")]
        public Nullable<bool> AllPartsRequirePT { get; set; }
        [Display(Name = "All Parts Require UT")]
        public Nullable<bool> AllPartsRequireUT { get; set; }
        public string Header { get; set; }
        [Display(Name = "Job Notes")]
        public string JobNotes { get; set; }
        public CustomerViewModel Customer { get; set; }
        public ContactViewModel Contact { get; set; }
        public List<Part> Parts { get; set; }
        public string JobStatus { get; set; }
        public string PartStatus { get; set; }
        public string CompanyName { get; set; }
        public int SortMethod { get; set; }
        public string FinalPrint { get; set; }
        public int? JobStatusId { get; set; }


        // nullable bools coerced to bool

        [Display(Name = "Required Time Type")]
        public bool CoercedOvertimeRequired { get; set; }
        [Display(Name = "Hold for Customer Approval")]
        public bool CoercedHoldForCustomerApproval { get; set; }
        [Display(Name = "Quote Only")]
        public bool CoercedQuoteOnly { get; set; }
        [Display(Name = "All Parts Require PT")]
        public bool CoercedAllPartsRequirePT { get; set; }
        [Display(Name = "All Parts Require UT")]
        public bool CoercedAllPartsRequireUT { get; set; }

        public bool IsNewJob { get; set; }
    }

    public class BigBoardJobViewModel
    {
        public int JobId { get; set; }

        public string Header { get; set; }

        public string DisplayLine1 { get; set; }

        public string DisplayLine2 { get; set; }

        public string DisplayLine3 { get; set; }

        public int JobStatusId { get; set; }

        public string JobStatus { get; set; }

        public int SortMethod { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string CompanyName { get; set; }
        public bool QuoteOnly { get; set; }
    }

    public class BigBoardJobViewModelTest
    {
        public int SortMethod { get; set; }

        public List<IGrouping<string,JobBoardViewModel>> JobDateGrouping { get; set; }
    }

    public class JobBoardViewModel
    {
        public int JobID { get; set; }

        public string CompanyName { get; set; }

        public DateTime ShipByDate { get; set; }

        public string JobStatus { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ReceivedDate { get; set; }

        public List<string> PartNames { get; set; } 

        public int SortMethod { get; set; }

        public bool QuoteOnly { get; set; }
    }

    public class RightBigBoardViewModel
    {
        public int JobID { get; set; }

        public List<PartViewModel> PartsViews { get; set; } 
    }
}
