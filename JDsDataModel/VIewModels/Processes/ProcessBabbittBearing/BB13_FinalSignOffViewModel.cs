using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    [Serializable]
    public class BB13_FinalSignOffViewModel : StepViewModel
    {
        [DisplayName("Inspector:")]
        public string InspectedBy { get; set; }

        public BB13_FinalSignOffViewModel()
        {
            Version = "1.0";
        }
    }
}
