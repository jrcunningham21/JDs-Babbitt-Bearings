using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class OutgoingFormViewModel
    {
        [DisplayName("Job #:")]
        public int JobId { get; set; }

        [DisplayName("Part:")]
        public int PartId { get; set; }

        [DisplayName("ID:")]
        public List<BoreMeasurementViewModel> ID1Measurements { get; set; }

        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD1Measurements { get; set; }

        [DisplayName("ID:")]
        public List<BoreMeasurementViewModel> ID2Measurements { get; set; }

        [DisplayName("OD:")]
        public List<BoreMeasurementViewModel> OD2Measurements { get; set; }

        [DisplayName("Customer:")]
        public string CustomerName { get; set; }

        [DisplayName("Date:")]
        public string Date { get; set; }

        [DisplayName("Contact:")]
        public string ContactName { get; set; }

        [DisplayName("Customer Job #:")]
        public string CustomerJobNumber { get; set; }

        [DisplayName("PO #:")]
        public string PONumber { get; set; }

        [DisplayName("Bore Diameter:")]
        public string BoreDiameter { get; set; }

        [DisplayName("Babbitt Length:")]
        public decimal BabbittLength { get; set; }

        [DisplayName("Insulated:")]
        public bool Insulated { get; set; }

        [DisplayName("Shaft Size:")]
        public string ShaftSize { get; set; }

        [DisplayName("Bore Size Vertical:")]
        public string BoreSizeVertical { get; set; }

        [DisplayName("Bore Size Horizontal:")]
        public string BoreSizeHorizontal { get; set; }

        [DisplayName("Checked By:")]
        public string CheckedBy { get; set; }

        [DisplayName("Bearing Manufacturer:")]
        public string BearingManufacturer { get; set; }

        [DisplayName("Bearing Type:")]
        public string BearingType { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }

        [DisplayName("Parts Installed:")]
        public List<IncomingInspectionAccessory> PartsInstalled { get; set; }



    }
}
