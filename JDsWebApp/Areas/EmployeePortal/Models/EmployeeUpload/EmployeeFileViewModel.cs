using System;
using System.ComponentModel;

namespace JDsWebApp.Areas.EmployeePortal.Models.EmployeeUpload
{
    public class EmployeeFileViewModel
    {
        [DisplayName("File:")]
        public int FileId { get; set; }

        [DisplayName("Employee:")]
        public int EmployeeId { get; set; }

        [DisplayName("Name:")]
        public string Filename { get; set; }

        [DisplayName("Uploaded:")]
        public DateTime? Uploaded { get; set; }

        public string URL { get; set; }
    }
}