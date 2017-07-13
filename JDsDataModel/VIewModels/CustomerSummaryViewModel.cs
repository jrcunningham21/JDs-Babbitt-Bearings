using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsDataModel.ViewModels
{
    public class CustomerSummaryViewModel
    {
        [DisplayName("Customer:")]
        public int CustomerId { get; set; }

        [DisplayName("Company:")]
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        [DisplayName("Code:")]
        public string CompanyCode { get; set; }

        [DisplayName("Billing Address:")]
        public int? BillingAddressId { get; set; }

        [DisplayName("Address Line 1:")]
        public string BillingAddressLine1 { get; set; }

        [DisplayName("Address Line 2:")]
        public string BillingAddressLine2 { get; set; }

        [DisplayName("City:")]
        public string BillingCity { get; set; }

        [DisplayName("State:")]
        public string BillingState { get; set; }

        [DisplayName("Zip:")]
        public string BillingZip { get; set; }

        [DisplayName("Country:")]
        public string BillingCountry { get; set; }

        [DisplayName("Contact:")]
        public int ContactId { get; set; }

        [DisplayName("First Name:")]
        public string ContactFirstName { get; set; }

        [DisplayName("Last Name:")]
        public string ContactLastName { get; set; }

        [DisplayName("Email:")]
        public string ContactEmail { get; set; }

        [DisplayName("Work Phone:")]
        public string ContactWorkPhone { get; set; }

        [DisplayName("Cell Phone:")]
        public string ContactCellPhone { get; set; }

        [DisplayName("Fax Phone:")]
        public string ContactFax { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }
    }
}
