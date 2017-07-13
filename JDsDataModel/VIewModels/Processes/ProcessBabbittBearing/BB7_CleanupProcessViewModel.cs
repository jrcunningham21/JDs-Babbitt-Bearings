using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 7, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB7_CleanupProcessViewModel : StepViewModel
    {
        [DisplayName("Clean Up By:")]
        public string CleanUpBy { get; set; }

        [DisplayName("Slinger Ring Cut Out By:")]
        public string SlingerRingCutOutBy { get; set; }

        public bool IsSlingerRingVisible { get; set; }

        public BB7_CleanupProcessViewModel()
        {
            Version = "1.0";
        }
    }
}
