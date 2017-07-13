using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsDataModel.ViewModels
{
    public class CustomerViewModel
    {
        [DisplayName("Customer")]
        public int CustomerId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is required")]
        public string CompanyName { get; set; }
        
        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Primary Contact")]
        public ContactViewModel PrimaryContact { get; set; }
        
        public int? PrimaryContactId { get; set; }

        [DisplayName("Billing Address")]
        public int? BillingAddressId { get; set; }

        [DisplayName("Address Line 1")]
        public string BillingAddressLine1 { get; set; }

        [DisplayName("Address Line 2")]
        public string BillingAddressLine2 { get; set; }

        [DisplayName("City")]
        public string BillingCity { get; set; }

        [DisplayName("State")]
        public string BillingState { get; set; }

        [DisplayName("Zip")]
        public string BillingZip { get; set; }

        [DisplayName("Country")]
        public string BillingCountry { get; set; }

        [DisplayName("Shipping Address")]
        public int? ShippingAddressId { get; set; }

        [DisplayName("Address Line 1")]
        public string ShippingAddressLine1 { get; set; }

        [DisplayName("Address Line 2")]
        public string ShippingAddressLine2 { get; set; }

        [DisplayName("City")]
        public string ShippingCity { get; set; }

        [DisplayName("State")]
        public string ShippingState { get; set; }

        [DisplayName("Zip")]
        public string ShippingZip { get; set; }

        [DisplayName("Country")]
        public string ShippingCountry { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; }
    }
}
