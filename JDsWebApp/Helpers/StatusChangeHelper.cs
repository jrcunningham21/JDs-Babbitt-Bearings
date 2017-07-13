using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Helpers
{
    /// <summary>
    /// Helper class intended to help publish job and part status change events
    /// </summary>
    public static class StatusChangeHelper
    {
        public static void PublishJobStatusChange(int jobId, int newStatusId)
        {
            // TODO: Call Signal R or something
            using (JDsDataModel.JDsDBEntities _db = new JDsDataModel.JDsDBEntities())
            {
                BigboardHub hub = new BigboardHub();
                var job = _db.Jobs.Where(j => j.JobId == jobId).FirstOrDefault();
                var contact = _db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.ContactId == job.Contact.ContactId).FirstOrDefault();
                var parts = _db.Parts.Where(p => p.IsActive && p.JobId == jobId).ToList();
                var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table

                hub.Send(
                       jobId,
                       newStatusId,
                       "JD#: " + jobId + " " + (contact.Customer.CompanyName ?? "") + ", Customer Job: #" + job.CustomerJobNumber,
                       contact.FirstName,
                       contact.LastName,
                       contact.WorkPhone,
                       job.ShipByDate.Value.ToShortDateString(),
                       partsFinished + "/" + parts.Count + " finished",
                       job.QuoteOnly.HasValue ? job.QuoteOnly.Value : false,
                       job.ShipByDate.Value,//job.RequiredDate.Value,
                       job.ReceivedDate.Value,
                       contact.Customer.CompanyName,
                       false
                );
            }            
        }

        public static void PublishPartStatusChange(int jobId, int partId, int newStatusId)
        {
            // TODO: Call Signal R or something
            using (JDsDataModel.JDsDBEntities _db = new JDsDataModel.JDsDBEntities())
            {
                BigboardHub hub = new BigboardHub();
                var part = _db.Parts.Include("PartType").Where(p => p.PartId == partId).FirstOrDefault();
                var partStatus = _db.PartStatus.Where(p => p.PartStatusId == newStatusId).FirstOrDefault();
                var sizes = _db.Sizes.Where(s => s.PartId == partId).FirstOrDefault();
                var process = _db.Processes.Where(p => p.PartId == part.PartId).FirstOrDefault();
                var step = _db.Steps.Where(s => s.ProcessId == process.ProcessId).OrderByDescending(x => x.StepNumber).FirstOrDefault();

                hub.UpdatePartStatus(jobId, partId, newStatusId, part.WorkScope, part.ShipByDate, sizes != null, step.Title, part.PartType.Name, part.IdentifyingInfo, partStatus.Name, step.StepNumber, step.ProcessId, part.RequiresPT ?? false, part.RequiresUT ?? false);
            }
        }

    }
}
