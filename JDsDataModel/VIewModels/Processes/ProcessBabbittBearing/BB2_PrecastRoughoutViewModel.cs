using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 2, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB2_PrecastRoughoutViewModel : StepViewModel
    {
        [DisplayName("Roughout By:")]
        public string RoughOutBy { get; set; }

        [DisplayName("Tinned By:")]
        public string TinnedBy { get; set; }

        [DisplayName("Base Material:")]
        public string BaseMaterial { get; set; }

        public BB2_PrecastRoughoutViewModel()
        {
            Version = "1.0";
        }
    }
}
