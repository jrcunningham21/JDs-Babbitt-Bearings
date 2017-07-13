using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsDataModel.ViewModels.Processes.ProcessBabbittBearing
{
    public class IncomingInspectionAccessoryViewModel
    {
        public string Name { get; set; }

        public int PartId { get; set; }

        public int? Quantity { get; set; }

        public bool IsInstalled { get; set; }

        public int IncomingInspectionAccessoryId { get; set; }

    }
}
