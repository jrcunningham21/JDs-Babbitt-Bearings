using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class PartViewModel
    {
        public int PartID { get; set; }

        public int JobID { get; set; }
        public int StepNumber { get; set; }
        public long ProcessId { get; set; }
        public DateTime? ShipByDate { get; set; }
        public int PartStatusId { get; set; }
        public string PartTypeName { get; set; }
        public string PartStatusName { get; set; }
        public string CurrentStep { get; set; }
        [Display (Name = "Bearing Description")]
        public string ItemCode { get; set; }
        [Display (Name = "Scope")]
        public string WorkScope { get; set; }
        [Display (Name = "Bearing Type")]
        public string PartType { get; set; }
        [Display (Name = "Bearing Process")]
        public IList<PartProcess> PartProcesses { get; set; }
        public int SelectedPartProcessID { get; set; }
        [Display (Name = "Identification")]
        public string IdentifyingInfo { get; set; }
        [Display (Name = "Requires UT")]
        public bool RequiresUT { get; set; }
        [Display (Name = "Requires PT")]
        public bool RequiresPT { get; set; }
        public bool HasCustSizes { get; set; }
        public string PTUTDesc { get; set; }
        public string PartStatus { get; set; }
    }
}
