using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JDsWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "SignOff",
            url: "SignOff/{action}/{id}",
            defaults: new { controller = "SignOff", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "AutoComplete",
            url: "EmployeePortal/AutoComplete/{action}/{id}/{value}",
            defaults: new { controller = "AutoComplete", id = UrlParameter.Optional, value = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Part",
            url: "Part/{action}/{id}/{value}",
            defaults: new { controller = "Part", id = UrlParameter.Optional, value = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Customer",
            url: "Customer/{action}/{id}",
            defaults: new { controller = "Customer", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "BigBoard",
            url: "BigBoard/{action}/{id}",
            defaults: new { controller = "BigBoard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "JobHistory",
            url: "JobHistory/{action}/{id}",
            defaults: new { controller = "JobHistory", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ProcessBabbitBearing",
            url: "ProcessBabbitBearing/{action}",
            defaults: new { controller = "ProcessBabbitBearing", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Process",
            url: "Process/{action}",
            defaults: new { controller = "Process", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "EmployeePortalLogOut",
            url: "EmployeePortalLogin{action}/{id}",
            defaults: new { controller = "EmployeePortalLogin", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "Root",
              url: "{action}/{id}",
              defaults: new { controller = "Home", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            

        }
    }
}
