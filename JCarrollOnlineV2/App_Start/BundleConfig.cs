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

            bundles.Add(new ScriptBundle("~/bundles/signalr")
                .Include("~/Scripts/jquery.signalR-2.4.3.js"));
        }
    }
}