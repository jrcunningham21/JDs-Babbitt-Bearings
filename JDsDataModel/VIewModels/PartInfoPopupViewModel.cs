using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels
{
    public class PartInfoPopupViewModel
    {
        public int PartId { get; set; }

        public int JobId { get; set; }

        public string RequiredDateStr { get; set; }

        public string EstimatedShipDateStr { get; set; }

        public string WorkScope { get; set; }

        public string JobNotes { get; set; }

        public string PartNotes { get; set; }

        public int PopupTabIndex { get; set; }

        public bool IsPartBlocked { get; set; }

        // Stuff the model needs, or at least needs to be associated generally with this...
        // - outgoing form
        // - access to customer sizes
    }

    /// <summary>
    /// Photos belonging to parts
    /// </summary>
    public class PartInfoPhotoViewModel
    {
        public int PartPhotoId { get; set; }

        public int PartId { get; set; }

        public string FileName { get; set; }

        public string URL { get; set; }

        public string PhotoNotes { get; set; }
    }

    /// <summary>
    /// Files belonging to parts
    /// </summary>
    public class PartInfoFileViewModel
    {
        public int PartFileId { get; set; }

        public int PartId { get; set; }

        public string FileName { get; set; }

        public string URL { get; set; }

        public string FileNotes { get; set; }

        public bool IsFinalPrint { get; set; }
    }
}
