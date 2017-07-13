using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDsWebApp.Models
{
    public class EmployeeSkillsModel
    {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public List<int> SkillIds { get; set; }
            public bool[] EmpHasSkillBoolArray { get; set; }
    }
}