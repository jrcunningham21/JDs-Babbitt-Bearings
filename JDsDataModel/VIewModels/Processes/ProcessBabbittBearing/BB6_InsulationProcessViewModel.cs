using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 6, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB6_InsulationProcessViewModel : StepViewModel
    {
        [DisplayName("Insulation Made By:")]
        public string InsulationMadeBy { get; set; }

        [DisplayName("Grit Blasted By:")]
        public string GritBlastedBy { get; set; }

        [DisplayName("Slinger Ring Cut Out By:")]
        public string SlingerRingCutOutBy { get; set; }

        [DisplayName("Insulation Installed By:")]
        public string InsulationInstalledBy { get; set; }

        public BB6_InsulationProcessViewModel()
        {
            Version = "1.0";
        }
    }
}
