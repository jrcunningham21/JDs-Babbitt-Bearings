using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    /// <summary>
    /// Step 3, SEE UI DISPLAY CONDITIONS....
    /// </summary>
    [Serializable]
    public class BB3_SpincastProcessViewModel : StepViewModel
    {
        [DisplayName("Base Material:")]
        public string BaseMaterial { get; set; }

        [DisplayName("Deburred By:")]
        public string DeburredBy { get; set; }

        [DisplayName("Tinned By:")]
        public string TinnedBy { get; set; }

        [DisplayName("Plastered By:")]
        public string PlasteredBy { get; set; }

        [DisplayName("RPM:")]
        public int? RPM { get; set; }

        [DisplayName("RPM By:")]
        public string RPMBy { get; set; }

        [DisplayName("Babbit Temp:")]
        public decimal? BabbitTemp { get; set; }

        [DisplayName("Babbit Temp By:")]
        public string BabbitTempBy { get; set; }

        [DisplayName("Plate Temp:")]
        public decimal? PlateTemp { get; set; }

        [DisplayName("Plate Temp By:")]
        public string PlateTempBy { get; set; }

        [DisplayName("Scurbbed for Bonding By:")]
        public string ScrubbedBy { get; set; }

        [DisplayName("Spuncast By:")]
        public string SpuncastBy { get; set; }

        [DisplayName("Cut Apart By:")]
        public string CutApartBy { get; set; }
        
        public bool IsTinnedByVisible { get; set; }

        public BB3_SpincastProcessViewModel()
        {
            Version = "1.0";
        }
    }
}
