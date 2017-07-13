using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.AutoComplete
{
    public class AutoCompleteModel
    {
        public int AutoCompleteID { get; set; }
        public int ControlID { get; set; }
        public string Value { get; set; }
    }
}