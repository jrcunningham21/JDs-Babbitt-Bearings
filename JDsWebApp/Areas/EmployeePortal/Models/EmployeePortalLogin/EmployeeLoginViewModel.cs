using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.EmployeePortalLogin
{
    public class EmployeeLoginViewModel
    {
        [DisplayName("PIN:")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "Please enter a numeric PIN")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN does mot meet the 4 characters in length")]
        [Required(ErrorMessage = "Please enter a numeric PIN")]
        public string Pin { get; set; }

        [DisplayName("Password:")]
        public string Password { get; set; }
    }
}