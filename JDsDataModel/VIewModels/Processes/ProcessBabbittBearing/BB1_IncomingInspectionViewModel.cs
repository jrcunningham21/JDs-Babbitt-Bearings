using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 1, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB1_IncomingInspectionViewModel : StepViewModel
    {
        [DisplayName("Disassembled and Stenciled By:")]
        public string DisassembledStenciledBy { get; set; }

        [DisplayName("Pictures Approved By:")]
        public string PicturesApprovedBy { get; set; }

        [DisplayName("Incoming DWG/Files Approved By:")]
        public string IncomingFilesApprovedBy { get; set; }

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
        [RegularExpression("^[0-9]+(\\.[0-9]+)?$", ErrorMessage = "Please enter a valid number.")]
        public decimal? OverallLength { get; set; }
        
        [DisplayName("Insulated:")]
        public bool IsInsulated { get; set; }

        [DisplayName("Insulation:")]
        public string Insulation { get; set; }
        
        [DisplayName("Measured Incoming Sizes By:")]
        public string MeasuredIncomingSizesBy { get; set; }

        [DisplayName("Parts / Accessories:")]
        public List<IncomingInspectionAccessory> PartsAccessories { get; set; }

        [DisplayName("Material:")]
        public string Material { get; set; }

        [DisplayName("T/C:")]
        public bool IsTC { get; set; }

        [DisplayName("T/C Depth:")]
        public decimal? TCDepth { get; set; }

        [DisplayName("T/C Diameter:")]
        public decimal? TCDiameter { get; set; }

        [DisplayName("T/C Quantity")]
        public int? TCQuantity { get; set; }

        [DisplayName("Part Notes:")]
        public string PartNotes { get; set; }

        [DisplayName("Final Incoming Inspection Approved By:")]
        public string FinalInspectionApprovedBy { get; set; }

        public BB1_IncomingInspectionViewModel()
        {
            Version = "1.0";
            ID1Measurements = new List<BoreMeasurementViewModel>();
            OD1Measurements = new List<BoreMeasurementViewModel>();
            ID2Measurements = new List<BoreMeasurementViewModel>();
            OD2Measurements = new List<BoreMeasurementViewModel>();
            SealMeasurements = new List<BoreMeasurementViewModel>();
            PartsAccessories = new List<IncomingInspectionAccessory>();
        }
    }
}
