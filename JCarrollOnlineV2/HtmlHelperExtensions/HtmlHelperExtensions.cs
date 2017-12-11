using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace JCarrollOnlineV2.HtmlHelperExtensions
{
    /// <summary>
    /// Helper class for transforming Markdown.
    /// </summary>

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString BlogShowCommentsButton(this HtmlHelper helper, int blogItemId)
        {
            //<button id="blogCommentsButton" class="commentButton">Comments</button>
            var builder = new TagBuilder("button");

            builder.MergeAttribute("class", "showCommentsButton");
            builder.MergeAttribute("id", "showCommentsButton" + blogItemId);
            builder.MergeAttribute("data-BlogItemId", "commentId" + blogItemId);
            builder.SetInnerText("Comments");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BlogShowCommentsDialogButton(this HtmlHelper helper, int blogItemId)
        {
            //<button id="blogCommentsButton" class="commentButton">Comments</button>
            var builder = new TagBuilder("button");

            builder.MergeAttribute("class", "ShowCommentsDialogButton btn btn-large btn-primary");
            builder.MergeAttribute("id", "showCommentsDialogButton" + blogItemId);
            builder.MergeAttribute("data-BlogItemId", blogItemId.ToString());
            builder.SetInnerText("Add Comment");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BlogCommentsDivTag(this HtmlHelper helper, int blogItemId)
        {
            var builder = new StringBuilder("<div id= \"");

            builder.Append("commentId" + blogItemId);
            builder.Append("\" class=\"");
            builder.Append("blogComments");
            builder.Append("\">");
            return new MvcHtmlString(builder.ToString());
        }

        public static string ExternalLink(this HtmlHelper helper, string URI, string label)
        {
            return string.Format("<a href='{0}'>{1}</a>", URI, label);
        }

        public static Regex rxExtractLanguage = new Regex("({{(.+)}}[\r\n])", RegexOptions.Compiled);
        public static MvcHtmlString Markdown(this HtmlHelper helper, string input)
        {
            // Try to extract the language from the first line
            var match = rxExtractLanguage.Match(input);
            string language = null;

            if (match.Success)
            {
                // Save the language
                var g = (Group)match.Groups[2];
                language = g.ToString();

                // Remove the first line
                input = input.Replace(match.ToString(), "");// (match.Groups[1].Length);
            }
            if(language != null)
                MarkdownHelper.Language = language.Replace("{{","").Replace("}}","");

            var md = new MarkdownDeep.Markdown();
            md.FormatCodeBlock = MarkdownHelper.FormatCodeBlock;
            md.ExtraMode = true;
            md.HtmlClassTitledImages = "markdown_image";
            return new MvcHtmlString(md.Transform(input));
        }
        private static string FormatCodeBlock(MarkdownDeep.Markdown md, string code)
        {
            // Wrap the code in <pre><code> as the default MarkdownDeep.NET implementation does, but add a class of
            // "prettyprint" which is what Google Code Prettify uses to identify code blocks.
            // http://google-code-prettify.googlecode.com/svn/trunk/README.html
            var sb = new StringBuilder();
            sb.Append("<pre class=\"prettyprint\"><code>");
            sb.Append(code);
            sb.Append("</code></pre>\n\n");
            return sb.ToString();
        }

        public static MvcHtmlString MultiLineActionLink(
            this HtmlHelper html,
            string linkText,
            string action,
            string controller,
            object routeValues,
            object htmlAttributes
        )
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var url = urlHelper.Action(action, controller, routeValues);
            var anchor = new TagBuilder("a");
            anchor.InnerHtml = linkText;
            anchor.Attributes["href"] = url;
            anchor.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(anchor.ToString());
        }
        public static string Pluralize(this HtmlHelper html, int? count, string word)
        {
            string retWord = word;
            var cnt = count ?? 0;
            if (cnt == 0 || cnt > 1)
            {
                PluralizationService ps = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
                retWord = ps.Pluralize(word);
            }
            return retWord;
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        public static string TreeView<T>(this HtmlHelper html, string treeId, string treeItemId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            return html.TreeView(treeId, treeItemId, rootItems, childrenProperty, itemContent, true, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>    
        public static string TreeView<T>(this HtmlHelper html, string treeId, string treeItemId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript)
        {
            return html.TreeView(treeId, treeItemId, rootItems, childrenProperty, itemContent, includeJavaScript, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>
        /// <param name="emptyContent">Content to be rendered when the tree is empty</param>
        /// <param name="includeJavaScript">If true, output will automatically into the JavaScript to turn the ul into the treeview</param>    
        public static string TreeView<T>(this HtmlHelper html, string treeId, string treeItemId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript, string emptyContent)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<ul id='{0}'>\r\n", treeId);

            if (rootItems.Count() == 0)
            {
                sb.AppendFormat("<li id='{0}'>{1}</li>", treeItemId, emptyContent);
            }

            foreach (T item in rootItems)
            {
                RenderLi(sb, item, itemContent, treeItemId);
                AppendChildren(sb, item, childrenProperty, itemContent, treeId, treeItemId);
            }

            sb.AppendLine("</ul>");

            if (includeJavaScript)
            {
                sb.AppendFormat(
                    @"<script type='text/javascript'>
                    $(document).ready(function() {{
                        $('#{0}').treeview({{ animated: 'fast' }});
                    }});
                </script>", treeId);
            }

            return sb.ToString();
        }

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, string treeId, string treeItemId)
        {
            var children = childrenProperty(root);
            if (children != null)
            {
                if (children.Count() == 0)
                {
                    sb.AppendLine("</li>");
                    return;
                }

                sb.AppendFormat("\r\n<ul id='{0}'>", treeId);
                foreach (T item in children)
                {
                    RenderLi(sb, item, itemContent, treeItemId);
                    AppendChildren(sb, item, childrenProperty, itemContent, treeId, treeItemId);
                }

                sb.AppendLine("</ul></li>");
            }
            else
            {
                sb.AppendLine("</li>");
            }
        }

        private static void RenderLi<T>(StringBuilder sb, T item, Func<T, string> itemContent, string treeItemId)
        {
            sb.AppendFormat("<li id='{0}'>{1}", treeItemId, itemContent(item));
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        public static string HierarchicalListView1<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            return html.HierarchicalListView1(treeId, rootItems, childrenProperty, itemContent, true, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>    
        public static string HierarchicalListView1<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript)
        {
            return html.HierarchicalListView1(treeId, rootItems, childrenProperty, itemContent, includeJavaScript, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>
        /// <param name="emptyContent">Content to be rendered when the tree is empty</param>
        /// <param name="includeJavaScript">If true, output will automatically into the JavaScript to turn the ul into the treeview</param>    
        public static string HierarchicalListView1<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript, string emptyContent)
        {
            StringBuilder sb = new StringBuilder();

            foreach (T item in rootItems)
            {
                RenderTr(sb, item, itemContent);
                AppendListChildren(sb, item, childrenProperty, itemContent);
            }

            sb.AppendLine("</tr>");

            if (includeJavaScript)
            {
                sb.AppendFormat(
                    @"<script type='text/javascript'>
                    $(document).ready(function() {{
                        $('#{0}').treeview({{ animated: 'fast' }});
                    }});
                </script>", treeId);
            }

            return sb.ToString();
        }

        private static void AppendListChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            var children = childrenProperty(root);
            if (children != null)
            {
                if (children.Count() == 0)
                {
                    sb.AppendLine("</tr>");
                    return;
                }

                //sb.AppendLine("\r\n<tr>");
                foreach (T item in children)
                {
                    RenderTr(sb, item, itemContent);
                    AppendListChildren(sb, item, childrenProperty, itemContent);
                }

                sb.AppendLine("</tr>");
            }
            else
            {
                sb.AppendLine("</tr>");
            }
        }

        private static void RenderTr<T>(StringBuilder sb, T item, Func<T, string> itemContent)
        {
            sb.AppendFormat("<tr>{0}", itemContent(item));
        }
    }
}
