using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 5, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB5_PostcastRoughoutViewModel : StepViewModel
    {
        [DisplayName("Incoming ID:")]
        public List<BoreMeasurementViewModel> IID1Measurements { get; set; }        

        [DisplayName("Incoming ID:")]
        public List<BoreMeasurementViewModel> IID2Measurements { get; set; }

        [DisplayName("Incoming OD:")]
        public List<BoreMeasurementViewModel> IOD1Measurements { get; set; }

        [DisplayName("Incoming OD:")]
        public List<BoreMeasurementViewModel> IOD2Measurements { get; set; }

        [DisplayName("Sizes Verified:")]
        public bool IsCustomerSizesVerified { get; set; }

        [DisplayName("OD Info:")]
        public string ODInfo { get; set; }

        [DisplayName("Bore:")]
        public string CustIdBore { get; set; }

        [DisplayName("Bore (horiz):")]
        public string CustIdBoreHoriz { get; set; }

        [DisplayName("UT:")]
        public bool IsUT { get; set; }

        [DisplayName("PT:")]
        public bool IsPT { get; set; }

        [DisplayName("Roughed Out By:")]
        public string RoughedOutBy { get; set; }

        public BB5_PostcastRoughoutViewModel()
        {
            Version = "1.0";
            IID1Measurements = new List<BoreMeasurementViewModel>();
            IID2Measurements = new List<BoreMeasurementViewModel>();
            IOD1Measurements = new List<BoreMeasurementViewModel>();
            IOD2Measurements = new List<BoreMeasurementViewModel>();
        }
    }
}
