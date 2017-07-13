using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsDataModel;
using Kendo.Mvc.Extensions;
using System.IO;
using NLog;
using Kendo.Mvc.UI;

namespace JDsWebApp.Controllers
{
    public class JobHistoryController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index(int id = -1)
        {
            using (var db = new JDsDBEntities())
            {
                var matchingJob = db.Jobs.FirstOrDefault(x => x.JobId == id);
                ViewBag.JobNum = id;

                // no matching job id
                if (matchingJob == null)
                {
                    ViewBag.JobNum = "JOB NOT FOUND";
                    ViewBag.Status = "na";
                    ViewBag.Company = "na";
                    ViewBag.Received = "na";
                    ViewBag.Entries = new List<string>();
                    return View();
                }

                // found a match. 
                ViewBag.Status = matchingJob.JobStatu == null ? "unknown status" : matchingJob.JobStatu.Name;
                ViewBag.Company = matchingJob.Contact == null ? "unknown contact" : matchingJob.Contact?.Customer?.CompanyName;
                ViewBag.Received = matchingJob.ReceivedDate == null ? "unknown" : matchingJob.ReceivedDate.Value.ToString("M/dd/yyyy");
            
                List<string> entries = new List<string>();

                foreach (var entry in db.ChangeLogEntries.OrderBy(x => x.ChangeTime).Where(x => x.JobId == id))
                {
                    string dateStr = entry.ChangeTime.HasValue ? entry.ChangeTime.Value.ToString("MM/dd/yyyy hh:mm tt") : "";
                    entries.Add(string.Format("{0} - {1}", dateStr, entry.Message));
                }

                ViewBag.Entries = entries;
            }

            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult AddJobChangeLogEntry(string dateStr,  int jobId, string message)
        {
            try
            {
                using (var db = new JDsDBEntities())
                {
                    var date = DateTime.Parse(dateStr);

                    // It's possible the jobId == 0. If so, then get the most recently added log entry and 
                    // use the jobId from that as the jobId for this one
                    var changeLogEntry = new ChangeLogEntry();
                    changeLogEntry.ChangeTime = date;
                    changeLogEntry.JobId = jobId;
                    changeLogEntry.Message = message;

                    if (jobId == 0)
                    {
                        var last = db.ChangeLogEntries.OrderByDescending(x => x.ChangeTime).FirstOrDefault();
                        if (last != null)
                            changeLogEntry.JobId = last.JobId;
                    }

                    db.ChangeLogEntries.Add(changeLogEntry);
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}