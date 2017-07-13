using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 4, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB4_PostcastCleanupViewModel : StepViewModel
    {
        [DisplayName("Plaster Removed By:")]
        public string PlasterRemovedBy { get; set; }

        [DisplayName("Washed By:")]
        public string WashedBy { get; set; }

        public BB4_PostcastCleanupViewModel()
        {
            Version = "1.0";
        }
    }
}
