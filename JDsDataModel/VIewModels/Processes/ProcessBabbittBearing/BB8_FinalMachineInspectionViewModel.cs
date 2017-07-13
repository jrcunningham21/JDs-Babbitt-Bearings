using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 8, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB8_FinalMachineInspectionViewModel : StepViewModel
    {
        [DisplayName("Split Lines Verified:")]
        public bool IsSplitLinesVerified { get; set; }

        [DisplayName("Dowl Checks Good:")]
        public bool IsDowelChecksGood { get; set; }

        [DisplayName("Bond Verified:")]
        public bool IsBondVerified { get; set; }

        [DisplayName("Clean:")]
        public bool IsClean { get; set; }
        
        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD1Measurements { get; set; }
        
        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD2Measurements { get; set; }

        [DisplayName("Ready for Final Machine By:")]
        public string ReadyForFinalMachineBy { get; set; }

        [DisplayName("Flag for Customer Approval")]
        public bool IsFlaggedForCustomerApproval { get; set; }

        [DisplayName("Problem Resolution:")]
        public string ProblemResolution { get; set; }

        [DisplayName("Sizes Approved By:")]
        public string SizedApprovedBy { get; set; }

        public bool IsPartInsulated { get; set; }   // from BB1

        public BB8_FinalMachineInspectionViewModel()
        {
            Version = "1.0";
            OD1Measurements = new List<BoreMeasurementViewModel>();
            OD2Measurements = new List<BoreMeasurementViewModel>();
        }
    }
    
}
