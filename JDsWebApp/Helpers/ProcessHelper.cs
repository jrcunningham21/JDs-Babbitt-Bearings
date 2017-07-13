using System;
using System.Collections.Generic;
using System.Linq;
using JDsDataModel;
using JDsDataModel.ViewModels;
using JDsDataModel.ViewModels.Processes;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using Newtonsoft.Json;
using NLog;

namespace JDsWebApp.Helpers
{
    public static class ProcessHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Process GetAndValidateProcessExists(JDsDBEntities db, long processId)
        {
            var model = db.Processes.
                Include("Part").
                Include("Part.PartType").
                Include("Part.Contact").
                Include("Part.Contact.Customer").
                Include("Steps").
                FirstOrDefault(x => x.ProcessId == processId);

            if (model == null)
            {
                var msg = $"Unable to find process for ProcessId={processId}";
                _logger.Error(msg);
                //throw new ArgumentException(msg);
            }

            return model;
        }

        public static Step GetStepFromProcesses(JDsDBEntities db, ICollection<Process> processes, long stepId)
        {
            var partProcessIds = processes.Select(x => x.ProcessId).ToList();
            return db.Steps.FirstOrDefault(x => x.StepNumber == 1 && partProcessIds.Contains(x.ProcessId));
        }

        public static void PopulateStepHeaderInfo(Process process, StepViewModel viewmodel)
        {
            if (process == null || viewmodel == null)
            {
                return;
            }

            viewmodel.ProcessId = process.ProcessId;
            viewmodel.PartId = process.PartId;

            viewmodel.ContactLabel = (process == null || process.Part == null || process.Part.Contact == null)
                ? "No Contact"
                : $"{process.Part.Contact.FirstName} {process.Part.Contact.LastName}: {process.Part.Contact.WorkPhone}";

            viewmodel.CustomerLabel = (process == null || process.Part == null || process.Part.Contact == null ||
                                       process.Part.Contact.Customer == null)
                ? $"# {process.Part.JobId}"
                : $"# {process.Part.JobId} {process.Part.Contact.Customer.CompanyName}";

            viewmodel.JobSummary = process.Name;
            viewmodel.JobId = process.Part.JobId.Value;
        }
        
        #region BB

