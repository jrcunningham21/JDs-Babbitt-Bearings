using System;
using System.ComponentModel;

namespace JDsWebApp.Areas.EmployeePortal.Models.Billing
{
    public class BillingJobViewModel
    {
        [DisplayName("JDs Job#:")]
        public int JobId { get; set; }
        
        [DisplayName("Customer / PO#:")]
        public string CustomerNameAndJobNumber { get; set; }
        
        //[DisplayName("Required:")]
        //public DateTime? RequiredDate { get; set; }

        [DisplayName("Shipped:")]
        public string ShippedDate { get; set; }

        [DisplayName("Billed:")]
        public string BilledDate { get; set; }

        //[DisplayName("Status:")]
        //public int JobStatusId { get; set; }
    }
}
