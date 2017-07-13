using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsDataModel.ViewModels;
using JDsDataModel;
using System.Net;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeUpload;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using JDsWebApp.Helpers;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using JDsDataModel.ViewModels.Processes;
using Newtonsoft.Json;
using System.IO;
using RestSharp.Extensions;

namespace JDsWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        readonly JDsDBEntities db = new JDsDBEntities();

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index(int sort = 0, string searchText = "")
        {
            return View(BuildJobBoardModel(sort,searchText));
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BigBoardJobsView(int sort = 0, string searchText = "")
        {
            return PartialView(BuildJobBoardModel(sort,searchText));
        }

        public BigBoardJobViewModelTest BuildJobBoardModel(int sortMethod, string searchText = "")
        {
            BigBoardJobViewModelTest model = new BigBoardJobViewModelTest();

            var jobQuery = db.Jobs
                            // Get active jobs that are still in play (not billed, canceled, or closed)
                            .Where(x => x.JobId > 0 && x.RequiredDate != null &&
                                    x.IsActive == true &&
                                    x.Contact != null && x.Contact.Customer != null &&
                                    x.RequiredDate.HasValue && x.ReceivedDate.HasValue &&
                                    x.ShipByDate.HasValue &&
                                    x.JobStatusId.HasValue && x.JobStatusId <= 5
                                    && x.BilledDate.HasValue == false);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                DateTime dt = new DateTime();

                if (DateTime.TryParse(searchText, out dt))
                {
                    jobQuery =
                        jobQuery.Where(
                            x =>
                                x.ReceivedDate.Value.Equals(dt) || x.RequiredDate.Value.Equals(dt) ||
                                x.ShipByDate.Value.Equals(dt) || x.JobId.ToString().Contains(searchText) ||
                                (x.Contact.FirstName + " " + x.Contact.LastName).ToLower()
                                    .Contains(searchText.ToLower()) ||
                                x.CustomerJobNumber.Contains(searchText) ||
                                x.Parts.Any(y => y.ItemCode.Contains(searchText)));
                }

                else
                {
                    jobQuery = jobQuery.Where(x => x.JobId.ToString().Contains(searchText) ||
                                               (x.Contact.FirstName + " " + x.Contact.LastName).ToLower()
                                                   .Contains(searchText.ToLower()) ||
                                               x.CustomerJobNumber.Contains(searchText) ||
                                               x.Parts.Any(y => y.ItemCode.Contains(searchText)));
                }
            }

            var jobsModel = jobQuery.Select(x => new JobBoardViewModel()
            {
                JobID = x.JobId,
                CompanyName = x.Contact.Customer.CompanyName,
                ShipByDate = x.ShipByDate.Value,
                JobStatus = x.JobStatu.Name,
                RequiredDate = x.RequiredDate.Value,
                ReceivedDate = x.ReceivedDate.Value,
                SortMethod = sortMethod,
                QuoteOnly = x.QuoteOnly ?? false,
                PartNames = x.Parts.Select(s => (s.PartType != null ? s.PartType.Name : "N/A") + " | " + s.IdentifyingInfo).ToList()
            }).ToList();

            IEnumerable<IGrouping<string, JobBoardViewModel>> stringGrouping = null;

            if (sortMethod == 0 || sortMethod == 3)
            {
                stringGrouping = jobsModel.GroupBy(x => x.ShipByDate.ToShortDateString()).OrderBy(x => DateTime.Parse(x.Key));
            }

            else if(sortMethod == 1)
            {
                stringGrouping = jobsModel.GroupBy(x => x.CompanyName).OrderBy(x => x.Key);
            }

            else if (sortMethod == 2)
            {
                stringGrouping = jobsModel.GroupBy(x => x.ReceivedDate.ToShortDateString()).OrderBy(x => DateTime.Parse(x.Key));
            }
            else
            {
                stringGrouping = jobsModel.GroupBy(x => x.ShipByDate.ToShortDateString()).OrderBy(x => DateTime.Parse(x.Key));
            }

            model.SortMethod = sortMethod;
            model.JobDateGrouping = stringGrouping.ToList();

            return model;
        }

        public Step GetCurrentStep(long processID)
        {
            Step Step = null;
            var Steps = db.Steps.Where(s => s.ProcessId == processID).OrderBy(x => x.StepNumber).ToList();

            var currentStepindex = -1;
            if (Steps.Count() != 0)
            {
                for (var i = Steps.Count - 1; i >= 0; i--) //start from the highest step 
                {
                    if (Steps[i].StringValue == null)
                        continue;
                    else
                    {
                        StepViewModel stepInfo = JsonConvert.DeserializeObject((Steps[i].StringValue), typeof(StepViewModel)) as StepViewModel;
                        if (stepInfo.IsCompleted == false)
                        {
                            continue;
                        }
                        else
                        {
                            if (i + 1 == Steps.Count)
                            {
                                Step = Steps[i];
                                currentStepindex = i;
                                break;
                            }
                            else
                            {
                                Step = Steps[i + 1];
                                currentStepindex = i + 1;
                                break;
                            }
                        }
                    }
                }
                if (currentStepindex != -1)
                {
                    Step = Steps[currentStepindex];
                }
                else
                {
                    Step = Steps[0];    //there are steps, but none of them are completed.
                }

            }

            else //If there are not steps for this job, create one @ BB1
            {
                BB1_IncomingInspectionViewModel viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(db, processID);
                string json = JsonConvert.SerializeObject(viewmodel, Formatting.Indented);

                Step = new Step
                {
                    StepNumber = 1,
                    ProcessId = processID,
                    Title = string.Format("IncomingInspection"),
                    Version = viewmodel.Version,
                    DataType = "viewmodel",
                    StringValue = json,
                    Created = DateTime.UtcNow,
                };

                db.Steps.Add(Step);
                db.SaveChanges();
            }

            return Step;
        }

        [HttpGet]
        public ActionResult BigBoardPartsView(int id, int sortMethod = 0)
        {
            RightBigBoardViewModel model = new RightBigBoardViewModel();

            Step newStep = null;
            Process newProcess = null;

            model.JobID = id;
            model.PartsViews = new List<PartViewModel>();

            var parts = db.Parts.Include("PartType").Where(p => p.IsActive && p.JobId == id).ToList();

            if (sortMethod == 3)
            {
                parts.OrderBy(x => x.PartType.Name);
            }

            foreach (var part in parts)
            {
                var sizes = db.Sizes.FirstOrDefault(s => s.PartId == part.PartId);
                var pvm = new PartViewModel
                {
                    PartID = part.PartId,
                    JobID = part.JobId ?? 0,
                    ItemCode = part.ItemCode ?? "N/A",
                    WorkScope = part.WorkScope ?? "N/A",
                    PartStatusName = part.PartStatu?.Name ?? "N/A",
                    PartStatusId = part.PartStatusId ?? 0,
                    PartTypeName = part.PartType?.Name ?? "N/A",
                    IdentifyingInfo = part.IdentifyingInfo,
                    CurrentStep = "",
                    StepNumber = 1,
                    RequiresPT = part.RequiresPT ?? false,
                    RequiresUT = part.RequiresUT ?? false,
                    HasCustSizes = sizes != null
                };

                var process = db.Processes.FirstOrDefault(p => p.PartId == part.PartId);
                if (process != null)
                {
                    Step Step = GetCurrentStep(process.ProcessId);
                    pvm.CurrentStep = Step.Title;
                    pvm.StepNumber = Step.StepNumber;
                    pvm.ProcessId = process.ProcessId;
                }
                else
                {
                    //create a process
                    newProcess = new Process
                    {
                        Name = string.Format("{0} - {1}", "Babbit Bearing", part.IdentifyingInfo),
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
                        Title = string.Format("IncomingInspection"),
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
                model.PartsViews.Add(pvm);
            }

            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetMoreJobs(int set, int sort)
        {
            JsonResult result = null;
            // TODO: Short-cutting this for now:

            result = Json(null, JsonRequestBehavior.AllowGet);

            switch (sort)
            {
                case 0:

                    var currentSet = set * 100;
                    var jobMaxCount = db.Jobs.Where(x => x.JobId > 0 && x.RequiredDate != null).ToList().Count();
                    var next = 100;
                    if (currentSet + next > jobMaxCount)
                        next = jobMaxCount - currentSet;

                    var jobs = db.Jobs
                        .AsEnumerable()
                        .Where(x => x.JobId > 0 && x.RequiredDate != null)
                        .OrderByDescending(o => o.RequiredDate)
                        .ToList()
                        .Skip(currentSet - 1)
                        .Take(next)
                        .GroupBy(x => x.RequiredDate)
                        .Select(x => x.ToList())
                        .ToList();

                    List<List<JobViewModel>> groups = new List<List<JobViewModel>>();

                    foreach (var group in jobs)
                    {
                        List<JobViewModel> models = new List<JobViewModel>();
                        foreach (var job in group)
                        {
                            try
                            {
                                var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.CustomerId == job.CustomerContactId).FirstOrDefault();
                                var parts = db.Parts.Where(p => p.IsActive && p.JobId == job.JobId).ToList();
                                var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table

                                if (contact == null)
                                {
                                    _logger.Error("JobId={0} does not have a customer contact associated with.", job.JobId);
                                }
                                else
                                {
                                    models.Add(new JobViewModel
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
                                        ShipByDate = job.ShipByDate,
                                        CustomerId = contact.CustomerId,
                                        ContactId = contact.ContactId,
                                        Header = "JD#: " + job.JobId + " " + (contact.Customer == null ? "N/A" : contact.Customer.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber,
                                        JobStatus = job.JobStatu?.Name,
                                        Contact = new ContactViewModel
                                        {
                                            CellPhone = contact.CellPhone,
                                            ContactId = contact.ContactId,
                                            CustomerId = contact.CustomerId,
                                            Email = contact.Email,
                                            Fax = contact.Fax,
                                            FirstName = contact.FirstName,
                                            LastName = contact.LastName,
                                            Notes = contact.Notes,
                                            WorkPhone = contact.WorkPhone
                                        },
                                        Customer = new CustomerViewModel
                                        {
                                            BillingAddressId = contact.Customer.BillingAddressId,
                                            Code = contact.Customer.Code,
                                            CompanyName = contact.Customer.CompanyName,
                                            CustomerId = contact.Customer.CustomerId,
                                            Notes = contact.Customer.Notes,
                                            PrimaryContactId = contact.Customer.PrimaryContactId,
                                            ShippingAddressId = contact.Customer.ShippingAddressId
                                        },
                                        PartStatus = partsFinished + "/" + parts.Count + " complete.",
                                        SortMethod = sort,
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Failed to add to view model: " + e);
                            }
                        }
                        groups.Add(models);
                    }

                    result = Json(groups, JsonRequestBehavior.AllowGet);
                    break;
                case 1:

                    var currentSet2 = set * 100;
                    var jobMaxCount2 = db.Jobs.Where(x => x.JobId > 0 && x.CustomerContactId > 0).ToList().Count();
                    var next2 = 100;
                    if (currentSet2 + next2 > jobMaxCount2)
                        next2 = jobMaxCount2 - currentSet2;

                    jobs = db.Jobs
                        .AsEnumerable()
                        .Where(x => x.JobId > 0 && x.CustomerContactId > 0)
                        .OrderBy(o => o.Contact.Customer.CompanyName)
                        .ToList()
                        .Skip(currentSet2 - 1)
                        .Take(next2)
                        .GroupBy(x => x.Contact.Customer.CompanyName)
                        .Select(x => x.ToList())
                        .ToList();
                    List<List<JobViewModel>> groups2 = new List<List<JobViewModel>>();

                    foreach (var group in jobs)
                    {
                        List<JobViewModel> models = new List<JobViewModel>();
                        foreach (var job in group)
                        {
                            try
                            {
                                var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.CustomerId == job.CustomerContactId).FirstOrDefault();
                                var parts = db.Parts.Where(p => p.IsActive && p.JobId == job.JobId).ToList();
                                var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table

                                if (contact == null)
                                {
                                    _logger.Error("JobId={0} does not have a customer contact associated with.", job.JobId);
                                }
                                else
                                {
                                    models.Add(new JobViewModel
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
                                        ShipByDate = job.ShipByDate,
                                        CustomerId = contact.CustomerId,
                                        ContactId = contact.ContactId,
                                        Header = "JD#: " + job.JobId + " " + (contact.Customer == null ? "N/A" : contact.Customer.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber,
                                        JobStatus = job.JobStatu?.Name,
                                        Contact = new ContactViewModel
                                        {
                                            CellPhone = contact.CellPhone,
                                            ContactId = contact.ContactId,
                                            CustomerId = contact.CustomerId,
                                            Email = contact.Email,
                                            Fax = contact.Fax,
                                            FirstName = contact.FirstName,
                                            LastName = contact.LastName,
                                            Notes = contact.Notes,
                                            WorkPhone = contact.WorkPhone
                                        },
                                        Customer = new CustomerViewModel
                                        {
                                            BillingAddressId = contact.Customer.BillingAddressId,
                                            Code = contact.Customer.Code,
                                            CompanyName = contact.Customer.CompanyName,
                                            CustomerId = contact.Customer.CustomerId,
                                            Notes = contact.Customer.Notes,
                                            PrimaryContactId = contact.Customer.PrimaryContactId,
                                            ShippingAddressId = contact.Customer.ShippingAddressId
                                        },
                                        PartStatus = partsFinished + "/" + parts.Count + " complete.",
                                        SortMethod = sort,
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Failed to add to view model: " + e);
                            }
                        }
                        groups2.Add(models);
                    }

                    result = Json(groups2, JsonRequestBehavior.AllowGet);
                    break;
                case 2:

                    var currentSet3 = set * 100;
                    var jobMaxCount3 = db.Jobs.Where(x => x.JobId > 0 && x.ReceivedDate != null).ToList().Count();
                    var next3 = 100;
                    List<List<JobViewModel>> groups3 = new List<List<JobViewModel>>();
                    if (currentSet3 <= jobMaxCount3)
                    {
                        if (currentSet3 + next3 > jobMaxCount3)
                            next = jobMaxCount3 - currentSet3;

                        jobs = db.Jobs
                            .AsEnumerable()
                            .Where(x => x.JobId > 0 && x.ReceivedDate != null)
                            .OrderByDescending(o => o.ReceivedDate)
                            .ToList()
                            .Skip(currentSet3 - 1)
                            .Take(next3)
                            .GroupBy(x => x.ReceivedDate)
                            .Select(x => x.ToList())
                            .ToList();


                        foreach (var group in jobs)
                        {
                            List<JobViewModel> models = new List<JobViewModel>();
                            foreach (var job in group)
                            {
                                try
                                {
                                    var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.CustomerId == job.CustomerContactId).FirstOrDefault();
                                    var parts = db.Parts.Where(p => p.IsActive && p.JobId == job.JobId).ToList();
                                    var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table

                                    if (contact == null)
                                    {
                                        _logger.Error("JobId={0} does not have a customer contact associated with.", job.JobId);
                                    }
                                    else
                                    {
                                        models.Add(new JobViewModel
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
                                            ShipByDate = job.ShipByDate,
                                            CustomerId = contact.CustomerId,
                                            ContactId = contact.ContactId,
                                            Header = "JD#: " + job.JobId + " " + (contact.Customer == null ? "N/A" : contact.Customer.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber,
                                            JobStatus = job.JobStatu?.Name,
                                            Contact = new ContactViewModel
                                            {
                                                CellPhone = contact?.CellPhone,
                                                ContactId = contact.ContactId,
                                                CustomerId = contact.CustomerId,
                                                Email = contact?.Email,
                                                Fax = contact?.Fax,
                                                FirstName = contact?.FirstName,
                                                LastName = contact?.LastName,
                                                Notes = contact?.Notes,
                                                WorkPhone = contact?.WorkPhone
                                            },
                                            Customer = new CustomerViewModel
                                            {
                                                BillingAddressId = contact?.Customer?.BillingAddressId,
                                                Code = contact?.Customer?.Code,
                                                CompanyName = contact?.Customer?.CompanyName,
                                                CustomerId = contact.Customer.CustomerId,
                                                Notes = contact?.Customer?.Notes,
                                                PrimaryContactId = contact?.Customer?.PrimaryContactId,
                                                ShippingAddressId = contact?.Customer?.ShippingAddressId
                                            },
                                            PartStatus = partsFinished + "/" + parts.Count + " complete.",
                                            SortMethod = sort,
                                        });
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Failed to add to view model: " + e);
                                }
                            }
                            groups3.Add(models);
                        }

                        result = Json(groups3, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:

                    var currentSet4 = set * 100;
                    var jobMaxCount4 = db.Jobs.Where(x => x.JobId > 0 && x.RequiredDate != null).ToList().Count();
                    var next4 = 100;
                    if (currentSet4 + next4 > jobMaxCount4)
                        next = jobMaxCount4 - currentSet4;

                    jobs = db.Jobs
                        .AsEnumerable()
                        .Where(x => x.JobId > 0 && x.RequiredDate != null)
                        .OrderByDescending(o => o.RequiredDate)
                        .ToList()
                        .Skip(currentSet4 - 1)
                        .Take(next4)
                        .GroupBy(x => x.RequiredDate)
                        .Select(x => x.ToList())
                        .ToList();

                    List<List<JobViewModel>> groups4 = new List<List<JobViewModel>>();

                    foreach (var group in jobs)
                    {
                        List<JobViewModel> models = new List<JobViewModel>();
                        foreach (var job in group)
                        {
                            try
                            {
                                var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.CustomerId == job.CustomerContactId).FirstOrDefault();
                                var parts = db.Parts.Where(p => p.IsActive && p.JobId == job.JobId).ToList();
                                var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table

                                if (contact == null)
                                {
                                    _logger.Error("JobId={0} does not have a customer contact associated with.", job.JobId);
                                }
                                else
                                {
                                    models.Add(new JobViewModel
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
                                        ShipByDate = job.ShipByDate,
                                        CustomerId = contact.CustomerId,
                                        ContactId = contact.ContactId,
                                        Header = "JD#: " + job.JobId + " " + (contact.Customer == null ? "N/A" : contact.Customer.CompanyName) + ", Customer Job: #" + job.CustomerJobNumber,
                                        JobStatus = job.JobStatu?.Name,
                                        Contact = new ContactViewModel
                                        {
                                            CellPhone = contact.CellPhone,
                                            ContactId = contact.ContactId,
                                            CustomerId = contact.CustomerId,
                                            Email = contact.Email,
                                            Fax = contact.Fax,
                                            FirstName = contact.FirstName,
                                            LastName = contact.LastName,
                                            Notes = contact.Notes,
                                            WorkPhone = contact.WorkPhone
                                        },
                                        Customer = new CustomerViewModel
                                        {
                                            BillingAddressId = contact.Customer.BillingAddressId,
                                            Code = contact.Customer.Code,
                                            CompanyName = contact.Customer.CompanyName,
                                            CustomerId = contact.Customer.CustomerId,
                                            Notes = contact.Customer.Notes,
                                            PrimaryContactId = contact.Customer.PrimaryContactId,
                                            ShippingAddressId = contact.Customer.ShippingAddressId
                                        },
                                        PartStatus = partsFinished + "/" + parts.Count + " complete.",
                                        SortMethod = sort,
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Failed to add to view model: " + e);
                            }
                        }
                        groups4.Add(models);
                    }

                    result = Json(groups4, JsonRequestBehavior.AllowGet);
                    break;
            }



            return result;
        }

        [HttpGet]
        public JsonResult GetJobParts(int id, int sortMethod)
        {
            List<PartViewModel> pvms = new List<PartViewModel>();
            try
            { 
                Step newStep = null;
                Process newProcess = null;
                    
                var parts = db.Parts.Include("PartType").Where(p => p.IsActive && p.JobId == id).ToList();

                if (sortMethod == 3)
                {
                    parts.OrderBy(x => x.PartType.Name);
                }

                foreach (var part in parts)
                {
                    var sizes = db.Sizes.Where(s => s.PartId == part.PartId).FirstOrDefault();
                    var pvm = new PartViewModel
                    {
                        PartID = part.PartId,
                        JobID = part.JobId ?? 0,
                        WorkScope = part.WorkScope ?? "N/A",
                        PartStatusName = part.PartStatu?.Name ?? "N/A",
                        PartStatusId = part.PartStatusId ?? 0,
                        PartTypeName = part.PartType?.Name ?? "N/A",
                        IdentifyingInfo = part.IdentifyingInfo,
                        CurrentStep = "",
                        StepNumber = 1,
                        RequiresPT = part.RequiresPT.Value,
                        RequiresUT = part.RequiresUT.Value,      
                        HasCustSizes = sizes != null                  
                    };

                    var process = db.Processes.FirstOrDefault(p => p.PartId == part.PartId);
                    if (process != null)
                    {
                        var Step = db.Steps.Where(s => s.ProcessId == process.ProcessId).OrderByDescending(x => x.StepNumber).FirstOrDefault();
                        if (Step == null)
                        {
                            BB1_IncomingInspectionViewModel viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(db, newProcess.ProcessId);
                            string json = JsonConvert.SerializeObject(viewmodel, Formatting.Indented);

                            Step = new Step
                            {
                                StepNumber = 1,
                                ProcessId = newProcess.ProcessId,
                                Title = string.Format("IncomingInspection"),
                                Version = viewmodel.Version,
                                DataType = "viewmodel",
                                StringValue = json,
                                Created = DateTime.UtcNow,
                            };

                            db.Steps.Add(Step);
                            db.SaveChanges();
                        }
                        pvm.CurrentStep = Step.Title;
                        pvm.StepNumber = Step.StepNumber;
                        pvm.ProcessId = process.ProcessId;


                    }
                    else
                    {
                        //create a process
                        newProcess = new Process
                        {
                            //Name = string.Format("{0} - {1}", "Babbit Bearing", pvm.PartTypeName),
                            Name = string.Format("{0} - {1}", "Babbit Bearing", part.IdentifyingInfo),
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
                            Title = string.Format("IncomingInspection"),
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(pvms, JsonRequestBehavior.AllowGet);
        }    

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AllJobs()
        {
            return View();
        }
        
        [HttpGet]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult JobDetails(int? id)
        {
            JobViewModel model = new JobViewModel();
            if (id.HasValue)
            {
                Job job = db.Jobs.Single(x => x.JobId == id);

                model.Header = "Edit Job Details";
                model.JobID = job.JobId;
                model.CustomerId = job.Contact?.CustomerId ?? 0;
                model.ContactId = job.CustomerContactId.GetValueOrDefault();
                model.CustomerJobNumber = job.CustomerJobNumber;
                model.PurchaseOrderNumber = job.PurchaseOrderNumber;
                model.ReceivedDate = job.ReceivedDate;
                model.RequiredDate = job.RequiredDate;
                model.ShipByDate = job.ShipByDate;
                model.OvertimeRequired = job.OvertimeRequired;
                model.CoercedHoldForCustomerApproval = job.HoldForCustomerApproval ?? false;
                model.CoercedQuoteOnly = job.QuoteOnly ?? false;
                model.CoercedAllPartsRequireUT = job.AllPartsRequireUT ?? false;
                model.CoercedAllPartsRequirePT = job.AllPartsRequirePT ?? false;
                model.JobNotes = job.JobNotes;

                ViewBag.Customers = new SelectList(db.Customers.Where(c => c.IsActive == true).OrderBy(s => s.CompanyName), "CustomerId", "CompanyName");

                // find the job's customer
                // You got a job. Job has 1 contact. That contact has a customer. That customer has a bunch of contacts.
                var contactList = job?.Contact?.Customer?.Contacts?.AsEnumerable();

                if (contactList != null)
                {
                    ViewBag.Contacts = new SelectList(contactList.Select(x => new
                    {
                        ContactId = x.ContactId,
                        ContactName = x.FirstName + " " + x.LastName,
                        IsPrimaryContact = x.Customer.PrimaryContactId == x.ContactId
                    }).OrderByDescending(x => x.IsPrimaryContact)
                    .ToList(), "ContactId", "ContactName");
                }
                else
                {
                    // you got a new job--no customer or contacts--return an empty contact list
                    ViewBag.Contacts = new SelectList(new List<SelectListItem>(), "ContactId", "ContactName");
                }
            }
            else
            {
                // Create an empty job in the DB
                // Set it to 'inactive' so it won't show up in the list if it gets saved
                // make sure the model.jobid is set iwth the new ID from the DB
                // Remember when you save it again to set its active to true
                //int JobId = CreateJob();

                //model.JobID = JobId;
                model.IsNewJob = true;
                model.OvertimeRequired = false;
                model.Header = "Add Job Details";
                model.ReceivedDate = DateTime.Now;
                ViewBag.Customers = new SelectList(db.Customers.Where(c => c.IsActive == true).OrderBy(s => s.CompanyName), "CustomerId", "CompanyName");
                ViewBag.Contacts = new SelectList(Enumerable.Empty<SelectList>());
            }                        
            return View(model);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AddJob(JobViewModel model)
        {
            BigboardHub hub = new BigboardHub();

            if (ModelState.IsValid)
            {
                bool isNewJob = false;
                model.Header = "Edit Job Details";
                Job currentJob = db.Jobs.Where(x => x.JobId == model.JobID).FirstOrDefault();

                if(currentJob == null)
                {
                    currentJob = new Job();
                    isNewJob = true;
                    db.Jobs.Add(currentJob);
                }

                currentJob.CustomerContactId = model.ContactId;
                currentJob.CustomerJobNumber = model.CustomerJobNumber;
                currentJob.PurchaseOrderNumber = model.PurchaseOrderNumber;
                currentJob.ReceivedDate = model.ReceivedDate;
                currentJob.RequiredDate = model.RequiredDate;
                currentJob.LastUpdated = DateTime.Now;
                currentJob.ShipByDate = model.ShipByDate;
                currentJob.OvertimeRequired = model.CoercedOvertimeRequired;
                currentJob.HoldForCustomerApproval = model.CoercedHoldForCustomerApproval;
                currentJob.QuoteOnly = model.CoercedQuoteOnly;
                currentJob.AllPartsRequirePT = model.CoercedAllPartsRequirePT;
                currentJob.AllPartsRequireUT = model.CoercedAllPartsRequireUT;
                currentJob.JobNotes = model.JobNotes;

                if (currentJob.HoldForCustomerApproval.Value)
                    currentJob.JobStatusId = (int)JobStatus.OnHold;
                else
                    currentJob.JobStatusId = (int)JobStatus.New;
                
                
                var contact = db.Contacts.Include("Customer").Where(c => c.IsActive.HasValue & c.IsActive.Value & c.ContactId == model.ContactId).FirstOrDefault();
                var companyName = contact == null ? "" : contact.Customer?.CompanyName;

                var parts = db.Parts.Where(p => p.IsActive && p.JobId == currentJob.JobId).ToList();
                var partsFinished = parts.Where(p => p.PartStatusId == 3).ToList().Count; //FJ: 3 = Finish in PartStatus lookup table
                if (currentJob.RequiredDate != null && currentJob.IsActive == null) //if job has required date and is a new job (IsActive == null), allow signalR to send to bigboard
                {
                    hub.Send(
                        currentJob.JobId,
                        currentJob.JobStatusId.Value,
                        "JD#: " + currentJob.JobId + " " + companyName + ", Customer Job: #" + currentJob.CustomerJobNumber,
                        contact.FirstName,
                        contact.LastName,
                        contact.WorkPhone,
                        currentJob.ShipByDate.Value.ToShortDateString(),
                        partsFinished + "/" + parts.Count + " finished",
                        currentJob.QuoteOnly.HasValue ? currentJob.QuoteOnly.Value : false,
                        model.ShipByDate.Value,
                        model.ReceivedDate.Value,
                        contact.Customer.CompanyName,
                        true
                    );
                }
                else if(currentJob.IsActive.HasValue && currentJob.IsActive.Value == true) //if job edit (IsActive == true), then update existing board
                {
                    hub.Send(
                        currentJob.JobId,
                        currentJob.JobStatusId.Value,
                        "JD#: " + currentJob.JobId + " " + (contact.Customer.CompanyName ?? "") + ", Customer Job: #" + currentJob.CustomerJobNumber,
                        contact.FirstName,
                        contact.LastName,
                        contact.WorkPhone,
                        currentJob.ShipByDate.Value.ToShortDateString(),
                        partsFinished + "/" + parts.Count + " finished",
                        currentJob.QuoteOnly.HasValue ? currentJob.QuoteOnly.Value : false,
                        model.ShipByDate.Value,
                        model.ReceivedDate.Value,
                        contact.Customer.CompanyName,
                        false
                    );
                }

                currentJob.IsActive = true;

                // Handle the job status
                if (currentJob.HoldForCustomerApproval.HasValue && currentJob.HoldForCustomerApproval == true)
                {
                    // If the hold box is checked, make sure it's status is on hold
                    currentJob.JobStatusId = (int)JobStatus.OnHold;
                }
                else if (currentJob.JobStatusId == null)
                {
                    // If it hasn't been set yet, it'll be new
                    currentJob.JobStatusId = (int)JobStatus.New;
                }
                else if (currentJob.HoldForCustomerApproval.HasValue && currentJob.HoldForCustomerApproval == false)
                {
                    // It is moving from hold to something else - maybe in progress
                    currentJob.JobStatusId = (int)JobStatus.InProgress;
                }

                if (!isNewJob)
                {
                    db.Entry<Job>(currentJob).State = System.Data.Entity.EntityState.Modified; 
                }

                try
                {
                    db.SaveChanges();
                }

                catch(Exception e)
                {
                    _logger.Trace($"Error adding/saving job: {e.Message}");
                    Console.WriteLine(e.Message);                    
                }

                ViewBag.Customers = new SelectList(db.Customers.Where(c => c.IsActive == true).OrderBy(s => s.CompanyName), "CustomerId", "CompanyName");
                ViewBag.Contacts = new SelectList((from c in db.Contacts
                                                   orderby c.LastName
                                                   where (c.IsActive.HasValue && c.IsActive.Value && c.CustomerId == model.CustomerId)
                                                   select new
                                                   {
                                                       ContactId = c.ContactId,
                                                       ContactName = c.FirstName + " " + c.LastName
                                                   }).ToList(), "ContactId", "ContactName");
                return Json(new { IsSuccessful = true, JobID = currentJob.JobId });
            }
            else
            {
                model.Header = "Edit Job Details";

                ViewBag.Customers = new SelectList(db.Customers.Where(c => c.IsActive == true).OrderBy(s => s.CompanyName), "CustomerId", "CompanyName");
                ViewBag.Contacts = new SelectList((from c in db.Contacts
                                                   orderby c.LastName
                                                   where c.CustomerId == model.CustomerId
                                                   select new
                                                   {
                                                       ContactId = c.ContactId,
                                                       ContactName = c.FirstName + " " + c.LastName
                                                   }).ToList(), "ContactId", "ContactName");

                return Json(new { IsSuccessful = false, View = this.RenderRazorViewToString("JobDetails", model) });
            }
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult DeleteJob(int id)
        {
            Job job = db.Jobs.Find(id);
            job.IsActive = false;

            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult DeleteJobPart(int id)
        {
            Part part = db.Parts.Find(id);
            part.IsActive = false;

            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult GetContacts(int customerId)
        {
            var contacts = (from contact in db.Contacts
                            join customer in db.Customers on contact.CustomerId equals customer.CustomerId
                            where (contact.IsActive == true && contact.CustomerId == customerId)
                            orderby contact.LastName
                         select new
                         {
                             ContactID = contact.ContactId,
                             ContactName = contact.FirstName + " " + contact.LastName,
                             IsPrimaryContact = customer.PrimaryContactId == contact.ContactId
                         }).ToList();

            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadPartsForJob(int id)
        {
            var partsTest = db.Parts.Include("PartType").Where(x => x.IsActive && x.JobId == id).ToList();

            // Removes 'ItemCode' (first chars before space) from Bearing Description 
            // so that only the description is shown in the grid
            foreach (var p in partsTest)
            {
                var split = (p.ItemCode ?? "").Split(',');
                var formattedString = "";
                foreach(var s in split)
                {
                    formattedString += s.Substring(s.IndexOf(" ") + 1) + ',';
                }
                formattedString = formattedString.TrimEnd(',');
                p.ItemCode = formattedString;
            }
            //Step step = GetCurrentStep(part.Processes.FirstOrDefault()?.ProcessId ?? 0).StepNumber;
            var result = partsTest.Select(part => new PartViewModel()
            {
                JobID = id,
                PartID = part.PartId,
                ItemCode = part.ItemCode,
                WorkScope = part.WorkScope,
                PartType = part.PartType.Name,
                IdentifyingInfo = part.IdentifyingInfo,
                StepNumber = GetCurrentStep(part.Processes.FirstOrDefault()?.ProcessId ?? 0).StepNumber,//?.Steps.OrderByDescending(x => x.StepNumber).FirstOrDefault()?.StepNumber ?? 1,
                ProcessId = part.Processes.FirstOrDefault()?.ProcessId ?? 0
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_AllJobs()
        {
            return Json(GetAllJobs(), JsonRequestBehavior.AllowGet);
        }


        public virtual List<JobViewModel> GetAllJobs()
        {
            var list = new List<JobViewModel>();
            using (var db = new JDsDBEntities())
            {
                var query = db.Jobs.Where(x => (x.IsActive.HasValue && x.IsActive.Value == true) &&
                                               (x.JobStatusId.HasValue && x.JobStatusId != (int)JobStatus.Cancelled && x.JobStatusId != (int)JobStatus.Closed) &&
                                               (x.Contact != null && x.Contact.Customer != null))
                                   .OrderBy(x => x.ReceivedDate)
                                   .Select(x => new JobViewModel()
                                   {
                                       AllPartsRequirePT = x.AllPartsRequirePT,
                                       AllPartsRequireUT = x.AllPartsRequireUT,
                                       CompanyName = x.Contact.Customer.CompanyName,
                                       CustomerContactId = x.CustomerContactId,
                                       CustomerJobNumber = x.CustomerJobNumber,
                                       FinalPrint = x.Parts.All(p => p.PartFiles.Any(pf => pf.IsFinalPrint.HasValue && pf.IsFinalPrint.Value == true)) ? "yes" : "no",
                                       HoldForCustomerApproval = x.HoldForCustomerApproval,
                                       CreatedByEmployeeId= x.CreatedByEmployeeId,
                                       JobID = x.JobId,
                                       JobNotes = x.JobNotes,
                                       OvertimeRequired = x.OvertimeRequired,
                                       PurchaseOrderNumber = x.PurchaseOrderNumber,
                                       QuoteOnly = x.QuoteOnly,
                                       ReceivedDate = x.ReceivedDate,
                                       RequiredDate = x.RequiredDate,
                                       ShipByDate = x.ShipByDate,
                                       Header = "JD#:" + x.JobId,
                                       JobStatus = x.JobStatu.Name,

                                   });

                list = query.ToList();
            }
            return list;
        }

        public ActionResult Read_Parts(int JobId)
        {
            var parts = GetParts(JobId);
            return Json(parts, JsonRequestBehavior.AllowGet);
        }


        public virtual List<PartViewModel> GetParts(int JobId)
        {
            List<PartViewModel> pvms = new List<PartViewModel>();

            using (JDsDBEntities db = new JDsDBEntities())
            {
                var ptRequired = "PT Required";
                var utRequired = "UT Required";
                var nothingRequired = "";

                var partQuery = db.Parts.Where(x => (x.JobId == JobId) &&
                                                    (x.IsActive == true))
                                                    .AsEnumerable()
                                        .Select(x => new PartViewModel()
                                        {
                                            PartID = x.PartId,
                                            JobID = x.JobId.Value,
                                            WorkScope = x.WorkScope,
                                            ShipByDate = x.ShipByDate,
                                            PartStatusName = x.PartStatu == null ? "n/a" : x.PartStatu.Name,
                                            PartTypeName = x.PartType == null ? "n/a" : x.PartType.Name,
                                            CurrentStep = $"{x.Processes?.FirstOrDefault()?.Steps?.OrderByDescending(s => s.StepNumber).FirstOrDefault()?.Title}",
                                            StepNumber = x.Processes?.FirstOrDefault()?.Steps?.OrderByDescending(s => s.StepNumber).FirstOrDefault()?.StepNumber == null ? 0 : (int)(x.Processes?.FirstOrDefault()?.Steps?.OrderByDescending(s => s.StepNumber).FirstOrDefault()?.StepNumber),
                                            PTUTDesc = $"{((x.RequiresPT.HasValue && x.RequiresPT.Value == true) ? ptRequired : nothingRequired) } {((x.RequiresUT.HasValue && x.RequiresUT.Value == true) ? utRequired : nothingRequired) }",
                                            ProcessId = x.PartProcessId.HasValue ? (int)x.PartProcessId : 0,
                                            PartStatus = x.PartStatu == null ? "n/a" : x.PartStatu.Name,
                                        });

                pvms = partQuery.ToList();
                return pvms;
            }
        }
    }
}
