using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.Certificate;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeManagement;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeUpload;
using JDsWebApp.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NLog;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class EmployeeUploadController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult EmployeeFilesMgmt(string name, int id = 0)
        {
            var viewmodel = new EmployeeFilesMgmtViewModel
            {
                Id = id,
                Name = name,
            };

            if (id > 0)
            {
                var employee = _db.Employees.FirstOrDefault(e => e.EmployeeId == id);
                EmployeeFilesHelper.SyncEmployeeFilesWithDatabaseEntry(employee, _db);
            }

            return PartialView("_EmployeeFilesMgmt", viewmodel);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadEmployeeFiles(int id)
        {
            IQueryable<EmployeeFile> files = _db.EmployeeFiles.Where(x => x.EmployeeId == id);
            
            var result = files.Select(file => new EmployeeFileViewModel()
            {
                EmployeeId = file.EmployeeId,
                FileId = file.EmployeeFileId,
                Filename = file.Filename,
                Uploaded =  file.CreatedDate,
                URL = file.FileURL
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UploadEmployeeFile(int id, HttpPostedFileBase file)
        {
            if (_db.Employees.Find(id) == null)
            {
                _logger.Error("EmployeeId={0} not found, unable to save employee files.", id);
                return RedirectToRoute("EmployeePortal/EmployeeManagement/EmployeeDetails/" + id);
            }

            if (file != null)
            {
                // Some browsers send file names with full path. This needs to be stripped.
                var filename = Path.GetFileName(file.FileName);

                // The dirctory is /{BaseEmployeeFilesPathname}/{EmployeeId}/  until otherwise.
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    //// TO USE AZURE STORAGE...
                    //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

                    //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    //CloudBlobContainer container = blobClient.GetContainerReference("employeefiles");
                    //container.CreateIfNotExists();


                    //CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                    //await blockBlob.UploadFromStreamAsync(file.InputStream);

                    //await blockBlob.FetchAttributesAsync();
                    //blockBlob.Properties.ContentType = file.ContentType;
                    //await blockBlob.SetPropertiesAsync();

                    var physicalPath = StorageHelper.addFile(file, "employeefiles", false); // blockBlob.StorageUri.PrimaryUri.AbsoluteUri;                   
                    
                    // prevent duplicate entries
                    var entry = _db.EmployeeFiles.FirstOrDefault(x => x.EmployeeId == id && x.Filename == filename);
                    if (entry == null)
                    {
                        entry = new EmployeeFile();
                        entry.EmployeeId = id;
                        entry.CreatedDate = DateTime.UtcNow;
                        entry.Filename = filename;
                        entry.FileURL = physicalPath;
                        entry.Notes = string.Empty;
                        _db.EmployeeFiles.Add(entry);

                        _logger.Trace("File for EmployeeId={0} '{1}' entry marked for insertion by UploadEmployeeFile().",
                            id, physicalPath);
                    }
                    else
                    {
                        // duplicate entry
                    }
                }

                if (_db.ChangeTracker.HasChanges())
                {
                    _db.SaveChanges();
                }
            }

            var employee = _db.Employees.Find(id);
            var viewmodel = new EmployeeFilesMgmtViewModel
            {
                Id = id,
                Name = employee == null ? string.Empty : employee.Name
            };
            
            return PartialView("_EmployeeFilesMgmt", viewmodel);
        }

        public FileResult DownloadEmployeeFile(int id)
        {
            var file = _db.EmployeeFiles.Find(id);
            if (file == null)
            {
                return null;
            }

            var path = Path.Combine(Server.MapPath(Properties.Settings.Default.BaseEmployeeFilesPathname));
            path = Path.Combine(path, file.EmployeeId.ToString(), file.Filename);

            if (!System.IO.File.Exists(path))
            {
                _logger.Error("Unable to find file '{0}' for EmployeeFileId={1}", file.Filename, id);
                _logger.Trace("Removing file entry for file '{0}' for EmployeeFileId={1}", file.Filename, id);
                _db.EmployeeFiles.Remove(file);
                _db.SaveChanges();
                return null;
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            var response = new FileContentResult(fileBytes, "application/octet-stream");
            response.FileDownloadName = file.Filename;
            return response;
        }

        [HttpPost]
        public async Task<ActionResult> DeleteEmployeeFile(int id)
        {
            var file = _db.EmployeeFiles.Find(id);

            if (file == null)
            {
                return Content("Unable to find file.");
            }

            //// TO USE AZURE BLOB STORAGE...
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //CloudBlobContainer container = blobClient.GetContainerReference("employeefiles");
            //container.CreateIfNotExists();

            //CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.Filename);

            //await blockBlob.DeleteIfExistsAsync();

            StorageHelper.deleteFile(file.Filename);

            _db.EmployeeFiles.Remove(file);
            _db.SaveChanges();

            return Content("");
        }
    }
}