        public static BB1_IncomingInspectionViewModel Generate_BB1_IncomingInspectionViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 1);
            var viewmodel = new BB1_IncomingInspectionViewModel();
            
            if (step != null)   //They've been here before
            {
                try
                {
                    if(step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Unable to deserialize BB1_IncomingInspectionViewModel for ProcessId={0} StepId={1} Version={2}: {3}",
                        id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)   //They've never been here before
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 1;
                step.ProcessId = process.ProcessId;
                step.Title = "Incoming Inspection";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB2_PrecastRoughoutViewModel Generate_BB2_PrecastRoughoutViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 2);
            var viewmodel = new BB2_PrecastRoughoutViewModel();

            if (step != null)
            {
                try
                {
                    if(step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB2_PrecastRoughoutViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Unable to deserialize BB2_PrecastRoughoutViewModel for ProcessId={0} StepId={1} Version={2}: {3}",
                        id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 2;
                step.ProcessId = process.ProcessId;
                step.Title = "Pre-cast Roughout";
                step.DataType = "viewmodel";
                
                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            // Base material is carried over from step 1
            var step1 = process.Steps.FirstOrDefault(x => x.StepNumber == 1);

            if (step1 != null)
            {
                try
                {
                    var vmStep1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                    viewmodel.BaseMaterial = vmStep1.Material;
                }
                catch (Exception ex)
                {
                    // just eat it.... TODO versioning.
                }
            }
            
            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB3_SpincastProcessViewModel Generate_BB3_SpincastProcessViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 3);
            var viewmodel = new BB3_SpincastProcessViewModel();
            
            if (step != null)
            {
                try
                {
                    if(step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB3_SpincastProcessViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                    viewmodel.InitJobInfoFromProcessId(id);
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Unable to deserialize BB3_SpincastProcessViewModel for ProcessId={0} StepId={1} Version={2}: {3}",
                        id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 3;
                step.ProcessId = process.ProcessId;
                step.Title = "Spincast Process";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            // Base material is carried over from step 1
            var step1 = process.Steps.FirstOrDefault(x => x.StepNumber == 1);

            if (step1 != null)
            {
                try
                {
                    var vmStep1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                    viewmodel.BaseMaterial = vmStep1.Material;
                }
                catch (Exception ex)
                {
                    // just eat it.... TODO versioning.
                }
            }
            
            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB4_PostcastCleanupViewModel Generate_BB4_PostcastCleanupViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 4);
            var viewmodel = new BB4_PostcastCleanupViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB4_PostcastCleanupViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Unable to deserialize BB4_PostcastCleanupViewModel for ProcessId={0} StepId={1} Version={2}: {3}",
                        id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 4;
                step.ProcessId = process.ProcessId;
                step.Title = "Postcast Cleanup";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB5_PostcastRoughoutViewModel Generate_BB5_PostcastRoughoutViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 5);
            var viewmodel = new BB5_PostcastRoughoutViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB5_PostcastRoughoutViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Unable to deserialize BB5_PostcastRoughoutViewModel for ProcessId={0} StepId={1} Version={2}: {3}",
                        id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 5;
                step.ProcessId = process.ProcessId;
                step.Title = "Post-cast Roughout";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }

            }

            // Insulation and incoming IDs is carried over from step 1 
            var step1 = process.Steps.FirstOrDefault(x => x.StepNumber == 1);

            if (step1 != null)
            {
                try
                {
                    var vmStep1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                    viewmodel.ODInfo = vmStep1.Insulation;
                    viewmodel.IID1Measurements = vmStep1.ID1Measurements;
                    viewmodel.IID2Measurements = vmStep1.ID2Measurements;
                    viewmodel.IOD1Measurements = vmStep1.OD1Measurements;
                    viewmodel.IOD2Measurements = vmStep1.OD2Measurements;
                }
                catch (Exception ex)
                {
                    // just eat it.... TODO versioning.
                }
            }

            viewmodel.CustIdBore = "N/A";
            viewmodel.CustIdBoreHoriz = "N/A";
            // Check for info from the customer sizes
            try
            {
                var customerSize = db.Sizes.Where(x => x.PartId == process.PartId).FirstOrDefault();
                viewmodel.CustIdBore = customerSize.BoreSize.Length > 0 ? customerSize.BoreSize : "N/A";
                viewmodel.CustIdBoreHoriz = customerSize.BoreSizeHorizontal.Length > 0 ? customerSize.BoreSizeHorizontal : "N/A";
            }
            catch (Exception e)
            {
                _logger.Trace("Error getting customer size: " + e.Message);
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB6_InsulationProcessViewModel Generate_BB6_InsulationProcessViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 6);
            var viewmodel = new BB6_InsulationProcessViewModel();
            
            if (step != null)
            {
                try
                {
                    if(step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB6_InsulationProcessViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB6_InsulationProcessViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 6;
                step.ProcessId = process.ProcessId;
                step.Title = "Insulation Process";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB7_CleanupProcessViewModel Generate_BB7_CleanupProcessViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 7);
            var viewmodel = new BB7_CleanupProcessViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB7_CleanupProcessViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB7_CleanupProcessViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 7;
                step.ProcessId = process.ProcessId;
                step.Title = "Cleanup process";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }

            }

            try
            {
                // If there was a 'slinger ring cutout by' sign off from step 6, propagate it here
                var bb6Data = db.Steps.FirstOrDefault(x => x.StepNumber == 6 && x.ProcessId == id)?.StringValue;

                var jsonObj = JsonConvert.DeserializeObject<BB6_InsulationProcessViewModel>(bb6Data);
                var slingerRingCutOutBySignoff = jsonObj.SlingerRingCutOutBy;
                if (!string.IsNullOrEmpty(slingerRingCutOutBySignoff))
                    viewmodel.SlingerRingCutOutBy = slingerRingCutOutBySignoff;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating BB& vm: {ex.Message}");
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB8_FinalMachineInspectionViewModel Generate_BB8_FinalMachineInspectionViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 8);
            var viewmodel = new BB8_FinalMachineInspectionViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB8_FinalMachineInspectionViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                    
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB8_FinalMachineInspectionViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 8;
                step.ProcessId = process.ProcessId;
                step.Title = "Final Machine Inspection";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);

            // The Bond Verified is tied to the Ultrasonic testing.  If UT is done and passed, auto-mark the Bond Verified toggle
            try
            {
                var part = db.Parts.Where(x => x.PartId == process.PartId).FirstOrDefault();
                viewmodel.JobId = part.JobId.HasValue ? part.JobId.Value : 0;               

                var partTest = db.PartTests.Where(x => x.PartId == part.PartId).FirstOrDefault();
                if (part.RequiresUT.HasValue && part.RequiresUT.Value == true 
                    && partTest.UTComplete.HasValue && partTest.UTComplete.Value == true 
                    && partTest.UTPassed.HasValue && partTest.UTPassed.Value == true)
                {
                    viewmodel.IsBondVerified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error looking up UT for process {id}: {ex.Message}");
            }

            // We need to know if this part is insulated, as described in BB1
            try
            {
                var step1 = db.Steps.Where(x => x.ProcessId == id && x.StepNumber == 1).FirstOrDefault();
                var bb1vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                viewmodel.IsPartInsulated = bb1vm.IsInsulated;
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error looking up insulation on process {id}: {ex.Message}");
            }
            return viewmodel;
        }

        public static BB9_FinishBoreProcessViewModel Generate_BB9_FinishBoreProcessViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 9);
            var viewmodel = new BB9_FinishBoreProcessViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB9_FinishBoreProcessViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB9_FinishBoreProcessViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 9;
                step.ProcessId = process.ProcessId;
                step.Title = "Finish Bore Process";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);

            // The shaft,clearance,shim,tolerance,bore,bore(h),and od size are all from customer sizes
            try
            {
                // Find the associated customer size
                var custSize = db.Sizes.FirstOrDefault(x => x.PartId == viewmodel.PartId);
                viewmodel.ShaftSize = custSize.Shaft;
                viewmodel.ClearanceSize = custSize.Clearance;
                viewmodel.ShimSize = custSize.ShimSize;
                viewmodel.Tolerance = custSize.Tolerance;
                viewmodel.CustomerBoreSize = custSize.BoreSize;
                viewmodel.CustomerBoreSizeHorizontal = custSize.BoreSizeHorizontal;
                viewmodel.Notes = custSize.SpecialNotes;

                var measurementList = db.MeasurementLists.FirstOrDefault(x => x.MeasurementListId == custSize.CustomerSuppliedMeasurementsListId);
                var measurements = db.PartDiameterMeasurements.Where(x => x.MeasurementListId == measurementList.MeasurementListId).ToList();

                viewmodel.CustomerODSize = measurements;
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error getting cust sizes from Generate_BB9_FinishBoreProcessViewModel for process {id}: {ex.Message}");
            }
            return viewmodel;
        }

        public static BB10_FinalAssemblyViewModel Generate_BB10_FinalAssemblyViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 10);
            var viewmodel = new BB10_FinalAssemblyViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB10_FinalAssemblyViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB10_FinalAssemblyViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 10;
                step.ProcessId = process.ProcessId;
                step.Title = "Final Assembly";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            // Parts & Accessories is carried over from step 1
            var step1 = process.Steps.FirstOrDefault(x => x.StepNumber == 1);

            if (step1 != null)
            {
                var originalParts = viewmodel.PartsAccessories;
                try
                {
                    var vmStep1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                    viewmodel.PartsAccessories = vmStep1.PartsAccessories;

                    // If an AR Pin was specified, we should add it to the parts list to make sure it gets reattached
                    if (vmStep1.HasARPin)
                    {
                        viewmodel.PartsAccessories.Add(new IncomingInspectionAccessory()
                        {
                            Name = "AR Pin",
                            Quantity = 1
                        });
                    }

                    foreach (var item in viewmodel.PartsAccessories)
                    {
                        item.IsInstalled = originalParts.SingleOrDefault(x => x.IncomingInspectionAccessoryId == item.IncomingInspectionAccessoryId)?.IsInstalled ?? false;
                    }
                }
                catch (Exception ex)
                {
                    // just eat it.... TODO versioning.
                }
            }


            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        public static BB11_FinalInspectionViewModel Generate_BB11_FinalInspectionViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 11);
            var viewmodel = new BB11_FinalInspectionViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB11_FinalInspectionViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB11_FinalInspectionViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 11;
                step.ProcessId = process.ProcessId;
                step.Title = "Final Inspection";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
           
            // Pull the incoming inspection is insulated value
            try
            {
                var step1 = db.Steps.Where(x => x.ProcessId == id && x.StepNumber == 1).FirstOrDefault();
                var bb1vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(step1.StringValue);
                viewmodel.IsInsulated = bb1vm.IsInsulated;
                viewmodel.HasTC = bb1vm.IsTC;

                // Get the ar info from inc inspection
                viewmodel.HasARPin = bb1vm.HasARPin;
                viewmodel.ARPinDepth = bb1vm.ARPinDepth.HasValue ? bb1vm.ARPinDepth.Value : 0;
                viewmodel.ARPinDiameter = bb1vm.ARPinDiameter.HasValue ? bb1vm.ARPinDiameter.Value : 0;
                viewmodel.OverallLength = bb1vm.OverallLength ?? 0;
                viewmodel.TCInstalledQuantity = bb1vm.TCQuantity ?? 0;

                //IF the part was defined as non-insulated, then carry the OD values entered on workflow step BB8 and make the ODs read-only.
                if (viewmodel.IsInsulated == false)
                {
                    var step8 = db.Steps.Where(x => x.ProcessId == id && x.StepNumber == 8).FirstOrDefault();
                    var bb8vm = JsonConvert.DeserializeObject<BB8_FinalMachineInspectionViewModel>(step8.StringValue);

                    viewmodel.OD1Measurements = bb8vm.OD1Measurements;
                    viewmodel.OD2Measurements = bb8vm.OD2Measurements;
                }

                // (IF the part was defined as insulated, then require the user to enter the values here, on this page)

                viewmodel.JobId = process.Part.JobId.Value;
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error getting inc info in Generate_BB11_FinalInspectionViewModel for process {id}: {ex.Message}");
            }

            return viewmodel;
        }

        public static BB12_DeliveryViewModel Generate_BB12_DeliveryViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 12);
            var viewmodel = new BB12_DeliveryViewModel();
     
            if (step != null)
            {
                try
                {
                    viewmodel = JsonConvert.DeserializeObject<BB12_DeliveryViewModel>(step.StringValue);
                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                    viewmodel.RequiredDate = DateTime.Today.Date;
                    viewmodel.ShipDate = DateTime.Today;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB12_DeliveryViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 12;
                step.ProcessId = process.ProcessId;
                step.Title = "Delivery";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);

            // Get the date shipped and the job id
            try
            {
                var job = process.Part.Job;
                
                viewmodel.RequiredDate = job.RequiredDate;
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error getting Generate_BB12_DeliveryViewModel step dates for process {id}: {ex.Message}");
            }

            return viewmodel;
        }

        public static BB13_FinalSignOffViewModel Generate_BB13_FinalSignOffViewModel(JDsDBEntities db, long id)
        {
            var process = ProcessHelper.GetAndValidateProcessExists(db, id);
            var step = process.Steps.FirstOrDefault(x => x.StepNumber == 13);
            var viewmodel = new BB13_FinalSignOffViewModel();
            
            if (step != null)
            {
                try
                {
                    if (step.StringValue != null)
                        viewmodel = JsonConvert.DeserializeObject<BB13_FinalSignOffViewModel>(step.StringValue);

                    viewmodel.StepId = step.StepId;
                    viewmodel.ProcessId = process.ProcessId;
                    viewmodel.JobId = process.Part == null || process.Part.JobId == null ? 0 : process.Part.JobId.Value;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to deserialize BB13_FinalSignOffViewModel for ProcessId={0} StepId={1} Version={2}: {3}", id, step.StepId, step.Version, ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            }

            if (step == null)
            {
                step = new Step();
                step.Created = DateTime.UtcNow;
                step.StepNumber = 13;
                step.ProcessId = process.ProcessId;
                step.Title = "Final Sign Off";
                step.DataType = "viewmodel";

                try
                {
                    db.Steps.Add(step);
                    db.SaveChanges();

                    viewmodel.StepId = step.StepId;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw;
                }
            }

            PopulateStepHeaderInfo(process, viewmodel);
            viewmodel.InitJobInfoFromProcessId(id);
            return viewmodel;
        }

        #endregion
    }
}