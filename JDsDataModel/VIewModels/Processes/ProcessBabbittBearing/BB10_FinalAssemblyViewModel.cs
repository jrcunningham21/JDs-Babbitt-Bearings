using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 10, SEE UI DISPLAY CONDITIONS....    
    /// </summary>
    [Serializable]
    public class BB10_FinalAssemblyViewModel : StepViewModel
    {
        [DisplayName("Mill Work Done By:")]
        public string MillWorkBy { get; set; }

        [DisplayName("Deburred By:")]
        public string DeburredBy { get; set; }

        [DisplayName("Parts / Accessories:")]
        public List<IncomingInspectionAccessory> PartsAccessories { get; set; }

        [DisplayName("Parts/Accessories Installation By:")]
        public string PartsInstalledBy { get; set; }

        [DisplayName("All Holes Checked and Verified Clean By:")]
        public string VerifiedBy { get; set; }

        public BB10_FinalAssemblyViewModel()
        {
            Version = "1.0";
            PartsAccessories = new List<IncomingInspectionAccessory>();
        }
    }
}
