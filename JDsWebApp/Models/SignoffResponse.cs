using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Models
{
    /// <summary>
    /// Web API response to an attempt to sign in or sign off on something
    /// </summary>
    public class SignoffResponse
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeePIN { get; set; }

        public bool IsManager { get; set; }

        public bool IsValidPIN { get; set; }

    }
}
