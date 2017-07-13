using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace JDsWebApp.Helpers
{
    public class StorageHelper
    {
        public static string addFile(HttpPostedFileBase file, string container="default", bool useGUID=true)
        {
            string storageFolder = ConfigurationManager.AppSettings["StorageDirectory"];
            string path = HttpContext.Current.Server.MapPath(String.Format("~/{0}/", storageFolder));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, container);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filename;
            if (useGUID)
                filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            else
                filename = file.FileName;

            path = Path.Combine(path, filename);
            file.SaveAs(path);

            return String.Format("/{0}/{1}/{2}", storageFolder, container, filename);
        }

        public static void deleteFile(string filename, string container="default")
        {
            string storageFolder = ConfigurationManager.AppSettings["StorageDirectory"];
            string path = HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}/{2}", storageFolder, container, filename));
            File.Delete(path);
        }
    }
}