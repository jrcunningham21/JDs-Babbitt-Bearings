//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JDsDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vacation
    {
        public int VacationId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> VacationSignOffId { get; set; }
        public int NumberOfVacationHoursUsed { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.DateTime Day { get; set; }
        public bool IsVacationTime { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual SignOff SignOff { get; set; }
    }
}