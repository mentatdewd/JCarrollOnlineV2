using System.Web.Optimization;

using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;


namespace JCarrollOnlineV2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null)
            {
                throw new System.ArgumentNullException(nameof(bundles));
            }

            bundles.UseCdn = true;

            var nullBuilder = new NullBuilder();
            var styleTransformer = new StyleTransformer();
            var scriptTransformer = new ScriptTransformer();
            var nullOrderer = new NullOrderer();

            BundleResolver.Current = new CustomBundleResolver();

            var cssBundle = new StyleBundle("~/bundles/css");
            cssBundle.Include("~/Content/css/Site.css", 
                "~/Content/css/bootstrap/bootstrap.css",
                "~/Content/css/Prism.css",
                "~/Content/css/PagedList.css",
                "~/Content/css/themes/base/accordion.css",
                "~/Content/css/themes/base/all.css",
                "~/Content/css/themes/base/autocomplete.css",
                "~/Content/css/themes/base/base.css",
                "~/Content/css/themes/base/button.css",
                "~/Content/css/themes/base/core.css",
                "~/Content/css/themes/base/datepicker.css",
                "~/Content/css/themes/base/dialog.css",
                "~/Content/css/themes/base/draggable.css",
                "~/Content/css/themes/base/menu.css",
                "~/Content/css/themes/base/progressbar.css",
                "~/Content/css/themes/base/resizeable.css",
                "~/Content/css/themes/base/selectable.css",
                "~/Content/css/themes/base/selectmenu.css",
                "~/Content/css/themes/base/slider.css",
                "~/Content/css/themes/base/sortable.css",
                "~/Content/css/themes/base/spinner.css",
                "~/Content/css/themes/base/tabs.css",
                "~/Content/css/themes/base/theme.css",
                "~/Content/css/themes/base/tooltip.css",
                "~/Content/css/MarkdownDeep.css",
                "~/Content/css/toastr.css");

            cssBundle.Builder = nullBuilder;
            cssBundle.Transforms.Add(styleTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(cssBundle);

            var jqueryBundle = new ScriptBundle("~/bundles/jquery");
            jqueryBundle.Include("~/Scripts/less.js");
            jqueryBundle.Include("~/Scripts/jquery/jquery.js");
            jqueryBundle.Include("~/Scripts/moment.js");
            jqueryBundle.Include("~/Scripts/livestamp.js");
            jqueryBundle.Include("~/Scripts/jquery.unobtrusive-ajax.js");
            jqueryBundle.Include("~/Scripts/jquery-ui/jquery-ui.js");
            jqueryBundle.Transforms.Add(scriptTransformer);
            jqueryBundle.Orderer = nullOrderer;
            bundles.Add(jqueryBundle);

            var jqueryUIBundle = new ScriptBundle("~/bundles/jquery-ui");
            jqueryUIBundle.Transforms.Add(scriptTransformer);
            jqueryUIBundle.Orderer = nullOrderer;
            bundles.Add(jqueryUIBundle);

            var jqueryvalBundle = new ScriptBundle("~/bundles/jqueryval");
            jqueryvalBundle.Include("~/Scripts/jquery.validate.js");
            jqueryvalBundle.Include("~/Scripts/jquery.validate.unobtrusive.js");
            jqueryvalBundle.Include("~/Scripts/jquery.validate.unobtrusive.ajax.js");
            jqueryvalBundle.Transforms.Add(scriptTransformer);
            jqueryvalBundle.Orderer = nullOrderer;
            bundles.Add(jqueryvalBundle);

            var myjsbundle = new ScriptBundle("~/bundles/myjsbundle");
            myjsbundle.Include("~/Scripts/prism.js");
            myjsbundle.Include("~/Scripts/toastr.js");
            myjsbundle.Include("~/Scripts/jcarrollonlinev2.js");
            myjsbundle.Include("~/Scripts/MarkdownDeepLib.min.js");
            myjsbundle.Transforms.Add(scriptTransformer);
            myjsbundle.Orderer = nullOrderer;
            bundles.Add(myjsbundle);

            var blogjsbundle = new ScriptBundle("~/bundles/blogjsbundle");
            //blogjsbundle.Include("~/Scripts/blogscripts.js");
            bundles.Add(blogjsbundle);



            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            var modernizrBundle = new ScriptBundle("~/bundles/modernizr");
            modernizrBundle.Include("~/Scripts/modernizr-2.8.3");
            modernizrBundle.Transforms.Add(scriptTransformer);
            modernizrBundle.Orderer = nullOrderer;
            bundles.Add(modernizrBundle);

            var bootstrapBundle = new ScriptBundle("~/bundles/bootstrap");
            bootstrapBundle.Include("~/Scripts/bootstrap.js");
            bootstrapBundle.Include("~/Scripts/respond.js");
            bootstrapBundle.Transforms.Add(scriptTransformer);
            bootstrapBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapBundle);
        }
    }
}