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
    
    public partial class ChangeLogEntry
    {
        public int ChangeLogEntryId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<System.DateTime> ChangeTime { get; set; }
        public string Message { get; set; }
    
        public virtual Job Job { get; set; }
    }
}
