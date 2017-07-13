using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeManagement;
using JDsWebApp.Helpers;
using System.Data.SqlClient;
using System.Configuration;


namespace JDsWebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        string cs = ConfigurationManager.ConnectionStrings["JDs"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            EmployeeFilesHelper.SyncAllFilesWithDatabaseEntries();
        }
    }
}
