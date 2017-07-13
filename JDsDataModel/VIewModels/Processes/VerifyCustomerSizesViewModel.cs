using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels.Processes
{
    public class VerifyCustomerSizesViewModel
    {
        public int PartID { get; set; }

        public int JobID { get; set; }

        [Display(Name = "Shaft:")]
        public string VerifyShaft { get; set; }

        [Display(Name = "Clearance:")]
        public string VerifyClearance { get; set; }

        [Display(Name = "Bore Size:")]
        public string VerifyBoreSize { get; set; }

        [Display(Name = "Bore Size (horz):")]
        public string VerifyBoreSizeHorizontal { get; set; }

        [Display(Name = "Shim Size:")]
        public string VerifyShimSize { get; set; }

        [Display(Name = "Tolerance:")]
        public string VerifyTolerance { get; set; }

        [Display(Name = "Seal Size:")]
        public string VerifySealSize { get; set; }

        // This is the customer specified OD size
        [Display(Name = "Cust OD Sizes:")]
        public List<PartDiameterMeasurement> VerifyODSizes { get; set; }

        // Below are measured values - from inc inspection and from this page
        [Display(Name = "ID 1 Sizes:")]
        public List<BoreMeasurementViewModel> VerifyIncomingID1Sizes { get; set; }

        [Display(Name = "ID 2 Sizes:")]
        public List<BoreMeasurementViewModel> VerifyIncomingID2Sizes { get; set; }

        [Display(Name = "OD 2 Sizes:")]
        public List<BoreMeasurementViewModel> VerifyIncomingOD1Sizes { get; set; }

        [Display(Name = "OD 2 Sizes:")]
        public List<BoreMeasurementViewModel> VerifyIncomingOD2Sizes { get; set; }

        public bool IsPartInsulated { get; set; }

    }
}
