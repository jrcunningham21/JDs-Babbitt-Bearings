using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels.Processes
{
    public class PartTestViewModel
    {
        public int PartId { get; set; }

        public bool RequiresPT { get; set; }
        public bool RequiresUT { get; set; }

        public bool PTComplete { get; set; }
        public bool UTComplete { get; set; }

        [DisplayName("PT Test Passed:")]
        public bool PTPassed { get; set; }

        [DisplayName("UT Test Passed:")]
        public bool UTPassed { get; set; }

        [DisplayName("Test notes:")]
        public string TestNotes { get; set; }

        // NOTE: I expect the customer will ask to have files directly associated with the 
        // tests. There's no time now to implement it unless he directly requests it. For now
        // these are here and unused.
        public int UTCertFileId { get; set; }
        public int PTCertFileId { get; set; }

        [DisplayName("Approved By:")]
        public string UTSignoffEmployee { get; set; }

        [DisplayName("Approved By:")]
        public string PTSignoffEmployee { get; set; }

        public int ReturnStep { get; set; }

        // UT or PT
        public string TestType { get; set; }

        public int ProcessId { get; set; }
    }
}
