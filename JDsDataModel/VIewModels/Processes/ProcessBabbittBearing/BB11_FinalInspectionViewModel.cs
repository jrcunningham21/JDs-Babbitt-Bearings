using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 11, , SEE UI DISPLAY CONDITIONS....   
    /// </summary>
    [Serializable]
    public class BB11_FinalInspectionViewModel : StepViewModel
    {
        [DisplayName("ID:")]
        public List<BoreMeasurementViewModel> ID1Measurements { get; set; }

        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD1Measurements { get; set; }

        [DisplayName("ID:")]
        public List<BoreMeasurementViewModel> ID2Measurements { get; set; }

        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD2Measurements { get; set; }

        [DisplayName("Seal:")]
        public List<BoreMeasurementViewModel> SealMeasurements { get; set; }

        [DisplayName("AR:")]
        public bool HasARPin { get; set; }

        [DisplayName("Diameter:")]
        public decimal? ARPinDiameter { get; set; }

        [DisplayName("Depth:")]
        public decimal? ARPinDepth { get; set; }

        [DisplayName("Quantity:")]
        public int Quantity { get; set; }

        [DisplayName("Overall Length:")]
        public decimal? OverallLength { get; set; }

        public bool HasTC { get; set; }

        [DisplayName("T/C Installed Quantity:")]
        public int? TCInstalledQuantity { get; set; }

        [DisplayName("Acceptable:")]
        public bool IsAcceptable { get; set; }

        [DisplayName("Call Customer:")]
        public bool IsCustomerCalled { get; set; }

        [DisplayName("Final Inspection By:")]
        public string FinalInspectionBy { get; set; }

        [DisplayName("Return to Step:")]
        public long? ReturnStepId { get; set; }

        [DisplayName("Fail Job")]
        public bool IsJobFailed { get; set; }

        // Pulled from BB1
        public bool IsInsulated { get; set; }

        [DisplayName("Rework Reason:")]
        public string Notes { get; set; }

        public BB11_FinalInspectionViewModel()
        {
            Version = "1.0";
            ID1Measurements = new List<BoreMeasurementViewModel>();
            OD1Measurements = new List<BoreMeasurementViewModel>();
            ID2Measurements = new List<BoreMeasurementViewModel>();
            OD2Measurements = new List<BoreMeasurementViewModel>();
            SealMeasurements = new List<BoreMeasurementViewModel>();
        }
    }
}
