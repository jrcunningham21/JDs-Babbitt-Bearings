using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JDsDataModel;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using Kendo.Mvc.Extensions;
using JDsWebApp.Helpers;
using Newtonsoft.Json;
using NLog;
using Kendo.Mvc.UI;
using JDsDataModel.ViewModels.Processes;
using JDsDataModel.ViewModels;
using System.Collections.Generic;

namespace JDsWebApp.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ProcessBabbitBearingController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        public ActionResult Index(int partId = 0)
        {
            return BB1_IncomingInspection(partId);
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 1)]
        [HttpGet]
        public ActionResult GetProcessIdFromPartId(int partId)
        {
            // Looks up the process if it exists, or creates it if it does not
            using (var db = new JDsDBEntities())
            {
                var part = db.Parts.FirstOrDefault(x => x.PartId == partId);
                var process = db.Processes.FirstOrDefault(x => x.PartId == partId);
                if (process == null)
                {
                    process = new Process()
                    {
                        Name = string.Format("{0} - {1}", "Babbit Bearing", part.IdentifyingInfo),
                        Description = string.Empty,
                        IsActive = true,
                        PartId = part.PartId, 
                        ProcessTypeId = 1
                    };

                    db.Processes.Add(process);
                    db.SaveChanges();
                }

                return Json(process.ProcessId, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 1)]
        [HttpPost]
        public ActionResult ViewCustomerSizes(SizesViewModel sizesVM)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var viewModel = new SizesViewModel();
                var process = db.Processes.First(x => x.PartId == sizesVM.PartID);
                var vmString = db.Steps.Where(x => x.ProcessId == process.ProcessId && x.StepNumber == 1).FirstOrDefault().StringValue;
                var bb1Vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(vmString);

                var size = db.Sizes.Where(x => x.PartId == sizesVM.PartID).FirstOrDefault();
                if (size == null)
                    size = new Size();

                viewModel.PartID = sizesVM.PartID;

                viewModel.Shaft = size.Shaft;
                viewModel.Clearance = size.Clearance;
                viewModel.BoreSize = size.BoreSize;
                viewModel.BoreSizeHorizontal = size.BoreSizeHorizontal;
                viewModel.ShimSize = size.ShimSize;
                viewModel.Tolerance = size.Tolerance;
                viewModel.SealSize = size.SealSize;
                viewModel.ODSizes = new List<PartDiameterMeasurement>();  // will get populated in separate ajax call
                viewModel.SpecialNotes = size.SpecialNotes;

                return PartialView(viewModel);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 1)]
        [HttpPost]
        //public ActionResult VerifyCustomerSizes(int partId, List<BoreMeasurementViewModel> od1, List<BoreMeasurementViewModel> od2)
        public ActionResult VerifyCustomerSizes(VerifyCustomerSizesViewModel incVm)
        {
            using (var db = new JDsDBEntities())
            {
                var viewModel = new VerifyCustomerSizesViewModel();

                var process = db.Processes.First(x => x.PartId == incVm.PartID);
                var vmString = db.Steps.Where(x => x.ProcessId == process.ProcessId && x.StepNumber == 1).FirstOrDefault().StringValue;
                var bb1Vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(vmString);

                var size = db.Sizes.Where(x => x.PartId == incVm.PartID).FirstOrDefault();
                if (size == null)
                    size = new Size();

                viewModel.IsPartInsulated = bb1Vm.IsInsulated;
                viewModel.PartID = incVm.PartID;
                viewModel.VerifyBoreSize = size.BoreSize;
                viewModel.VerifyBoreSizeHorizontal = size.BoreSizeHorizontal;
                viewModel.VerifyClearance = size.Clearance;
                viewModel.VerifySealSize = size.SealSize;
                viewModel.VerifyShaft = size.Shaft;
                viewModel.VerifyShimSize = size.ShimSize;
                viewModel.VerifyTolerance = size.Tolerance;
                viewModel.VerifyODSizes = new List<PartDiameterMeasurement>();  // will get populated in separate ajax call

                // Pull the incoming IDs from incoming inspection
                try
                {
                    viewModel.VerifyIncomingID1Sizes = new System.Collections.Generic.List<JDsDataModel.ViewModels.BoreMeasurementViewModel>();
                    viewModel.VerifyIncomingID2Sizes = new System.Collections.Generic.List<JDsDataModel.ViewModels.BoreMeasurementViewModel>();
                    viewModel.VerifyIncomingOD1Sizes = new System.Collections.Generic.List<JDsDataModel.ViewModels.BoreMeasurementViewModel>();
                    viewModel.VerifyIncomingOD2Sizes = new System.Collections.Generic.List<JDsDataModel.ViewModels.BoreMeasurementViewModel>();                    

                    // Get the incoming ID from inocming inspection
                    foreach (var element in bb1Vm.ID1Measurements)
                        viewModel.VerifyIncomingID1Sizes.Add(new JDsDataModel.ViewModels.BoreMeasurementViewModel() { A = element.A, B = element.B, C = element.C });

                    foreach (var element in bb1Vm.ID2Measurements)
                        viewModel.VerifyIncomingID2Sizes.Add(new JDsDataModel.ViewModels.BoreMeasurementViewModel() { A = element.A, B = element.B, C = element.C });

                    // If part is non-insulated, use the OD measurements from BB8 OD grids given in the parameters
                    if (!viewModel.IsPartInsulated)
                    {
                        if (incVm.VerifyIncomingOD1Sizes != null)
                            viewModel.VerifyIncomingOD1Sizes.AddRange(incVm.VerifyIncomingOD1Sizes);
                        if (incVm.VerifyIncomingOD2Sizes != null)
                            viewModel.VerifyIncomingOD2Sizes.AddRange(incVm.VerifyIncomingOD2Sizes);
                    }
                    else
                    {
                        // Otherwise, pull the ones from incoming inspection
                        foreach (var element in bb1Vm.OD1Measurements)
                            viewModel.VerifyIncomingOD1Sizes.Add(new JDsDataModel.ViewModels.BoreMeasurementViewModel() { A = element.A, B = element.B, C = element.C });

                        foreach (var element in bb1Vm.OD2Measurements)
                            viewModel.VerifyIncomingOD2Sizes.Add(new JDsDataModel.ViewModels.BoreMeasurementViewModel() { A = element.A, B = element.B, C = element.C });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Error getting incoming IDs from parg {incVm.PartID}: {ex.Message}");
                }


                try
                {
                    // Try to set the job ID
                    viewModel.JobID = db.Parts.First(x => x.PartId == incVm.PartID).JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Error verifying customer sizes view model for part {incVm.PartID}: {ex.Message}");
                }

                return PartialView(viewModel);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 1)]
        public ActionResult PartTestView(int id)
        {
            using (var db = new JDsDBEntities())
            {
                // find the corresponding part test, if any
                var partTest = db.PartTests.FirstOrDefault(x => x.PartId == id);
                if (partTest == null)
                    partTest = new PartTest();
                var part = db.Parts.Find(id);

                var viewModel = new PartTestViewModel();
                viewModel.PartId = id;
                viewModel.RequiresPT = part.RequiresPT.HasValue ? part.RequiresPT.Value : false;
                viewModel.PTComplete = !string.IsNullOrEmpty(partTest.PTSignoffEmpName);
                viewModel.RequiresUT = part.RequiresUT.HasValue ? part.RequiresUT.Value : false;
                viewModel.UTComplete = !string.IsNullOrEmpty(partTest.UTSignoffEmpName);
                viewModel.PTSignoffEmployee = partTest.PTSignoffEmpName;
                viewModel.UTSignoffEmployee = partTest.UTSignoffEmpName;
                viewModel.TestNotes = partTest.TestNotes;
                if (partTest.PTPassed.HasValue) viewModel.PTPassed = partTest.PTPassed.Value;
                if (partTest.UTPassed.HasValue) viewModel.UTPassed = partTest.UTPassed.Value;
                viewModel.ProcessId = (int)db.Processes.FirstOrDefault(x => x.PartId == id).ProcessId;
                return PartialView(viewModel);
            }
        }

        [HttpPost]
        public ActionResult SavePartTest(PartTestViewModel viewModel)
        {
            using (var db = new JDsDBEntities())
            {
                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, PartialView("PartTestView", viewModel).ToString());
                }

                var partTest = db.PartTests.FirstOrDefault(x => x.PartId == viewModel.PartId);
                if (partTest== null)
                {
                    partTest = new PartTest();
                    db.PartTests.Add(partTest);
                }

                bool signOffCompleted = !string.IsNullOrEmpty(viewModel.PTSignoffEmployee) || !string.IsNullOrEmpty(viewModel.UTSignoffEmployee);

                partTest.PartId = viewModel.PartId;
                // Then we're doing a PT test this time
                if (!string.IsNullOrEmpty(viewModel.PTSignoffEmployee))
                {
                    partTest.PTComplete = !string.IsNullOrEmpty(viewModel.PTSignoffEmployee);
                    if (!string.IsNullOrEmpty(viewModel.PTSignoffEmployee)) // don't overwrite it with null
                        partTest.PTSignoffEmpName = viewModel.PTSignoffEmployee;
                }

                if (!string.IsNullOrEmpty(viewModel.UTSignoffEmployee))
                {
                    // Then we're doing a UT test this time
                    partTest.UTComplete = !string.IsNullOrEmpty(viewModel.UTSignoffEmployee);
                    if (!string.IsNullOrEmpty(viewModel.UTSignoffEmployee)) // don't overwrite it with null
                        partTest.UTSignoffEmpName = viewModel.UTSignoffEmployee;
                }

                bool needToRollBack = false;
                if (viewModel.TestType == "PT" && !viewModel.PTPassed)
                {
                    partTest.PTPassed = viewModel.PTPassed;
                    needToRollBack = true;
                }
                if (viewModel.PTPassed)
                    partTest.PTPassed = true;

                if (viewModel.TestType == "UT" && !viewModel.UTPassed)
                {
                    partTest.UTPassed = viewModel.UTPassed;
                    needToRollBack = true;
                }
                if (viewModel.UTPassed)
                    partTest.UTPassed = true;

                if (!string.IsNullOrEmpty(viewModel.TestNotes))
                    partTest.TestNotes = viewModel.TestNotes;

                int rollBackToStepNumber = viewModel.ReturnStep;

                db.SaveChanges();

                // clean things up
                if (needToRollBack)
                {
                    RollPartBackToStep(viewModel.PartId, rollBackToStepNumber);
                }

                return Json(signOffCompleted, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB1_IncomingInspection(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(_db, id);
            return View(viewmodel);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AddEditIncomingAccessory(int incomingAccessoryId, int partId)
        {
            var vm = new IncomingInspectionAccessoryViewModel();
            vm.PartId = partId;
            return PartialView(vm);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult SaveIncomingAccessory(IncomingInspectionAccessoryViewModel viewModel)
        {
            var matchingAccessory = _db.IncomingInspectionAccessories.FirstOrDefault(x => x.IncomingInspectionAccessoryId == viewModel.IncomingInspectionAccessoryId);

            if (matchingAccessory == null)
            {
                matchingAccessory = new IncomingInspectionAccessory();
                _db.IncomingInspectionAccessories.Add(matchingAccessory);
            }

            matchingAccessory.Name = viewModel.Name;
            matchingAccessory.PartId = viewModel.PartId;
            matchingAccessory.Quantity = viewModel.Quantity;

            _db.SaveChanges();

            // Return the id of the newly added accessory
            return Json(matchingAccessory.IncomingInspectionAccessoryId, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadPartAccessories(int id)
        {
            var accessories = _db.IncomingInspectionAccessories.Where(x => x.PartId == id).ToList();

            var result = accessories.Select(x => new IncomingInspectionAccessoryViewModel()
            {
                IncomingInspectionAccessoryId = x.IncomingInspectionAccessoryId,
                IsInstalled = x.IsInstalled,
                Name = x.Name,
                Quantity = x.Quantity,
                PartId = x.PartId ?? 0,
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult DeleteIncomingAccessory(int incomingInspectionAccessoryId)
        {
            var matchingAccessory = _db.IncomingInspectionAccessories.FirstOrDefault(x => x.IncomingInspectionAccessoryId == incomingInspectionAccessoryId);
            if (matchingAccessory != null)
                _db.IncomingInspectionAccessories.Remove(matchingAccessory);

            _db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB1_IncomingInspection(BB1_IncomingInspectionViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB1_IncomingInspection", viewModel);
            }
            
            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", ""); // Work around for now.
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            // Save the part notes in the Part table, too
            var matchingPart = _db.Parts.FirstOrDefault(x => x.PartId == viewModel.PartId);
            if (matchingPart != null)
            {
                matchingPart.NonConformanceReportNotes = viewModel.PartNotes;
            }

            // Set the part status and job status. If status is in "Not Started" move it to "In progress"
            // Same with the Job status.
            try
            {
                // find the job
                var job = _db.Jobs.FirstOrDefault(x => x.JobId == matchingPart.JobId);
                var currentJobStatusId = job.JobStatusId.HasValue ? (JobStatus)job.JobStatusId : JobStatus.Unknown;

                // Update the job status
                if (currentJobStatusId == JobStatus.New || currentJobStatusId == JobStatus.Unknown)
                {
                    job.JobStatusId = (int)JobStatus.InProgress;
                    StatusChangeHelper.PublishJobStatusChange(job.JobId, job.JobStatusId.Value);
                }

                // Update the part status
                var currentPartStatus = matchingPart.PartStatusId.HasValue ? (PartStatus)matchingPart.PartStatusId : PartStatus.Unknown;
                if (currentPartStatus == PartStatus.NotStarted || currentPartStatus == PartStatus.Unknown)
                {
                    matchingPart.PartStatusId = (int)PartStatus.InProgress;
                    StatusChangeHelper.PublishPartStatusChange(job.JobId, matchingPart.PartId, matchingPart.PartStatusId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.Trace("Error setting job status: " + ex.Message);
            }
            
            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }
            
            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB2_PrecastRoughout(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB2_PrecastRoughoutViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB2_PrecastRoughout(BB2_PrecastRoughoutViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                return PartialView("BB2_PrecastRoughout", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB3_SpincastProcess(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB3_SpincastProcessViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB3_SpincastProcess(BB3_SpincastProcessViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB3_SpincastProcess", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB4_PostcastCleanup(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB4_PostcastCleanupViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB4_PostcastCleanup(BB4_PostcastCleanupViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB4_PostcastCleanup", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB5_PostcastRoughout(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB5_PostcastRoughoutViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB5_PostcastRoughout(BB5_PostcastRoughoutViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB5_PostcastRoughout", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            bool isInsulated = false;
            try
            {
                // Let the view know if it should go to the next step, or skip the next step.  Insulated jobs go to Insulation.  Non-insulated jobs skip
                var bb1Data = _db.Steps.FirstOrDefault(x => x.StepNumber == 1 && x.ProcessId == viewModel.ProcessId)?.StringValue;

                var jsonObj = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(bb1Data);
                isInsulated = jsonObj.IsInsulated;
            }
            catch (Exception ex)
            {
                _logger.Trace("Error getting BB1 insulated data: " + ex.Message);
            }
            var retVal = new
            {
                isPartInsulated = isInsulated
            };

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB6_InsulationProcess(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB6_InsulationProcessViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB6_InsulationProcess(BB6_InsulationProcessViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB6_InsulationProcess", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB7_CleanupProcess(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB7_CleanupProcessViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB7_CleanupProcess(BB7_CleanupProcessViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB7_CleanupProcess", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB8_FinalMachineInspection(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB8_FinalMachineInspectionViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB8_FinalMachineInspection(BB8_FinalMachineInspectionViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB8_FinalMachineInspection", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            var employee = _db.Employees.SingleOrDefault(x => x.Name == viewModel.SizedApprovedBy);
            var process = _db.Processes.Single(x => x.ProcessId == viewModel.ProcessId);

            if (employee != null && employee.Skills.Any(x => x.Name == "Management") && process.Part.PartStatu.Name == "Blocked")
            {
                process.Part.PartStatusId = _db.PartStatus.Single(x => x.Name == "In Progress").PartStatusId;
                _db.Entry(process.Part).State = EntityState.Modified;

                var job = _db.Jobs.Single(x => x.JobId == viewModel.JobId);

                if (job.Parts.All(x => x.PartStatu.Name != "Blocked"))
                {
                    job.JobStatusId = _db.JobStatus.Single(x => x.Name == "In Progress").JobStatusId;
                    _db.Entry(job).State = EntityState.Modified;
                }
            }

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB9_FinishBoreProcess(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB9_FinishBoreProcessViewModel(_db, id);
            
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB9_FinishBoreProcess(BB9_FinishBoreProcessViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB9_FinishBoreProcess", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB10_FinalAssembly(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB10_FinalAssemblyViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB10_FinalAssembly(BB10_FinalAssemblyViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB10_FinalAssembly", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB11_FinalInspection(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB11_FinalInspectionViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB11_FinalInspection(BB11_FinalInspectionViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB11_FinalInspection", viewModel);
            }

            Part part = _db.Processes.SingleOrDefault(x => x.ProcessId == viewModel.ProcessId)?.Part;

            if (part != null)
            {
                PartTest partTest = _db.PartTests.FirstOrDefault(x => x.PartId == part.PartId);

                if (part.RequiresPT == true || part.RequiresUT == true)
                {
                    if (partTest == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Part test is required before continuing");
                    }

                    if (part.RequiresPT == true && partTest.PTComplete != true)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Penetrant test is required before continuing");
                    }

                    if (part.RequiresUT == true && partTest.UTComplete != true)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UT is required before continuing");
                    }
                }

            }
            //we have issues if this is null


            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB12_Delivery(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB12_DeliveryViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB12_Delivery(BB12_DeliveryViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB12_Delivery", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

            // Update the part and the job accordingly
            try
            {
                var process = _db.Processes.FirstOrDefault(x => x.ProcessId == viewModel.ProcessId);
                var part = process.Part;
                part.ShippedDate = viewModel.ShipDate;  // set the part's ship date
                
                // Check the job and see if all of the job's parts have ship dates
                if (part.Job.Parts.All(x => x.ShippedDate.HasValue))
                {
                    part.Job.ShippedDate = viewModel.ShipDate;
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error updating part and job with dates: {ex.Message}");
            }

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BB13_FinalSignOff(long id = 0)
        {
            var viewmodel = ProcessHelper.Generate_BB13_FinalSignOffViewModel(_db, id);
            return View(viewmodel);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Save_BB13_FinalSignOff(BB13_FinalSignOffViewModel viewModel)
        {
            // Server side validation
            if (!viewModel.IsOnlySave && !ModelState.IsValid)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return PartialView("BB13_FinalSignOff", viewModel);
            }

            // Create new or edit existing
            var step = viewModel.StepId == 0
                ? new Step()
                : _db.Steps.Include("Process").FirstOrDefault(x => x.StepId == viewModel.StepId);

            step.Version = viewModel.Version.Replace("\"", "");
            step.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);


            // Set the part status and job status. If status is in "Not Started" move it to "In progress"
            // Same with the Job status.
            try
            {
                var matchingPart = _db.Parts.FirstOrDefault(x => x.PartId == viewModel.PartId);

                // find the job
                var job = _db.Jobs.FirstOrDefault(x => x.JobId == matchingPart.JobId);
                var currentJobStatusId = job.JobStatusId.HasValue ? (JobStatus)job.JobStatusId : JobStatus.Unknown;

                // Update the part status
                var currentPartStatus = matchingPart.PartStatusId.HasValue ? (PartStatus)matchingPart.PartStatusId : PartStatus.Unknown;
                if (currentPartStatus == PartStatus.InProgress)
                {
                    matchingPart.PartStatusId = (int)PartStatus.Finished;
                    StatusChangeHelper.PublishPartStatusChange(job.JobId, matchingPart.PartId, matchingPart.PartStatusId.Value);
                }

                // Update the job status - if all the parts of this job are done, mark the job as done
                if (currentJobStatusId == JobStatus.InProgress)
                {
                    if (job.Parts.Where(x => x.IsActive).All(x => x.PartStatusId == (int)PartStatus.Finished))
                        job.JobStatusId = (int)JobStatus.Finished;

                    StatusChangeHelper.PublishJobStatusChange(job.JobId, job.JobStatusId.Value);
                }

            }
            catch (Exception ex)
            {
                _logger.Trace("Error setting part/job status to finished: " + ex.Message);
            }

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            return Content("");
        }
        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SaveFailJob(int partId, string notes)
        {
            var part = _db.Parts.FirstOrDefault(x => x.PartId == partId);
            part.NonConformanceReportNotes = notes;
          
            _db.Entry<Part>(part).State = EntityState.Modified;
            _db.SaveChanges();
        
            return Content("");
        }
        public void RollPartBackToStep(int partId, int newStepNumber)
        {
            // Find and delete the step ids with data >= newStepMember associated with this part
            try
            {
                using (var db = new JDsDBEntities())
                {
                    // Find the process
                    var process = db.Processes.FirstOrDefault(x => x.PartId == partId);
                    var stepsToDelete = db.Steps.Where(x => x.ProcessId == process.ProcessId &&
                        x.StepNumber >= newStepNumber)
                        .OrderBy(x => x.StepNumber);

                    db.Steps.RemoveRange(stepsToDelete);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error rolling back part {partId} to new step {newStepNumber}: {ex.Message}");
            }
        }

        public long GetProcessIdFromPart(int PartId)
        {
            long ProcessID = 0;

            using (JDsDBEntities db = new JDsDBEntities())
            {
                var test = db.Processes.Where(x => x.PartId == PartId).Select(x => x.ProcessId).FirstOrDefault();
                ProcessID = test;
            }
                return ProcessID;
        }
    }
}