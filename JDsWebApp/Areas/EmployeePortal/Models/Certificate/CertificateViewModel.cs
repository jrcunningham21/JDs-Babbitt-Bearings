using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace JDsWebApp.Areas.EmployeePortal.Models.Certificate
{
    public class CertificateViewModel
    {
        [DisplayName("Certificate:")]
        public int CertificateId { get; set; }

        [DisplayName("Certificate File:")]
        public int? CertificateFileId { get; set; }

        [DisplayName("Certificate Name:")]
        [Required(ErrorMessage = "Certificate Name is required")]
        public string Name { get; set; }

        [DisplayName("Start Date:")]
        public string CertificateDate { get; set; }

        [DisplayName("Expires:")]
        public string CertificateExpires { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }

        [DisplayName("Certificate Issuer:")]
        public string CompanyName { get; set; }

        [DisplayName("File:")]
        public string OriginalFilename { get; set; }

        [DisplayName("URL")]
        public string URL { get; set; }
    }
}