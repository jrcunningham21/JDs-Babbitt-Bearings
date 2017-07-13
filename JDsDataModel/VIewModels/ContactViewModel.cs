using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsDataModel.ViewModels
{
    public class ContactViewModel
    {
        [DisplayName("Contact:")]
        public int ContactId { get; set; }

        [DisplayName("Customer:")]
        public int CustomerId { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [DisplayName("Email:")]
        [Required(ErrorMessage = "An email address is required")]
        [EmailAddress(ErrorMessage = "Must be a valid Email Address")]
        public string Email { get; set; }

        [DisplayName("Work Phone:")]
        public string WorkPhone { get; set; }

        [DisplayName("Cell Phone:")]
        public string CellPhone { get; set; }

        [DisplayName("Fax:")]
        public string Fax { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }
    }
}
