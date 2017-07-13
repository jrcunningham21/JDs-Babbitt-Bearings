using JDsDataModel;
using JDsDataModel.ViewModels;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using JDsWebApp.Areas.EmployeePortal.Models.JobScheduler;
using JDsWebApp.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class JobSchedulerController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // GET: EmployeePortal/JobScheduler
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 3600, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult GetJobsForTimePeriod(string startStr, string endStr)
        {
            // We need to return the JD job #, Customer name, customer job #, contact name, contact phone
            // We're filtering by active jobs whose ship by date falls within the given range
            using (var db = new JDsDBEntities())
            {
                DateTime start = DateTime.Parse(startStr);
                DateTime end = DateTime.Parse(endStr);

                // adjust the start and end
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

                var jobs = db.Jobs.Where(x => (x.IsActive.HasValue == true && x.IsActive.Value == true) &&
                                         (x.ShipByDate.HasValue && x.ShipByDate.Value >= start && x.ShipByDate.Value <= end) && //(x.RequiredDate.HasValue && x.RequiredDate.Value >= start && x.RequiredDate.Value <= end) &&
                                         x.CustomerContactId != null && x.Contact.Customer != null
                                         && x.BilledDate.HasValue == false)
                                          .Select(x => new
                                          {
                                              JobId = x.JobId,
                                              JobStatusId = x.JobStatusId,
                                              RequiredBy = x.RequiredDate.Value.Month + "/" + x.RequiredDate.Value.Day + "/" + x.RequiredDate.Value.Year,
                                              ShipBy = x.ShipByDate.Value.Month + "/" + x.ShipByDate.Value.Day + "/" + x.ShipByDate.Value.Year,
                                              DisplayLine1 = x.JobId.ToString() + " " +  x.Contact.Customer.CompanyName,
                                              DisplayLine2 = "Customer Job: " + x.CustomerJobNumber,
                                              DisplayLine3 = x.Contact.FirstName + " " + x.Contact.LastName,
                                              QuoteOnly = x.QuoteOnly
                                          }).ToList();

                return Json(jobs, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult SetJobsShipByDate(int jobId, string shipBy)
        {
            try
            {
                int statusId = 0;
                DateTime oldDate;
                using (var db = new JDsDBEntities())
                {
                    var job = db.Jobs.Find(jobId);
                    var s = DateTime.Parse(shipBy);
                    oldDate = job.ShipByDate.Value;
                    job.ShipByDate = s;
                    statusId = job.JobStatusId.HasValue ? job.JobStatusId.Value : 0;
                    db.SaveChanges();
                }

                //StatusChangeHelper.PublishJobStatusChange(jobId, statusId);
                // Let the world  know we've rescheduled
                BigboardHub hub = new BigboardHub();
                hub.UpdateRequiredDate(jobId, oldDate, DateTime.Parse(shipBy));

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _logger.Trace($"Error setting jobs required date: {jobId} to {shipBy}: {e.Message}");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
                
        }

        public virtual IQueryable<Models.JobScheduler.JobViewModel> GetAll()
        {
            var list = new List<Models.JobScheduler.JobViewModel>();
            using (var db = new JDsDBEntities())
            {
                var allJobs = db.Jobs.Where(j => !string.IsNullOrEmpty(j.CustomerJobNumber)).ToList();

                foreach (var job in db.Jobs)
                {
                    var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.CustomerId == job.CustomerContactId).FirstOrDefault();

                    var vm = new Models.JobScheduler.JobViewModel()
                    {
                        AllPartsRequirePT = job.AllPartsRequirePT,
                        AllPartsRequireUT = job.AllPartsRequireUT,
                        CreatedByEmployeeId = job.CreatedByEmployeeId,
                        CustomerContactId = job.CustomerContactId,
                        CustomerJobNumber = job.CustomerJobNumber,
                        HoldForCustomerApproval = job.HoldForCustomerApproval,                        
                        JobID = job.JobId,
                        JobNotes = job.JobNotes,
                        OvertimeRequired = job.OvertimeRequired,
                        PurchaseOrderNumber = job.PurchaseOrderNumber,
                        QuoteOnly = job.QuoteOnly,
                        ReceivedDate = job.ReceivedDate,
                        RequiredDate = job.RequiredDate,
                        ShipByDate = job.ShipByDate ?? new DateTime(),
                        Header = "JD#: " + job.JobId + " " + (contact == null ? "" : contact?.Customer?.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber,
                        Start = job.RequiredDate ?? new DateTime(),
                        End = job.RequiredDate ?? new DateTime(),
                        IsAllDay = true,
                        Title = "JD#: " + job.JobId + " " + (contact == null ? "" : contact?.Customer?.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber
                    };
                    list.Add(vm);
                }
            }
            return list.AsQueryable();
        }

        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, Models.JobScheduler.JobViewModel task)
        {
            if (ModelState.IsValid)
            {
                UpdateJob(task, ModelState);
            }
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual void UpdateJob(Models.JobScheduler.JobViewModel task, ModelStateDictionary modelState)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                using (JDsDBEntities db = new JDsDBEntities())
                {
                    var job = db.Jobs.Where(x => x.JobId == task.JobID).FirstOrDefault();
                    var oldDate = job.RequiredDate.Value;
                    job.RequiredDate = new DateTime(task.Start.Year, task.Start.Month, task.Start.Day, 0, 0, 0, DateTimeKind.Utc);
                    db.SaveChanges();

                    BigboardHub hub = new BigboardHub();

                    hub.UpdateRequiredDate(job.JobId, oldDate, job.ShipByDate.Value);

                    logger.Trace("Vacation update success.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Vacation update failed:" + ex.Message);
            }
        }

        public JsonResult GetJobParts(int id, int sortMethod)
        {
            List<PartViewModel> pvms = new List<PartViewModel>();
            try
            {
                Step newStep = null;
                Process newProcess = null;
                using (JDsDBEntities db = new JDsDBEntities())
                {
                    var parts = db.Parts.Include("PartType").Where(p => p.IsActive && p.JobId == id).ToList();

                    if (sortMethod == 3)
                    {
                        parts.OrderBy(x => x.PartType.Name);
                    }

                    foreach (var part in parts)
                    {
                        var pvm = new PartViewModel
                        {
                            PartID = part.PartId,
                            JobID = part.JobId ?? 0,
                            WorkScope = part.WorkScope ?? "N/A",
                            ShipByDate = part.ShipByDate ?? new DateTime(),
                            PartStatusName = part.PartStatu?.Name ?? "N/A",
                            PartStatusId = part.PartStatusId ?? 0,
                            PartTypeName = part.PartType?.Name ?? "N/A",
                            CurrentStep = "",
                            StepNumber = 1,
                        };

                        var process = db.Processes.Where(p => p.PartId == part.PartId).FirstOrDefault();
                        if (process != null)
                        {
                            var Step = db.Steps.Where(s => s.ProcessId == process.ProcessId).OrderByDescending(x => x.StepNumber).FirstOrDefault();
                            pvm.CurrentStep = Step.Title;
                            pvm.StepNumber = Step.StepNumber;
                            pvm.ProcessId = process.ProcessId;
                        }
                        else
                        {
                            //create a process
                            newProcess = new Process
                            {
                                Name = string.Format("{0} - {1}", "Babbit Bearing", pvm.PartTypeName),
                                Description = string.Empty,
                                IsActive = true,
                                PartId = part.PartId,
                                ProcessTypeId = 1
                            };

                            db.Processes.Add(newProcess);

                            db.SaveChanges();

                            // Generate VM from the process information, serialize to step
                            BB1_IncomingInspectionViewModel viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(db, newProcess.ProcessId);
                            string json = JsonConvert.SerializeObject(viewmodel, Formatting.Indented);

                            newStep = new Step
                            {
                                StepNumber = 1,
                                ProcessId = newProcess.ProcessId,
                                Title = string.Format("{0} IncomingInspection", 1),
                                Version = viewmodel.Version,
                                DataType = "viewmodel",
                                StringValue = json,
                                Created = DateTime.UtcNow,
                            };

                            db.Steps.Add(newStep);

                            db.SaveChanges();

                            pvm.CurrentStep = newStep.Title;
                            pvm.StepNumber = newStep.StepNumber;
                            pvm.ProcessId = newProcess.ProcessId;
                        }
                        pvms.Add(pvm);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(pvms, JsonRequestBehavior.AllowGet);
        }
    }
}