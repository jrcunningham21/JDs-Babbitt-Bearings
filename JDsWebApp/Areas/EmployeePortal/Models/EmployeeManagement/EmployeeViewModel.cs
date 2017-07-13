using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JDsWebApp.Areas.EmployeePortal.Models.EmployeeManagement
{
    public class EmployeeViewModel
    {
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Employee Name is required")]
        public string Name { get; set; }

        [DisplayName("Address")]
        public int AddressId { get; set; }

        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }

        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("State")]
        public string State { get; set; }

        [DisplayName("Zip")]
        public string Zip { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter correct email")]
        //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }

        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b", ErrorMessage = "Please Enter a valid phone number")]
        public string Phone { get; set; }

        [DisplayName("mm/dd/yyyy")]
        public DateTime? HireDate { get; set; }

        [DisplayName("Hrs")]
        public int? VacationHoursEarned { get; set; }

        [DisplayName("Used")]
        public int? VacationHoursUsed { get; set; }

        [DisplayName("Name")]
        public string EmergencyContact { get; set; }

        [DisplayName("Phone")]
        public string EmergencyPhone { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; }

        [DisplayName("PIN")]
        [DataType(DataType.Password)]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN does mot meet the 4 characters in length")]
        [Required(ErrorMessage = "PIN is required")]
        public string PIN { get; set; }
    }
}