using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Helpers
{
    [HubName("BigboardHub")]
    public class BigboardHub : Hub
    {
        public void Send(int JobId, int JobStatus, string Header, string FirstName, string LastName, string WorkPhone, string ShipByDate, string PartStatus, bool QuoteOnly, DateTime RequiredDate, DateTime ReceivedDate, string CompanyName, bool isNew)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<BigboardHub>();
            context.Clients.All.update(JobId, JobStatus, Header, FirstName, LastName, WorkPhone, ShipByDate, PartStatus, QuoteOnly, RequiredDate.ToString("MM/dd/yyyy"), ReceivedDate.ToString("MM/dd/yyyy"), CompanyName, isNew);
        }
        
        public void UpdatePartStatus(int jobId, int partId, int newStatusId, string workScope, DateTime? shipByDate, bool hasSize, string stepTitle, string partTypeName, string identifyInfo, string partStatus, int stepNumber, long processId, bool requiresPT, bool requiresUT)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<BigboardHub>();            
            context.Clients.All.updatePartStatus(jobId, partId, newStatusId, workScope, shipByDate.HasValue ? shipByDate.Value.ToShortDateString() : null, hasSize, stepTitle, partTypeName, identifyInfo, partStatus, stepNumber, processId, requiresPT, requiresUT);
        }
        
        public void UpdateRequiredDate(int JobId, DateTime OldRequiredDate, DateTime RequiredDate)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<BigboardHub>();
            context.Clients.All.edit(JobId, OldRequiredDate.ToString("MM/dd/yyyy"), RequiredDate.ToString("MM/dd/yyyy"));
        }
    }
}
