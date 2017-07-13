using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsDataModel;
using JDsWebApp.Helpers;
using Kendo.Mvc.Extensions;
using System.IO;
using System.Threading.Tasks;
using JDsWebApp.Areas.EmployeePortal.Models.Certificate;
using NLog;
using Kendo.Mvc.UI;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class CertificateController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JDsDBEntities _db = new JDsDBEntities();

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult CertificateDetails(int id = 0)
        {
            var viewmodel = new CertificateViewModel();

            if (id == 0)
            {
                viewmodel.CertificateDate = DateTime.UtcNow.Month.ToString() + "/" + DateTime.UtcNow.Day.ToString() + "/" + DateTime.UtcNow.Year.ToString();
                viewmodel.CertificateExpires = DateTime.UtcNow.AddDays(365).Month.ToString() + "/" + DateTime.UtcNow.AddDays(365).Day.ToString() + "/" + DateTime.UtcNow.AddDays(365).Year.ToString();
            }
            else
            {
                var certificate = _db.Certificates.Find(id);

                if (certificate == null)
                {
                    _logger.Error("Certificate details for CertificateId={0} not found.", id);
                }
                else
                {
                    var certificateFile = certificate.CertificateFileId.HasValue
                        ? _db.CertificateFiles.Find(certificate.CertificateFileId)
                        : null;
                    viewmodel.CertificateId = certificate.CertificateId;
                    viewmodel.CertificateFileId = certificate.CertificateFileId;
                    viewmodel.Name = certificate.Name;
                    viewmodel.CertificateDate = certificate.CertificateDate;
                    viewmodel.CertificateExpires = certificate.CertificateExpires;
                    viewmodel.Notes = certificate.Notes;
                    viewmodel.CompanyName = certificate.CompanyName;
                    viewmodel.OriginalFilename = certificateFile == null
                        ? string.Empty
                        : certificateFile.OriginalFilename;
                    viewmodel.URL = certificateFile?.FileURL ?? "";
                }
            }
            
            return PartialView("_CertificateDetails", viewmodel);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult DeleteCertificate(int id)
        {
            var certificate = _db.Certificates.Include("CertificateFile").FirstOrDefault(x => x.CertificateId == id);

            if (certificate == null)
            {
                _logger.Error("Delete certificate for CertificateId={0} not found.", id);
                return Content(string.Format("Delete certificate for CertificateId={0} not found.", id));
            }
            else
            {
                _logger.Trace("Deleting certificate and file information for CertificateId={0} '{1}'", certificate.CertificateId,
                    certificate.Name);

                if (certificate.CertificateFile != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath(Properties.Settings.Default.BaseEmployeeCertificates), certificate.CertificateFile.Filename);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                        _db.CertificateFiles.Remove(certificate.CertificateFile);
                        _logger.Trace("Deleted file '{0}'.", physicalPath);
                    }
                    else
                    {
                        _logger.Error("Unable to find file '{0}' to delete.", physicalPath);
                    }
                }

                _db.Certificates.Remove(certificate);
                _db.SaveChanges();
            }

            return Content("");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ReadCertificates()
        {
            var certificates = _db.Certificates.Include("CertificateFile").ToList();

            var result = certificates.Select(x => new CertificateViewModel
            {
                CertificateId = x.CertificateId,
                CertificateFileId = x.CertificateFileId,
                Name = x.Name,
                CertificateDate = x.CertificateDate,
                CertificateExpires = x.CertificateExpires,
                Notes = x.Notes,
                CompanyName = x.CompanyName,
                OriginalFilename = x.CertificateFile == null ? string.Empty : x.CertificateFile.OriginalFilename,
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SaveCertificate(CertificateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CertificateDetails", model);
            }

            // Create new or edit existing
            var certificate = model.CertificateId == 0 ? new Certificate() : _db.Certificates.Find(model.CertificateId);

            if (certificate == null)
            {
                _logger.Error("Unable to save CertificateId={0} because the certificate record was not found.", model.CertificateId);
                return PartialView("_CertificateDetails", new CertificateViewModel());
            }

            certificate.CertificateDate = model.CertificateDate;
            certificate.CertificateExpires = model.CertificateExpires;
            certificate.Notes = model.Notes;
            certificate.CompanyName = model.CompanyName;
            certificate.Name = model.Name;
            certificate.CertificateFileId = model.CertificateFileId;
            
            if (certificate.CertificateId == 0)
            {
                _db.Certificates.Add(certificate);
            }
            else
            {
                var entry = _db.Entry<Certificate>(certificate);
                entry.State = EntityState.Modified;
            }

            _db.SaveChanges();
            _logger.Trace("Saved certerficate information for CertificateId={0}", certificate.CertificateId);

            if (certificate.CertificateFileId != null)
            {
                var certificateFile = _db.CertificateFiles.Find(certificate.CertificateFileId);

                if (certificateFile != null)
                {
                    model.OriginalFilename = certificateFile.OriginalFilename;
                }
            }

            return CertificateDetails(certificate.CertificateId);
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public async Task<ActionResult> UploadCertificateFile(int id, HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("");
            }

            var certificate = _db.Certificates.Find(id);

            if (certificate == null)
            {
                _logger.Error("CertificateId={0} not found, unable to save certificate file.", id);
                return Content(string.Format("CertificateId={0} not found, unable to save certificate file.", id));
            }

            if (file != null)
            {
                //// TO USE AZURE STORAGE...
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString);

                //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //CloudBlobContainer container = blobClient.GetContainerReference("certificates");
                //container.CreateIfNotExists();


                //CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                //await blockBlob.UploadFromStreamAsync(file.InputStream);

                //await blockBlob.FetchAttributesAsync();
                //blockBlob.Properties.ContentType = file.ContentType;
                //await blockBlob.SetPropertiesAsync();

                string path = StorageHelper.addFile(file, "certificates"); // blockBlob.StorageUri.PrimaryUri.AbsoluteUri;
                string originalfilename = file.FileName;
                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var certificateFile = _db.CertificateFiles.FirstOrDefault(x => x.Filename.Equals(filename));
                    if (certificateFile == null)
                    {
                        certificateFile = new CertificateFile();
                    }

                    certificateFile.Filename = filename;
                    certificateFile.FileURL = path;
                    certificateFile.CreatedDate = DateTime.UtcNow;
                    certificateFile.OriginalFilename = originalfilename;

                    if (certificateFile.CertificateFileId == 0)
                    {
                        // clean up old certificate files
                        var oldFileEntries =
                            _db.CertificateFiles.Where(x => x.CertificateFileId == certificate.CertificateFileId)
                                .ToList();
                        _db.CertificateFiles.RemoveRange(oldFileEntries);

                        _db.CertificateFiles.Add(certificateFile);
                    }
                    else
                    {
                        var entry = _db.Entry<CertificateFile>(certificateFile);
                        entry.State = EntityState.Modified;
                    }
                    
                    certificate.CertificateFile = certificateFile;

                    await _db.SaveChangesAsync();
                    _logger.Trace("Saved certificate file information for CertificateId={0} '{1}'", certificate.CertificateId, originalfilename);
                }
            }

            return Content("");
        }

        private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
        {
            return
                from a in files
                where a != null
                select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
        }
    }
}