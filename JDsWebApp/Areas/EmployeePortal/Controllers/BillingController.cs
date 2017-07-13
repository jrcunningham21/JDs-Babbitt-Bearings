using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.Billing;
using JDsWebApp.Areas.EmployeePortal.Models.Certificate;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using JDsWebApp.Helpers;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class BillingController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();
        
        public ActionResult BillingReport()
        {
            return View();
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult MarkJobAsBilled(int jobId)
        {
            try
            {
                using (var db = new JDsDBEntities())
                {
                    var job = db.Jobs.Find(jobId);
                    job.BilledDate = DateTime.Today + new TimeSpan(12, 0, 0);   // set it at noon today
                    job.JobStatusId = (int)JobStatus.BilledPaidFull;
                    db.SaveChanges();

                    StatusChangeHelper.PublishJobStatusChange(job.JobId, job.JobStatusId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error marking job {jobId} as billed: {ex.Message}");
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ReadJobsForBilling()
        {
            var jobs =_db.Jobs.Where(x => x.IsActive == true && 
                                           x.JobStatusId != null &&
                                           x.JobStatusId != (int)JDsWebApp.Helpers.JobStatus.New &&
                                           x.JobStatusId != (int)JDsWebApp.Helpers.JobStatus.Cancelled &&
                                           x.JobStatusId != (int)JDsWebApp.Helpers.JobStatus.Blocked &&
                                           x.JobStatusId != (int)JDsWebApp.Helpers.JobStatus.InProgress &&
                                           x.JobStatusId != (int)JDsWebApp.Helpers.JobStatus.Unknown) 
                              .Select( x=> new
                              {
                                  JobId = x.JobId,
                                  Contact = x.Contact,
                                  PurchaseOrderNumber = x.PurchaseOrderNumber,
                                  ShippedDate = x.ShippedDate,
                                  BilledDate = x.BilledDate,
                              })
                              .OrderByDescending(x => x.BilledDate.Value).ToList();

            int count = jobs.Count;

            var result = jobs.Select(x => new BillingJobViewModel
            {
                JobId = x.JobId,
                //CustomerNameAndJobNumber =
                //                (x.Contact != null && x.Contact.Customer != null)
                //                    ? x.Contact.Customer.CompanyName
                //                    : string.Empty,
                CustomerNameAndJobNumber = $"{x.Contact?.Customer?.CompanyName}/{x.PurchaseOrderNumber}",
                ShippedDate = x.ShippedDate.HasValue ? x.ShippedDate.Value.ToString("M/d/yyyy") : "",
                BilledDate = x.BilledDate.HasValue ? x.BilledDate.Value.ToString("M/d/yyyy") : ""
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}