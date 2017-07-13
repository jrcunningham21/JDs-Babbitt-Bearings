using JDsDataModel;
using JDsDataModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using Newtonsoft.Json;
using JDsWebApp.Helpers;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace JDsWebApp.Controllers
{
    public class PartController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        #region Part
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult PartDetails(int jobID, int partID, bool? requiresPT, bool? requiresUT)
        {
            var partModel = new PartViewModel();
            partModel.JobID = jobID;
            partModel.PartID = partID;

            if (partID != 0)
            {
                using (var db = new JDsDBEntities())
                {
                    var part = db.Parts.Find(partID);

                    if (part != null)
                    {
                        partModel.PartID = partID;
                        partModel.ItemCode = part.ItemCode;
                        partModel.WorkScope = part.WorkScope;
                        partModel.PartType = part.PartType?.Name;
                        partModel.PartProcesses = db.PartProcesses.ToList();
                        partModel.IdentifyingInfo = part.IdentifyingInfo;
                        partModel.RequiresUT = (bool)part.RequiresUT;
                        partModel.RequiresPT = (bool)part.RequiresPT;
                    }
                }
            }
            else //New Part
            {
                using (var db = new JDsDBEntities())
                {
                    partModel.PartProcesses = db.PartProcesses.ToList();
                }
            }

            return PartialView("PartDetails", partModel);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult PartDetails(PartViewModel partModel)
        {
            if (!ModelState.IsValid)
                return PartialView("PartDetails", partModel);

            using (var db = new JDsDBEntities())
            {
                if (partModel.PartID != 0)
                {
                    var part = db.Parts.Find(partModel.PartID); //Update Existing Part

                    part.ItemCode = partModel.ItemCode;
                    part.WorkScope = partModel.WorkScope;
                    part.IdentifyingInfo = partModel.IdentifyingInfo;
                    part.RequiresPT = partModel.RequiresPT;
                    part.RequiresUT = partModel.RequiresUT;

                    var partType = db.PartTypes.Where(pT => pT.Name == partModel.PartType).FirstOrDefault();
                    if (partType != null)
                        part.PartTypeId = partType.PartTypeId;
                    if (partModel.SelectedPartProcessID != 0)
                        part.PartProcessId = partModel.SelectedPartProcessID;
                }
                else
                {
                    int newPartID = CreatePart();
                    partModel.PartID = newPartID;

                    var part = db.Parts.Find(newPartID);

                    part.PartStatusId = 1; //Not Started - TODO Enum
                    part.ItemCode = partModel.ItemCode;
                    part.WorkScope = partModel.WorkScope;
                    part.IdentifyingInfo = partModel.IdentifyingInfo;
                    part.RequiresPT = partModel.RequiresPT;
                    part.RequiresUT = partModel.RequiresUT;

                    var partType = db.PartTypes.Where(pT => pT.Name == partModel.PartType).FirstOrDefault();
                    if (partType != null)
                        part.PartTypeId = partType.PartTypeId;
                    if (partModel.SelectedPartProcessID != 0)
                        part.PartProcessId = partModel.SelectedPartProcessID;
                }
                db.SaveChanges();
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return PartDetails(partModel.JobID, (int)partModel.PartID, partModel.RequiresPT, partModel.RequiresUT);
        }

        public int CreatePart()
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                Part newPart = new Part();
                db.Parts.Add(newPart);

                db.SaveChanges();

                return newPart.PartId;
            }
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public void FlagPartForCustomerApproval(int partId, string message)
        {
            using (var db = new JDsDBEntities())
            {
                var part = db.Parts.FirstOrDefault(x => x.PartId == partId);
                if (part == null)
                {
                    _logger.Trace("Error flagging part for approval, can't find part: " + partId);
                    return;
                }
                part.PartStatusId = (int)(JDsWebApp.Helpers.PartStatus.Blocked);
                part.NonConformanceReportNotes += $"{Environment.NewLine} Part flagged for customer approval";

                var job = db.Jobs.FirstOrDefault(x => x.JobId == part.JobId);
                if (job == null)
                {
                    _logger.Trace("Error flagging job for approval, can't find job: " + part.JobId);
                    return;
                }
                job.JobStatusId = (int)(JDsWebApp.Helpers.JobStatus.Blocked);

                // Set the job state and the part state accordingly
                db.SaveChanges();

                // notify the world the state has changed
                StatusChangeHelper.PublishPartStatusChange(job.JobId, part.PartId, part.PartStatusId.Value);
                StatusChangeHelper.PublishJobStatusChange(job.JobId, job.JobStatusId.Value);
                SMSHelper.SendSMSToManagers(message);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public void SavePart(PartViewModel part)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var p = db.Parts.Find(part.PartID);
                var process = db.Processes.Where(x => x.PartId == p.PartId).FirstOrDefault();

                if (process == null)
                {
                    Process newProcess = new Process();
                    newProcess.IsActive = true;
                    newProcess.Name = string.Format("{0} - {1}", "Babbit Bearing", part.IdentifyingInfo);
                    newProcess.PartId = part.PartID;
                    newProcess.Description = string.Empty;
                    newProcess.ProcessTypeId = 1;

                    db.Processes.Add(newProcess);                    
                    db.SaveChanges();

                    process = db.Processes.Find(newProcess.ProcessId);
                }

                var step = db.Steps.Where(x => x.ProcessId == process.ProcessId).FirstOrDefault();

                if (step == null) // New Parts initialize to StepNumber = 1 (IncomingInspection)
                {
                    Step newStep = new Step();
                    BB1_IncomingInspectionViewModel viewModel = new BB1_IncomingInspectionViewModel();

                    newStep.ProcessId = process.ProcessId;
                    newStep.StepNumber = 1;
                    newStep.Title = "IncomingInspection";
                    newStep.Created = DateTime.UtcNow;
                    newStep.DataType = "viewmodel";
                    newStep.StringValue = JsonConvert.SerializeObject(viewModel, Formatting.Indented);

                    db.Steps.Add(newStep);
                    db.SaveChanges();

                    step = db.Steps.Find(newStep.StepId);
                }

                p.JobId = part.JobID;
                p.ItemCode = part.ItemCode;
                p.WorkScope = part.WorkScope;

                var partType = db.PartTypes.Where(pT => pT.Name == part.PartType).FirstOrDefault();
                if (partType != null)
                    p.PartTypeId = partType.PartTypeId;
                else
                    p.PartTypeId = 1;

                if (part.SelectedPartProcessID != 0)
                    p.PartProcessId = part.SelectedPartProcessID;

                p.IdentifyingInfo = part.IdentifyingInfo;
                p.RequiresUT = part.RequiresUT;
                p.RequiresPT = part.RequiresPT;
                p.IsActive = true;                

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.Trace("Error saving part: " + ex.Message);
                }

                
            }
        }

        [HttpGet]
        public ActionResult GetAutoCompleteIds(string values, int controlId)
        {            
            string[] strArray = values.Split(',');
            string[] json = new string [strArray.Length];

            using (JDsDBEntities db = new JDsDBEntities())
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    var s = strArray[i];

                    var acID = db.AutoCompletes.Where(x => x.ControlId == controlId && x.Value == s).Select(o => o.AutoCompleteId).FirstOrDefault();

                    json[i] = acID.ToString();
                }
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sizes
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult Sizes(int partID)//, int sizesID)
        {
            // If the given part has a Sizes associated, populate a view model with its values
            // if the given part does not, create a Sizes in the DB and return a blank view model
         
            using (var db = new JDsDBEntities())
            {
                // check for an existing Sizes
                var sizes = db.Sizes.FirstOrDefault(x => x.PartId == partID);
                if (sizes == null)
                {
                    // Create the blank sizes to return
                    sizes = new Size();
                    sizes.PartId = partID;
                    sizes.MeasurementList = new MeasurementList();
                    db.Sizes.Add(sizes);    // do this to get the ID back
                    db.SaveChanges();
                }

                // Populate the view model with what we find in the db
                var sizesModel = new SizesViewModel()
                {
                    BoreSize = sizes.BoreSize,
                    BoreSizeHorizontal = sizes.BoreSizeHorizontal,
                    Clearance = sizes.Clearance,
                    ODSizes = new List<PartDiameterMeasurement>(),
                    PartID = partID,
                    SealSize = sizes.SealSize,
                    Shaft = sizes.Shaft,
                    ShimSize = sizes.ShimSize,
                    SizesID = sizes.SizesId,
                    SpecialNotes = sizes.SpecialNotes,
                    Tolerance = sizes.Tolerance
                };

                return PartialView("Sizes", sizesModel);
            }

        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult Sizes(SizesViewModel sizesModel)
        {
            if (!ModelState.IsValid)
            {
                sizesModel.ODSizes = new List<PartDiameterMeasurement>();
                return PartialView("Sizes", sizesModel);
            }
                

            using (var db = new JDsDBEntities())
            {
                if (sizesModel.SizesID != 0)
                {
                    var sizes = db.Sizes.Find(sizesModel.SizesID); // Update Existing Sizes

                    sizes.Shaft = string.IsNullOrEmpty(sizesModel.Shaft) ? "N/A" : sizesModel.Shaft;
                    sizes.Clearance = string.IsNullOrEmpty(sizesModel.Clearance) ? "N/A" : sizesModel.Clearance;
                    sizes.BoreSize = string.IsNullOrEmpty(sizesModel.BoreSize) ? "N/A" : sizesModel.BoreSize;
                    sizes.BoreSizeHorizontal = string.IsNullOrEmpty(sizesModel.BoreSizeHorizontal) ? "N/A" : sizesModel.BoreSizeHorizontal;
                    sizes.ShimSize = string.IsNullOrEmpty(sizesModel.ShimSize) ? "N/A" : sizesModel.ShimSize;
                    sizes.Tolerance = string.IsNullOrEmpty(sizesModel.Tolerance) ? "N/A" : sizesModel.Tolerance;
                    sizes.SealSize = string.IsNullOrEmpty(sizesModel.SealSize) ? "N/A" : sizesModel.SealSize;
                    sizes.SpecialNotes = sizesModel.SpecialNotes;
                }
                
                db.SaveChanges();
            }
            //return Sizes(sizesModel.PartID);
            return Json(sizesModel.PartID, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetODSizes(int id)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                // Find the sizes
                var sizes = db.Sizes.Where(x => x.PartId == id).FirstOrDefault();

                if (sizes == null)
                {
                    // We have a problem
                    _logger.Trace("Error in GetODSizes sizes for part: " + id);
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                // find its measurement list to get the ODs
                var measurementList = db.MeasurementLists.FirstOrDefault(x => x.MeasurementListId == sizes.CustomerSuppliedMeasurementsListId);

                if (measurementList == null)
                {
                    _logger.Trace("Error in GetODSizes measurementList for part: " + id);
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                // find all of the Part DIameter Measurements in this list
                var measurements = db.PartDiameterMeasurements.Where(x => x.MeasurementListId == measurementList.MeasurementListId).ToList();

                // Transform this to a list of 'A's'
                var retVals = measurements.Select(x => new
                {
                    MeasurementId = x.PartDiameterMeasurementId,
                    A = x.ODComment
                });

                var result = retVals.Select(r => new PartDiameterMeasurement()
                {
                     PartDiameterMeasurementId = r.MeasurementId,
                     ODComment = r.A
                });

                return Json(result, JsonRequestBehavior.AllowGet);

              //  return Json(retVals);
            }
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult GetIncomingIDSizes(int id)  // the id is the part id
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                // this info is serialized in the incoming inspection VM stuff
                // get the process for this part, get step number 1, get string value
                try
                {
                    var process = db.Processes.FirstOrDefault(x => x.PartId == id);
                    var incInspectionStep = db.Steps.FirstOrDefault(x => x.ProcessId == process.ProcessId && x.StepNumber == 1);
                    var bb1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(incInspectionStep.StringValue);
                    var incIds = bb1.ID1Measurements;
                    incIds.AddRange(bb1.ID2Measurements);

                    return Json(incIds, JsonRequestBehavior.AllowGet);
                }
                catch(Exception e)
                {
                    _logger.Trace("Error in getting incoming id sizes for part: " + id + ". msg=" + e.Message);
                    return null;
                }
            }
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult GetIncomingODSizes(int id)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                // this info is serialized in the incoming inspection VM stuff
                // get the process for this part, get step number 1, get string value
                try
                {
                    var process = db.Processes.FirstOrDefault(x => x.PartId == id);
                    var incInspectionStep = db.Steps.FirstOrDefault(x => x.ProcessId == process.ProcessId && x.StepNumber == 1);
                    var bb1 = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(incInspectionStep.StringValue);
                    var incOds = bb1.OD1Measurements;
                    incOds.AddRange(bb1.OD2Measurements);

                    return Json(incOds, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    _logger.Trace("Error in getting incoming od sizes for part: " + id + ". msg=" + e.Message);
                    return null;
                }
            }
        }
        
        public ActionResult DeleteODSize (int partDiameterMeasurementId)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var measurement = db.PartDiameterMeasurements.Find(partDiameterMeasurementId);
                db.PartDiameterMeasurements.Remove(measurement);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AddODSize(int partId, string ODSize)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var sizes = db.Sizes.Where(x => x.PartId == partId).FirstOrDefault();

                if (sizes == null)
                {
                    // We have a problem
                    _logger.Trace("Error in AddODSize sizes for part: " + partId);                    
                }

                // find its measurement list to get the ODs
                var measurementList = db.MeasurementLists.FirstOrDefault(x => x.MeasurementListId == sizes.CustomerSuppliedMeasurementsListId);

                if (measurementList == null)
                {
                    _logger.Trace("Error in AddODSize measurementList for part: " + partId);                    
                }

                var newMeasurement = new PartDiameterMeasurement();
                newMeasurement.MeasurementListId = measurementList.MeasurementListId;
                newMeasurement.ODComment = ODSize;
                newMeasurement.MeasuredDate = DateTime.Now;

                db.PartDiameterMeasurements.Add(newMeasurement);
                db.SaveChanges();


                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Part Info pop-up

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult Debug_MJ()
        {

            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult PartInfoPopUp(int partID, int tabIndex)
        {
            PartInfoPopupViewModel model = new PartInfoPopupViewModel();

            model.PartId = partID;
            model.EstimatedShipDateStr = "";
            model.RequiredDateStr = "";
            model.WorkScope = "";
            model.JobNotes = "";
            model.PartNotes = "";
            model.PopupTabIndex = tabIndex;

            using (var db = new JDsDBEntities())
            {
                var matchingPart = db.Parts.FirstOrDefault(x => x.PartId == partID);
                if (matchingPart == null)
                {
                    // This is a problem
                    _logger.Trace("Error fetching part: " + partID);
                    return PartialView("PartInfoPopUp", model);
                }
                var matchingJob = db.Jobs.FirstOrDefault(x => x.JobId == matchingPart.JobId);
                if (matchingJob == null)
                {
                    _logger.Trace("Error fetching job for part: " + partID);
                    return PartialView("PartInfoPopUp", model);
                }

                // otherwise, we've got it all

                model.PartId = partID;
                model.JobId = matchingJob.JobId;
                model.EstimatedShipDateStr = matchingPart.ShipByDate.HasValue ? matchingPart.ShipByDate.Value.ToString("M/d/yyyy") : "";
                model.RequiredDateStr = matchingPart.RequiredDate.HasValue ? matchingPart.RequiredDate.Value.ToString("M/d/yyyy") : "";
                model.WorkScope = matchingPart.WorkScope;
                model.JobNotes = matchingJob.JobNotes;
                model.PartNotes = matchingPart.NonConformanceReportNotes;
                model.IsPartBlocked = matchingPart.PartStatusId.HasValue && matchingPart.PartStatusId == (int)PartStatus.Blocked;

                // If dates haven't been specified for the est ship and required, then use the ones from the Job
                if (string.IsNullOrEmpty(model.EstimatedShipDateStr))
                    model.EstimatedShipDateStr = db.Jobs.First(x => x.JobId == matchingPart.JobId).ShipByDate.HasValue ? db.Jobs.First(x => x.JobId == matchingPart.JobId).ShipByDate.Value.ToShortDateString() : "";
                if (string.IsNullOrEmpty(model.RequiredDateStr))
                    model.RequiredDateStr = db.Jobs.First(x => x.JobId == matchingPart.JobId).RequiredDate.HasValue ? db.Jobs.First(x => x.JobId == matchingPart.JobId).RequiredDate.Value.ToShortDateString() : "";

                return PartialView("PartInfoPopUp", model);
            }
        }
        

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult UnblockPart(int partId)
        {
            using (var db = new JDsDBEntities())
            {
                try
                {
                    // find the part & job
                    var part = db.Parts.Find(partId);
                    var job = db.Jobs.Find(part.JobId);

                    // Change the part status back to in progress
                    part.PartStatusId = (int)PartStatus.InProgress;

                    // Change the job status back to in progress, too, but only if there are no other blocked parts
                    bool noBlockedParts = true;
                    foreach (var p in job.Parts)
                    {
                        if (p.PartStatusId == (int)PartStatus.Blocked)
                            noBlockedParts = false;
                    }
                    if (noBlockedParts)
                        job.JobStatusId = (int)JobStatus.InProgress;

                    db.SaveChanges();

                    // Let the world know
                    StatusChangeHelper.PublishJobStatusChange(job.JobId, job.JobStatusId.Value);
                    StatusChangeHelper.PublishPartStatusChange(job.JobId, part.PartId, part.PartStatusId.Value);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Error unblocking part {partId}: {ex.Message}");
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult PartInfoPopUpModal()
        {
            // Just gives the basic scaffolding for the part info dialog
            return PartialView("PartInfoPopUpModal");
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AddEditPartPhotoPopUpModal(int partPhotoId, int partId)
        {
            using (var db = new JDsDBEntities())
            {
                var matchingPhoto = db.PartPhotoes.FirstOrDefault(x => x.PartPhotoId == partPhotoId);
                if (matchingPhoto == null)
                {
                    matchingPhoto = new PartPhoto();
                }

                PartInfoPhotoViewModel vm = new PartInfoPhotoViewModel();
                vm.PartPhotoId = partPhotoId;
                vm.URL = matchingPhoto.PhotoUrl;
                vm.PhotoNotes = matchingPhoto.Notes;
                vm.PartId = partId;

                // if there's no picture, put in the stub
                if (string.IsNullOrEmpty(vm.URL))
                    vm.URL = "/Content/Images/missingImage.png";

                return PartialView("AddEditPartPhoto", vm);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AddEditPartFilePopUpModal(int partFileId, int partId)
        {
            using (var db = new JDsDBEntities())
            {
                var matchingFile = db.PartFiles.FirstOrDefault(x => x.PartFileId == partFileId);
                if (matchingFile == null)
                {
                    matchingFile = new PartFile();
                }

                PartInfoFileViewModel vm = new PartInfoFileViewModel();
                vm.PartFileId = partFileId;
                vm.URL = matchingFile.FileUrl;
                vm.FileNotes = matchingFile.Notes;
                vm.PartId = partId;
                vm.FileName = matchingFile.FileName;

                return PartialView("AddEditPartFile", vm);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadPartInfoPhotos(int id)
        {
            using (var db = new JDsDBEntities())
            {
                var photos = db.PartPhotoes.Where(x => x.PartId == id).ToList();

                var result = photos.Select(file => new PartInfoPhotoViewModel()
                {
                    FileName = file.Notes,
                    PartPhotoId = file.PartPhotoId,
                    URL = file.PhotoUrl,
                    PhotoNotes = file.Notes
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadPartInfoFiles(int id)
        {
            using (var db = new JDsDBEntities())
            {
                var photos = db.PartFiles.Where(x => x.PartId == id).ToList();

                var result = photos.Select(file => new PartInfoFileViewModel()
                {
                    FileName = file.FileName,
                    PartFileId = file.PartFileId,
                    URL = file.FileUrl,
                    FileNotes = file.Notes
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // Return the URL of the uploaded file
        [HttpPost]
        public async Task<ActionResult> PartInfoPhotoUpload(int id, HttpPostedFileBase file)
        {
            if (file == null)
            {
                _logger.Error("PartInfoPhotoUpload for PartId={0}: file is null", id);
                return null;
            }

            //// TO USER AZURE BLOB STORAGE...
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //CloudBlobContainer container = blobClient.GetContainerReference("partphotos");
            //container.CreateIfNotExists();


            //CloudBlockBlob blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");

            //await blockBlob.UploadFromStreamAsync(file.InputStream);

            //await blockBlob.FetchAttributesAsync();
            //blockBlob.Properties.ContentType = "image/png";
            //await blockBlob.SetPropertiesAsync();

            string fileURL = StorageHelper.addFile(file, "partphotos");

            return Json(new { success = true, URL = fileURL }, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public FileResult DownloadPartPhoto(int id)
        {
            using (var db = new JDsDBEntities())
            {
                var photo = db.PartPhotoes.Where(x => x.PartPhotoId == id).FirstOrDefault();
                if (photo == null || string.IsNullOrEmpty(photo.PhotoUrl))
                    return null;

                // get the file name to use to download it
                var extension = System.IO.Path.GetExtension(photo.PhotoUrl);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(photo.PhotoUrl);

                string imgPath;
                if (photo.PhotoUrl.ToLower().StartsWith("http"))
                {
                    imgPath = photo.PhotoUrl;   // don't map it if it's already complete

                    // we need to download it from its current location before we can send it back down
                    var tempFile = Path.GetTempFileName();
                    tempFile = Path.ChangeExtension(tempFile, extension);
                    WebClient client = new WebClient();
                    client.DownloadFile(imgPath, tempFile);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(tempFile);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = "partPhoto" + extension;

                    // delete the temp file if possible
                    try
                    {
                        System.IO.File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                        // well, we tried to be tidy
                    }

                    return response;
                }
                else if (photo.PhotoUrl.StartsWith("~") || photo.PhotoUrl.StartsWith("/") || photo.PhotoUrl.StartsWith("\\"))
                {
                    // Then this is (probably) a partial, virtual path; go ahead and map it

                    imgPath = Server.MapPath(photo.PhotoUrl);   //if it's partial go ahead and map it

                    if (!System.IO.File.Exists(imgPath))
                        return null;        // probably got saved on somebody else's maching

                    byte[] fileBytes = System.IO.File.ReadAllBytes(imgPath);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = "partPhoto" + extension;
                    return response;
                }
                else  // then this is (probably) a rooted path like D:\my\app\stuff
                {
                    if (!System.IO.File.Exists(photo.PhotoUrl))
                        return null;        // probably got saved on somebody else's maching
                    byte[] fileBytes = System.IO.File.ReadAllBytes(photo.PhotoUrl);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = "partPhoto" + extension;
                    return response;
                }
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult SavePartPhoto(PartInfoPhotoViewModel model)
        {
            using (var db = new JDsDBEntities())
            {
                bool isNewRecord = false;
                var matchingPartPhoto = db.PartPhotoes.FirstOrDefault(x => x.PartPhotoId == model.PartPhotoId);
                if (matchingPartPhoto == null)
                {
                    isNewRecord = true;
                    matchingPartPhoto = new PartPhoto();
                }

                string path = Server.MapPath($"~{model.URL}");
                if (!System.IO.File.Exists(path))
                    return Json(false, JsonRequestBehavior.AllowGet);

                matchingPartPhoto.Notes = model.PhotoNotes?.Trim() ?? "";
                matchingPartPhoto.PartId = model.PartId;
                matchingPartPhoto.PhotoUrl = model.URL;
                matchingPartPhoto.DateTaken = DateTime.Now;

                if (isNewRecord)
                    db.PartPhotoes.Add(matchingPartPhoto);

                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult DeletePartPhoto(int id)
        {
            // Find that sucker and delete it
            using (var db = new JDsDBEntities())
            {
                var matchingPhoto = db.PartPhotoes.FirstOrDefault(x => x.PartPhotoId == id);
                if (matchingPhoto == null)
                {
                    _logger.Trace("Error, can't find part photo for deletion. ID=" + id);
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                db.PartPhotoes.Remove(matchingPhoto);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public async Task<string> SaveDataURLToFile(int partId, string data)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("partphotos");
            container.CreateIfNotExists();
            

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");

            byte[] dataAsBytes = Convert.FromBase64String(data);
            await blockBlob.UploadFromByteArrayAsync(dataAsBytes, 0, dataAsBytes.Length);

            await blockBlob.FetchAttributesAsync();
            blockBlob.Properties.ContentType = "image/png";
            await blockBlob.SetPropertiesAsync();
            
            return blockBlob.StorageUri.PrimaryUri.AbsoluteUri;
        }



        // Return the URL of the uploaded file
        [HttpPost]
        public async Task<ActionResult> PartInfoFileUpload(int id, HttpPostedFileBase file)
        {
            if (file == null)
            {
                _logger.Error("PartInfoFileUpload for PartId={0}: file is null", id);
                return null;
            }

            using (var db = new JDsDBEntities())
            {
                // Find the corresponding part
                var part = db.Parts.FirstOrDefault(x => x.PartId == id);
                if (part == null)
                {
                    // problem! no matching part
                    _logger.Error("PartInfoFileUpload for PartId={0} not found.", id);
                    return Json(new { success = false, URL = "" }, JsonRequestBehavior.AllowGet);
                }
            }

            //// TO USE AZURE BLOB STORAGE...
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //CloudBlobContainer container = blobClient.GetContainerReference("partphotos");
            //container.CreateIfNotExists();


            //CloudBlockBlob blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");

            //await blockBlob.UploadFromStreamAsync(file.InputStream);

            //await blockBlob.FetchAttributesAsync();
            //blockBlob.Properties.ContentType = file.ContentType;
            //await blockBlob.SetPropertiesAsync();

            string fileURL = StorageHelper.addFile(file, "partfiles");

            return Json(new { success = true, URL = fileURL }, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public FileResult DownloadPartFile(int id)
        {
            using (var db = new JDsDBEntities())
            {
                var file = db.PartFiles.Where(x => x.PartFileId == id).FirstOrDefault();
                if (file == null || string.IsNullOrEmpty(file.FileUrl))
                    return null;

                // get the file name to use to download it
                var extension = System.IO.Path.GetExtension(file.FileUrl);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileUrl);

                string imgPath;
                if (file.FileUrl.ToLower().StartsWith("http"))
                {
                    imgPath = file.FileUrl;   // don't map it if it's already complete

                    // we need to download it from its current location before we can send it back down
                    var tempFile = Path.GetTempFileName();
                    tempFile = Path.ChangeExtension(tempFile, extension);
                    WebClient client = new WebClient();
                    client.DownloadFile(imgPath, tempFile);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(tempFile);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = file.FileName;

                    // delete the temp file if possible
                    try
                    {
                        System.IO.File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                        // well, we tried to be tidy
                    }

                    return response;
                }
                else if (file.FileUrl.StartsWith("~") || file.FileUrl.StartsWith("/") || file.FileUrl.StartsWith("\\"))
                {
                    // Then this is (probably) a partial, virtual path; go ahead and map it

                    imgPath = Server.MapPath(file.FileUrl);   //if it's partial go ahead and map it

                    if (!System.IO.File.Exists(imgPath))
                        return null;        // probably got saved on somebody else's maching

                    byte[] fileBytes = System.IO.File.ReadAllBytes(imgPath);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = file.FileName;
                    return response;
                }
                else  // then this is (probably) a rooted path like D:\my\app\stuff
                {
                    if (!System.IO.File.Exists(file.FileUrl))
                        return null;        // probably got saved on somebody else's maching
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file.FileUrl);
                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = file.FileName;
                    return response;
                }
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult SavePartFile(PartInfoFileViewModel model)
        {
            using (var db = new JDsDBEntities())
            {
                bool isNewRecord = false;
                var matchingPartFile = db.PartFiles.FirstOrDefault(x => x.PartFileId == model.PartFileId);
                if (matchingPartFile == null)
                {
                    isNewRecord = true;
                    matchingPartFile = new PartFile();
                }

                string path = Server.MapPath($"~{model.URL}");
                if (!System.IO.File.Exists(path))
                    return Json(false, JsonRequestBehavior.AllowGet);

                matchingPartFile.Notes = model.FileNotes?.Trim() ?? "";
                matchingPartFile.PartId = model.PartId;
                matchingPartFile.FileUrl = model.URL;
                matchingPartFile.CreatedDate = DateTime.Now;
                matchingPartFile.FileName = model.FileName;
                matchingPartFile.IsFinalPrint = false;

                if (isNewRecord)
                    db.PartFiles.Add(matchingPartFile);

                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public ActionResult DeletePartFile(int id)
        {
            // Find that entry and delete it
            using (var db = new JDsDBEntities())
            {
                var matchingFile = db.PartFiles.FirstOrDefault(x => x.PartFileId == id);
                if (matchingFile == null)
                {
                    _logger.Trace("Error, can't find part file for deletion. ID=" + id);
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                db.PartFiles.Remove(matchingFile);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetFilesForDropdown(int id = 0)
        {
            using (var db = new JDsDBEntities())
            {
                var partFiles = db.PartFiles.Where(x => x.PartId == id)
                    .Select(x =>
                      new
                      {
                          Name = x.FileName,
                          PartFileId = x.PartFileId,
                      }).OrderBy(x => x.Name).ToList();

                partFiles.Insert(0, new
                {
                    Name = "None",
                    PartFileId = 0,
                });

                return Json(partFiles, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetFinalPrintId(int partId)
        {
            // Return the first ID of the print file marked Final Print, or 0
            using (var db = new JDsDBEntities())
            {
                int matchingId = 0;
                var matchingPartFile = db.PartFiles.Where(x => x.PartId == partId && x.IsFinalPrint.HasValue && x.IsFinalPrint.Value == true).FirstOrDefault();
                if (matchingPartFile != null)
                    matchingId = matchingPartFile.PartFileId;

                string matchingName = matchingPartFile == null ? "" : matchingPartFile.FileName;

                return Json(new { PartFileId = matchingId, Name = matchingName }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public void SetFinalPrintId(int partFileId, int partId)
        {
            // Return the first ID of the print file marked Final Print, or 0
            using (var db = new JDsDBEntities())
            {
                // Set all partfiles for this part to false
                foreach (var part in db.PartFiles.Where(x => x.PartId == partId))
                {
                    part.IsFinalPrint = false;
                }

                // If one matches, set it to true
                var matchingPartFile = db.PartFiles.Where(x => x.PartFileId == partFileId).FirstOrDefault();
                if (matchingPartFile != null)
                {
                    // Set the one to true
                    matchingPartFile.IsFinalPrint = true;
                }

                db.SaveChanges();
            }
        }


        #endregion


        #region PrintOutgoingForm

        public ActionResult OutgoingFormWPartId(int partId)
        {
            OutgoingFormViewModel ofvm = new OutgoingFormViewModel();
            BB11_FinalInspectionViewModel finalInspectionVM = new BB11_FinalInspectionViewModel();
            BB9_FinishBoreProcessViewModel finishBoreVM = new BB9_FinishBoreProcessViewModel();
            BB1_IncomingInspectionViewModel incomingInspectionVM = new BB1_IncomingInspectionViewModel();
            BB8_FinalMachineInspectionViewModel finalMachineVM = new BB8_FinalMachineInspectionViewModel();

            using (JDsDBEntities db = new JDsDBEntities())
            {
                var part = db.Parts.Where(x => x.PartId == partId).FirstOrDefault();
                if (part != null)
                {
                    ofvm.PartId = part.PartId;
                    ofvm.BearingType = part.PartType.Name;
                    ofvm.Notes = part.WorkScope;
                    ofvm.Date = part.ShippedDate.HasValue ? part.ShippedDate.Value.ToShortDateString() : "";
                }

                var sizes = db.Sizes.Where(x => x.PartId == partId).FirstOrDefault();
                if(sizes != null)
                {
                    ofvm.BoreDiameter = sizes.BoreSize;
                    ofvm.ShaftSize = sizes.Shaft;
                    ofvm.BoreSizeVertical = sizes.BoreSize;
                    ofvm.BoreSizeHorizontal = sizes.BoreSizeHorizontal;
                }

                var job = db.Jobs.Where(x => x.JobId == part.JobId).FirstOrDefault();
                if(job != null)
                {
                    ofvm.JobId = job.JobId;                                        
                    ofvm.CustomerJobNumber = job.CustomerJobNumber;
                    ofvm.PONumber = job.PurchaseOrderNumber;
                    ofvm.CustomerName = job.Contact.Customer.CompanyName;
                    ofvm.ContactName = job.Contact.FirstName + " " + job.Contact.LastName;

                }

                var processId = db.Processes.Where(x => x.PartId == partId).Select(s => s.ProcessId).FirstOrDefault();

                // Get Incoming Inspection + data
                var jsonII = db.Steps.Where(x => x.StepNumber == 1 && x.ProcessId == processId).Select(x => x.StringValue).FirstOrDefault();
                if (jsonII != null)
                {
                    incomingInspectionVM = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(jsonII);

                    ofvm.PartsInstalled = incomingInspectionVM.PartsAccessories;
                    ofvm.Insulated = incomingInspectionVM.IsInsulated;
                    ofvm.BabbittLength = incomingInspectionVM.OverallLength.HasValue ? incomingInspectionVM.OverallLength.Value : 0;

                }

                // Get Final Inspection + corresponding data
                var jsonFI = db.Steps.Where(x => x.StepNumber == 11 && x.ProcessId == processId).Select(s => s.StringValue).FirstOrDefault();
                var jsonFM = db.Steps.Where(x => x.StepNumber == 8 && x.ProcessId == processId).Select(s => s.StringValue).FirstOrDefault();
                if(jsonFI != null)
                {
                    finalInspectionVM = JsonConvert.DeserializeObject<BB11_FinalInspectionViewModel>(jsonFI ?? "");
                    finalMachineVM = JsonConvert.DeserializeObject<BB8_FinalMachineInspectionViewModel>(jsonFM ?? "");

                    // If insulated get OD sizes from Final Inspection
                    if(ofvm.Insulated)
                    {
                        ofvm.OD1Measurements = finalInspectionVM.OD1Measurements;
                        ofvm.OD2Measurements = finalInspectionVM.OD2Measurements;
                    }
                    else // If non-insulated get OD sizes from Final Machine
                    {
                        ofvm.OD1Measurements = finalMachineVM.OD1Measurements;
                        ofvm.OD2Measurements = finalMachineVM.OD2Measurements;
                    }                    

                    ofvm.ID1Measurements = finalInspectionVM.ID1Measurements;
                    ofvm.ID2Measurements = finalInspectionVM.ID2Measurements;
                    ofvm.CheckedBy = finalInspectionVM.FinalInspectionBy;
                }        
            }

            return PartialView("OutgoingForm", ofvm);
        }

        public ActionResult OutgoingFormForJob (int jobId)
        {
            OutgoingFormsForJobViewModel outgoingFormList = new OutgoingFormsForJobViewModel() { OutgoingForms = new List<OutgoingFormViewModel>() };
            BB11_FinalInspectionViewModel finalInspectionVM = new BB11_FinalInspectionViewModel();
            BB9_FinishBoreProcessViewModel finishBoreVM = new BB9_FinishBoreProcessViewModel();
            BB1_IncomingInspectionViewModel incomingInspectionVM = new BB1_IncomingInspectionViewModel();

            using (JDsDBEntities db = new JDsDBEntities())
            {
                var job = db.Jobs.Find(jobId);
                var parts = job.Parts.Select(x => x.PartId).ToList();

                if(parts != null)
                {
                    foreach (var p in parts)
                    {
                        var ofvm = new OutgoingFormViewModel();
                        ofvm.JobId = job.JobId;
                        ofvm.Date = job.ShippedDate.HasValue ? job.ShippedDate.Value.ToShortDateString() : "";
                        ofvm.CustomerJobNumber = job.CustomerJobNumber;
                        ofvm.PONumber = job.PurchaseOrderNumber;

                        var part = db.Parts.Where(x => x.PartId == p).FirstOrDefault();
                        if(part != null)
                        {
                            ofvm.PartId = part.PartId;
                            ofvm.BearingType = part.PartType.Name;
                            ofvm.Notes = part.WorkScope;

                            var sizes = db.Sizes.Where(x => x.PartId == part.PartId).FirstOrDefault();
                            if(sizes != null)
                            {
                                ofvm.BoreDiameter = sizes.BoreSize;
                                ofvm.ShaftSize = sizes.Shaft;
                            }
                        }
                        
                        var contact = db.Contacts.Where(x => x.CustomerId == job.CustomerContactId).FirstOrDefault();
                        if(contact != null)
                        {
                            ofvm.ContactName = contact.FirstName + " " + contact.LastName;

                            var customer = db.Customers.Where(x => x.CustomerId == contact.CustomerId).FirstOrDefault();
                            if(customer != null)
                            {
                                ofvm.CustomerName = customer.CompanyName;
                            }
                        }
                        

                        var processId = db.Processes.Where(x => x.PartId == p).Select(s => s.ProcessId).FirstOrDefault();

                        // Get Final Inspection + corresponding data
                        var json = db.Steps.Where(x => x.StepNumber == 11 && x.ProcessId == processId).Select(s => s.StringValue).FirstOrDefault();
                        if (json != null)
                        {
                            finalInspectionVM = JsonConvert.DeserializeObject<BB11_FinalInspectionViewModel>(json);

                            ofvm.OD1Measurements = finalInspectionVM.OD1Measurements.Select(x => new BoreMeasurementViewModel()
                            {
                                A= x.A,
                                B = x.B,
                                C = x.C
                            }).ToList();
                            ofvm.OD2Measurements = finalInspectionVM.OD2Measurements;
                            ofvm.ID1Measurements = finalInspectionVM.ID1Measurements;
                            ofvm.ID2Measurements = finalInspectionVM.ID2Measurements;
                            ofvm.CheckedBy = finalInspectionVM.FinalInspectionBy;
                        }

                        //Get Finish Bore + corresponding data
                        var json2 = db.Steps.Where(x => x.StepNumber == 9 && x.ProcessId == processId).Select(x => x.StringValue).FirstOrDefault();
                        if (json2 != null)
                        {
                            finishBoreVM = JsonConvert.DeserializeObject<BB9_FinishBoreProcessViewModel>(json2);

                            ofvm.BoreSizeVertical = finishBoreVM.CustomerBoreSize;
                            ofvm.BoreSizeHorizontal = finishBoreVM.CustomerBoreSizeHorizontal;
                        }


                        // Get Incoming Inspection + data
                        var json3 = db.Steps.Where(x => x.StepNumber == 1 && x.ProcessId == processId).Select(x => x.StringValue).FirstOrDefault();
                        if (json3 != null)
                        {
                            incomingInspectionVM = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(json3);

                            ofvm.PartsInstalled = incomingInspectionVM.PartsAccessories;
                            ofvm.Insulated = incomingInspectionVM.IsInsulated;
                            ofvm.BabbittLength = incomingInspectionVM.OverallLength.HasValue ? incomingInspectionVM.OverallLength.Value : 0;

                        }

                        outgoingFormList.OutgoingForms.Add(ofvm);
                    }
                }
            }

            return PartialView("OutgoingFormsForJob", outgoingFormList);
        }

        #endregion


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult CheckPartForFiles(int partId)
        {
            bool partHasPhotosAndFiles = false;
            try
            {
                using (var db = new JDsDBEntities())
                {
                    int photoCount = db.PartPhotoes.Where(x => x.PartId == partId).Count();
                    int fileCount = db.PartFiles.Where(x => x.PartId == partId).Count();

                    partHasPhotosAndFiles = (photoCount > 0 && fileCount > 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error in CheckPartForFiles for {partId}: {ex.Message}");
            }
            return Json(partHasPhotosAndFiles, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpGet]
        public ActionResult CheckPartForCustomerSizes(int partId)
        {
            bool partHasSizes = false;
            try
            {
                using (var db = new JDsDBEntities())
                {
                    var custSize = db.Sizes.FirstOrDefault(x => x.PartId == partId);
                    if (custSize.BoreSize.Length > 0 && custSize.MeasurementList.PartDiameterMeasurements.Count() > 0)
                        partHasSizes = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Error in CheckPartForFiles for {partId}: {ex.Message}");
            }
            return Json(partHasSizes, JsonRequestBehavior.AllowGet);
        }
    }
}