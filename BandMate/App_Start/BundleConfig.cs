using System.Web;
using System.Web.Optimization;

namespace BandMate
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/checkout.js",
                      "~/Scripts/chartist.min.js",
                      "~/Scripts/jquery.cookie.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/fullcalendar.min.js",
                      "~/Scripts/transition.js",
                      "~/Scripts/collapse.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery-sortable-min.js",
                      "~/Scripts/fileinput.min.js",
                      "~/Scripts/app.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-slate.min.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/chartist.min.css",
                      "~/Content/fullcalendar.min.css",
                      "~/Content/fileinput.min.css",
                      "~/Content/site.css"));
        }
    }
}
