using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Helpers
{

    enum JobStatus
    {
        Unknown = 0,
        New = 1,
        InProgress = 2,
        OnHold = 3,
        Blocked = 4,
        Finished = 5,
        NotBilled = 6,
        BilledUnpaid = 7,
        BilledPaidPartial = 8,
        BilledPaidFull = 9,
        Shipped = 10,
        Cancelled = 11,
        Closed = 12
    }

    enum PartStatus
    {
        Unknown = 0,
        NotStarted = 1,
        InProgress = 2,
        Finished = 3,
        Blocked = 4
    }
}