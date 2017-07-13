using System;
using System.ComponentModel;

namespace JDsDataModel.ViewModels.Processes
{
    [Serializable]
    public class DeliveryStepViewModel : StepViewModel
    {
        [DisplayName("Packed By:")]
        public string PackedBy { get; set; }

        [DisplayName("Shipped Via:")]
        public string ShippedVia { get; set; }

        [DisplayName("Date Shipped:")]
        public DateTime? ShipDate { get; set; }

        [DisplayName("Date Required:")]
        public DateTime? RequiredDate { get; set; }

        [DisplayName("Tracking Number:")]
        public string TrackingNumber { get; set; }
    }
}
