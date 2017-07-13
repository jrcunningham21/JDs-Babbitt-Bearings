using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class SizesViewModel
    {
        //public SizesViewModel()
        //{
        //    ODSizes = new List<decimal>();
        //    ODSizes.Add(new decimal());
        //}

        public int SizesID { get; set; }
        public int PartID { get; set; }

        [Display (Name = "Shaft:")]
        public string Shaft { get; set; }

        [Display (Name = "Clearance:")]
        public string Clearance { get; set; }

        [Display (Name = "Bore Size:")]
        public string BoreSize { get; set; }

        [Display (Name = "Bore Size Horizontal:")]
        public string BoreSizeHorizontal { get; set; }

        [Display (Name = "Shim Size:")]
        public string ShimSize { get; set; }

        [Display (Name = "Tolerance:")]
        public string Tolerance { get; set; }

        [Display (Name = "Seal Size:")]
        public string SealSize { get; set; }

        [Display (Name = "OD Sizes:")]
        [DisplayName ("OD Sizes:")]
        public List<PartDiameterMeasurement> ODSizes { get; set; }

        [Display (Name = "Special Notes:")]
        public string SpecialNotes { get; set; }
    }
}
