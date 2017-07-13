using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    [Serializable]
    public class BoreMeasurementViewModel
    {
        // Hack for Kendo grid
        public int Index { get; set; }

        [DisplayName("Measurement:")]
        public int MeasurementId { get; set; }

        [DisplayName("Group:")]
        public int? MeasurementGroupId { get; set; }

        [DisplayName("A:")]
        [Range(-99999.99999, 99999.99999, ErrorMessage = "Please enter a value between -99999.99999 and 99999.99999")]        
        public decimal? A { get; set; }
        [Range(-99999.99999, 99999.99999, ErrorMessage = "Please enter a value between -99999.99999 and 99999.99999")]
        [DisplayName("B:")]
        public decimal? B { get; set; }

        [DisplayName("C:")]
        [Range(-99999.99999, 99999.99999, ErrorMessage = "Please enter a value between -99999.99999 and 99999.99999")]
        public decimal? C { get; set; }
    }
}
