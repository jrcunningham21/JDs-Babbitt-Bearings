using System.Web;
using System.Web.Optimization;

namespace JDsWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery-{version}.js", "~/Scripts/jquery.maskedinput.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/libs/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/libs/bootstrap.js",
                      "~/Scripts/libs/respond.js",
                      "~/Scripts/libs/bootstrap-toggle.js",
                      "~/Scripts/libs/bootbox.min.js",
                      "~/Scripts/libs/bootstrap-select.js",
                      "~/Scripts/libs/bootstrap-multiselect.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                      "~/Scripts/libs/kendo/kendo.all.min.js",
                      "~/Scripts/libs/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                      "~/Scripts/libs/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/libs/jquery.validate.js",
                        "~/Scripts/libs/jquery.validate.unobtrusive.js",
                        "~/Scripts/libs/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jszip").Include(
                        "~/Scripts/libs/jszip.js"));


            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/site.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/moment-range.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/fonts.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-toggle.min.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                      "~/Content/kendo/kendo.common.min.css",
                      "~/Content/kendo/kendo.default.min.css"));

            bundles.IgnoreList.Clear();
        }
    }
}
