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
    
    public partial class StepActivity
    {
        public long StepActivityId { get; set; }
        public long StepId { get; set; }
        public int ActivityId { get; set; }
        public string Employee { get; set; }
        public string Manager { get; set; }
        public System.DateTime Created { get; set; }
    
        public virtual Activity Activity { get; set; }
        public virtual Step Step { get; set; }
    }
}