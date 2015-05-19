using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web.Optimization;

namespace JCarrollOnlineV2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            var cssTransformer = new StyleTransformer();
            var jsTransformer = new ScriptTransformer();
            var nullOrderer = new NullOrderer();

            var cssBundle = new StyleBundle("~/bundles/css");
            cssBundle.Include("~/Content/Site.less", 
                "~/Content/bootstrap/bootstrap.less",
                "~/Content/Prism.less",
                "~/Content/PagedList.less",
                "~/Content/themes/base/accordion.less",
                "~/Content/themes/base/all.less",
                "~/Content/themes/base/autocomplete.less",
                "~/Content/themes/base/base.less",
                "~/Content/themes/base/button.less",
                "~/Content/themes/base/core.less",
                "~/Content/themes/base/datepicker.less",
                "~/Content/themes/base/dialog.less",
                "~/Content/themes/base/draggable.less",
                "~/Content/themes/base/menu.less",
                "~/Content/themes/base/progressbar.less",
                "~/Content/themes/base/resizeable.less",
                "~/Content/themes/base/selectable.less",
                "~/Content/themes/base/selectmenu.less",
                "~/Content/themes/base/slider.less",
                "~/Content/themes/base/sortable.less",
                "~/Content/themes/base/spinner.less",
                "~/Content/themes/base/tabs.less",
                "~/Content/themes/base/theme.less",
                "~/Content/themes/base/tooltip.less",
                "~/Content/MarkdownDeep.less");

            cssBundle.Transforms.Add(cssTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(cssBundle);

            var jqueryBundle = new ScriptBundle("~/bundles/jquery");
            jqueryBundle.Include("~/Scripts/jquery-{version}.js");
            jqueryBundle.Include("~/Scripts/moment.js");
            jqueryBundle.Include("~/Scripts/livestamp.js");
            jqueryBundle.Include("~/Scripts/jquery.unobtrusive-ajax.min.js");
            jqueryBundle.Transforms.Add(jsTransformer);
            jqueryBundle.Orderer = nullOrderer;
            bundles.Add(jqueryBundle);

            var jqueryUIBundle = new ScriptBundle("~/bundles/jquery-ui");
            jqueryUIBundle.Include("~/Scripts/jquery-ui-1.11.4.js");
            jqueryUIBundle.Transforms.Add(jsTransformer);
            jqueryUIBundle.Orderer = nullOrderer;
            bundles.Add(jqueryUIBundle);

            var jqueryvalBundle = new ScriptBundle("~/bundles/jqueryval");
            jqueryvalBundle.Include("~/Scripts/jquery.validate*");
            jqueryvalBundle.Include("~/Scripts/jquery.validate.unobtrusive.ajax.min.js");
            jqueryvalBundle.Transforms.Add(jsTransformer);
            jqueryvalBundle.Orderer = nullOrderer;
            bundles.Add(jqueryvalBundle);

            var myjsbundle = new ScriptBundle("~/bundles/myjsbundle");
            myjsbundle.Include("~/Scripts/prism.js");
            myjsbundle.Include("~/Scripts/jcarrollonlinev2.js");
            myjsbundle.Include("~/Scripts/MarkdownDeepLib.min.js");
            myjsbundle.Transforms.Add(jsTransformer);
            myjsbundle.Orderer = nullOrderer;
            bundles.Add(myjsbundle);
            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            var modernizrBundle = new ScriptBundle("~/bundles/modernizr");
            modernizrBundle.Include("~/Scripts/modernizr-*");
            modernizrBundle.Transforms.Add(jsTransformer);
            modernizrBundle.Orderer = nullOrderer;
            bundles.Add(modernizrBundle);

            var bootstrapBundle = new ScriptBundle("~/bundles/bootstrap");
            bootstrapBundle.Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js");
            bootstrapBundle.Transforms.Add(jsTransformer);
            bootstrapBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapBundle);
        }
    }
}