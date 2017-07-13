using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 9
    /// </summary>
    [Serializable]
    public class BB9_FinishBoreProcessViewModel : StepViewModel
    {
        [DisplayName("Shaft:")]
        public string ShaftSize { get; set; }

        [DisplayName("Clearance:")]
        public string ClearanceSize { get; set; }

        [DisplayName("Shim Size:")]
        public string ShimSize { get; set; }

        [DisplayName("Tolerance:")]
        public string Tolerance { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }

        [DisplayName("Bore Size:")]
        public string CustomerBoreSize { get; set; }

        [DisplayName("Bore Size (horizontal):")]
        public string CustomerBoreSizeHorizontal { get; set; }

        [DisplayName("OD Sizes:")]
        public List<PartDiameterMeasurement> CustomerODSize { get; set; }

        [DisplayName("Front:")]
        public decimal? Runout1FrontSize { get; set; }

        [DisplayName("Back:")]
        [DisplayFormat(DataFormatString = "{0:0.000#}", ApplyFormatInEditMode = true)]
        public decimal? Runout1BackSize { get; set; }

        [DisplayName("Middle:")]
        [DisplayFormat(DataFormatString = "{0:0.000#}", ApplyFormatInEditMode = true)]
        public decimal? Runout1MiddleSize { get; set; }

        [DisplayName("Face:")]
        [DisplayFormat(DataFormatString = "{0:0.000#}", ApplyFormatInEditMode = true)]
        public decimal? Runout2FaceSize { get; set; }

        [DisplayName("Bore:")]
        [DisplayFormat(DataFormatString = "{0:0.000#}", ApplyFormatInEditMode = true)]
        public decimal? Runout2BoreSize { get; set; }

        [DisplayName("Finish Bore By:")]
        public string FinishedBoreBy { get; set; }
        
        public bool? IsVerifiedSizesOk { get; set; }

        public BB9_FinishBoreProcessViewModel()
        {
            Version = "1.0";
            CustomerODSize = new List<PartDiameterMeasurement>();
        }

    }
}
