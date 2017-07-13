using System.Web.Mvc;
using System.Web.Http;

namespace JDsWebApp.Areas.EmployeePortal
{
    public class EmployeePortalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EmployeePortal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EmployeePortal_default",
                "EmployeePortal/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.Routes.MapHttpRoute(
                  name: "EmployeePortal_defaultApi",
                  routeTemplate: "EmployeePortal/api/{controller}/{action}/{id}",
                  defaults: new { id = RouteParameter.Optional }
            );
            
        }
    }
}