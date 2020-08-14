using System.Web.Optimization;

using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;


namespace JCarrollOnlineV2
{
    public static class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null)
            {
                throw new System.ArgumentNullException(nameof(bundles));
            }

            bundles.UseCdn = true;

            NullBuilder nullBuilder = new NullBuilder();
            StyleTransformer styleTransformer = new StyleTransformer();
            NullOrderer nullOrderer = new NullOrderer();

            BundleResolver.Current = new CustomBundleResolver();

            StyleBundle cssBundle = new StyleBundle("~/bundles/css");
            cssBundle.Include(
                "~/Content/css/bootstrap/bootstrap.min.css",
                "~/Content/css/Prism.css",
                "~/Content/css/PagedList.css",
                "~/Content/css/themes/base/jquery-ui.min.css",
                "~/Content/css/MarkdownDeep.css",
                "~/Content/css/toastr.min.css",
                "~/Content/css/Site.css");

            cssBundle.Builder = nullBuilder;
            cssBundle.Transforms.Add(styleTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(cssBundle);

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Content/scripts/jquery/jquery-3.5.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Content/scripts/jquery/jquery.validate.js")
                .Include("~/Content/scripts/jquery/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/scripts/bootstrap/bootstrap.js",
                      "~/Content/scripts/bootstrap/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/jcarrollonlinev2").Include(
                      "~/Content/scripts/jcarrollonlinev2.js"));

            bundles.Add(new ScriptBundle("~/bundles/markdowndeep")
                .Include("~/Content/scripts/markdowndeep/MarkdownDeep.js")
                .Include("~/Content/scripts/markdowndeep/MarkdownDeepEditor.js")
                .Include("~/Content/scripts/markdowndeep/MarkdownDeepEditorUI.js")
                .Include("~/Content/scripts/markdowndeep/MarkdownDeepLib.js"));

            bundles.Add(new ScriptBundle("~/bundles/livestamp")
                .Include("~/Content/scripts/moment.js")
                .Include("~/Content/scripts/livestamp.js"));

            bundles.Add(new ScriptBundle("~/bundles/prism")
                .Include("~/Content/scripts/prism.js"));

            //var jqueryBundle = new ScriptBundle("~/bundles/jquery");
            //jqueryBundle.Include("~/Scripts/jquery/jquery.min.js");
            //jqueryBundle.Include("~/Scripts/bootstrap/bootstrap.min.js");
            //jqueryBundle.Include("~/Scripts/less.min.js");
            //jqueryBundle.Include("~/Scripts/moment.min.js");
            //jqueryBundle.Include("~/Scripts/livestamp.js");
            //jqueryBundle.Include("~/Scripts/jquery.unobtrusive-ajax.min.js");
            ////jqueryBundle.Include("~/Scripts/jquery-ui/jquery-ui.min.js");
            //jqueryBundle.Include("~/Scripts/jquery.validate.min.js");
            //jqueryBundle.Include("~/Scripts/jquery.validate.unobtrusive.min.js");
            //jqueryBundle.Include("~/Scripts/jquery.validate.unobtrusive.ajax.min.js");
            //jqueryBundle.Include("~/Scripts/prism.js");
            //jqueryBundle.Include("~/Scripts/toastr.min.js");
            //jqueryBundle.Include("~/Scripts/MarkdownDeepLib.min.js");
            //jqueryBundle.Include("~/Scripts/jcarrollonlinev2.js");
            //jqueryBundle.Include("~/Scripts/modernizr-2.8.3.js");
            //jqueryBundle.Transforms.Add(scriptTransformer);
            //jqueryBundle.Orderer = nullOrderer;
            //bundles.Add(jqueryBundle);

            //var angularBundle = new ScriptBundle("~/bundles/angularbundle");
            //angularBundle.Include("~/Content/lib/angular.min.js");


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        }
    }
}