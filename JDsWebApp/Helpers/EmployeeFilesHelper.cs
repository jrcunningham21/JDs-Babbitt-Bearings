using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using JDsDataModel;
using NLog;
using System.Web.Hosting;

namespace JDsWebApp.Helpers
{
    public static class EmployeeFilesHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void SyncAllFilesWithDatabaseEntries()
        {
            SyncCertificateFilesWithDatabaseEntries();
            SyncEmployeeFilesWithDatabaseEntries();
        }

        /// <summary>
        /// The files actually on the filesystem is the source.
        /// </summary>
        public static void SyncCertificateFilesWithDatabaseEntries()
        {
            return;

            var physicalPath = Path.Combine(HostingEnvironment.MapPath(Properties.Settings.Default.BaseEmployeeCertificates));

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            if (Directory.Exists(physicalPath))
            {
                var actualFilenames = Directory.GetFiles(physicalPath);

                for (int i = 0; i < actualFilenames.Length; i++)
                {
                    actualFilenames[i] = Path.GetFileName(actualFilenames[i]);
                }

                using (var db = new JDsDBEntities())
                {
                    var certFiles = db.CertificateFiles.ToList();
                    var filenamesToRemove = certFiles.Where(x => !actualFilenames.Contains(x.Filename)).ToList();
                    var fileIds = filenamesToRemove.Select(x => x.CertificateFileId).ToList();

                    var markedCertificates =
                        db.Certificates.Where(
                            x => fileIds.Contains(x.CertificateFileId.HasValue ? x.CertificateFileId.Value : 0))
                            .ToList();

                    foreach (var c in markedCertificates)
                    {
                        c.CertificateFile = null;
                        var entry = db.Entry<Certificate>(c);
                        entry.State = EntityState.Modified;
                    }

                    db.CertificateFiles.RemoveRange(filenamesToRemove);
                    db.SaveChanges();
                }
            }

        }

        public static void SyncEmployeeFilesWithDatabaseEntries()
        {
            return;

            using (var db = new JDsDBEntities())
            {
                var employees = db.Employees.Include("EmployeeFiles").ToList();

                foreach (var employee in employees)
                {
                    SyncEmployeeFilesWithDatabaseEntry(employee, db);
                }
            }
        }

        /// <summary>
        /// The files actually on the filesystem is the source, sync the entries with what is there.
        /// </summary>
        public static void SyncEmployeeFilesWithDatabaseEntry(Employee employee, JDsDBEntities db)
        {
            return;

            if (employee == null)
            {
                return;
            }
            
            var physicalPath = Path.Combine(HostingEnvironment.MapPath(Properties.Settings.Default.BaseEmployeeFilesPathname));
            physicalPath = Path.Combine(physicalPath, employee.EmployeeId.ToString());
            
            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            if (Directory.Exists(physicalPath))
            {
                var actualFilenames = Directory.GetFiles(physicalPath);

                for (int i = 0; i < actualFilenames.Length; i++)
                {
                    actualFilenames[i] = Path.GetFileName(actualFilenames[i]);
                }

                var entriesToDelete =
                    employee.EmployeeFiles.Where(ef => !actualFilenames.Contains(ef.Filename)).ToArray();

                string[] filenamesToAdd;

                if (employee.EmployeeFiles.Count == 0)
                {
                    filenamesToAdd = actualFilenames;
                }
                else
                {
                    filenamesToAdd = actualFilenames.Where(af => employee.EmployeeFiles.Where(ef => ef.Filename == af) == null).ToArray();
                }

                foreach (var entry in entriesToDelete)
                {
                    _logger.Trace("EmployeeFileId={0} '{1}' entry marked for deletion from DB by SyncEmployeeFilesWithDatabaseEntry().",
                        entry.EmployeeFileId,
                        Path.Combine(physicalPath, entry.Filename));
                }

                db.EmployeeFiles.RemoveRange(entriesToDelete);

                foreach (var filename in filenamesToAdd)
                {
                    var entry = new EmployeeFile();
                    entry.EmployeeId = employee.EmployeeId;
                    entry.CreatedDate = DateTime.UtcNow;
                    entry.Filename = filename;
                    entry.FileURL = Path.Combine(physicalPath, filename);
                    entry.Notes = "SyncEmployeeFilesWithDatabaseEntry()";

                    db.EmployeeFiles.Add(entry);
                }

                if (db.ChangeTracker.HasChanges())
                {
                    db.SaveChanges();
                }
            }
        }
    }
}