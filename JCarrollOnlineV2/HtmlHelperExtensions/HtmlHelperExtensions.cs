﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace JCarrollOnlineV2.HtmlHelpers
{
    /// <summary>
    /// Helper class for transforming Markdown.
    /// </summary>

    public static class HtmlExtensions
    {
        public static MvcHtmlString BlogShowCommentsButton(this HtmlHelper helper, int blogItemId)
        {
            //<button id="blogCommentsButton" class="commentButton">Comments</button>
            TagBuilder builder = new TagBuilder("button");

            builder.MergeAttribute("class", "showCommentsButton");
            builder.MergeAttribute("id", "showCommentsButton" + blogItemId);
            builder.MergeAttribute("data-BlogItemId", "commentId" + blogItemId);
            builder.SetInnerText("Comments");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BlogShowCommentsDialogButton(this HtmlHelper helper, int blogItemId)
        {
            //<button id="blogCommentsButton" class="commentButton">Comments</button>
            TagBuilder builder = new TagBuilder("button");

            builder.MergeAttribute("class", "ShowCommentsDialogButton btn btn-large btn-primary");
            builder.MergeAttribute("id", "showCommentsDialogButton" + blogItemId);
            builder.MergeAttribute("data-BlogItemId", blogItemId.ToString(CultureInfo.InvariantCulture));
            builder.SetInnerText("Add a Comment");
            return new MvcHtmlString(builder.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Div")]
        public static MvcHtmlString BlogCommentsDivTag(this HtmlHelper helper, int blogItemId)
        {
            StringBuilder builder = new StringBuilder("<div id= \"");

            builder.Append("commentId" + blogItemId);
            builder.Append("\" class=\"");
            builder.Append("blogComments");
            builder.Append("\">");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BlogFormDivTag(this HtmlHelper helper, int blogItemId)
        {
            StringBuilder builder = new StringBuilder("<div id=\"comment-dialog" + blogItemId + "\" class=\"commentFormClassDiv\" title=\"Add Comment\">");

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BlogEndDiv(this HtmlHelper helper)
        {
            return new MvcHtmlString("<\\div>");
        }

        public static string ExternalLink(this HtmlHelper helper, Uri uri, string label)
        {
            return string.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", uri, label);
        }

        private static Regex _rxExtractLanguage = new Regex("({{(.+)}}[\r\n])", RegexOptions.Compiled);

        public static MvcHtmlString Markdown(this HtmlHelper helper, string input)
        {
            // TODO: fix this, for some reason input is null and it shouldn't throw
            if (input == null)
            {
                return new MvcHtmlString("");
                //throw new ArgumentNullException(nameof(input));
            }
            // Try to extract the language from the first line
            Match match = _rxExtractLanguage.Match(input);
            string language = null;

            if (match.Success)
            {
                // Save the language
                Group g = (Group)match.Groups[2];
                language = g.ToString();

                // Remove the first line
                input = input.Replace(match.ToString(), "");// (match.Groups[1].Length);
            }
            if(language != null)
                MarkdownHelper.Language = language.Replace("{{","").Replace("}}","");

            MarkdownDeep.Markdown markDown = new MarkdownDeep.Markdown
            {
                FormatCodeBlock = MarkdownHelper.FormatCodeBlock,
                ExtraMode = true,
                HtmlClassTitledImages = "markdown_image"
            };

            return new MvcHtmlString(markDown.Transform(input));
        }

        //private static string FormatCodeBlock(MarkdownDeep.Markdown md, string code)
        //{
        //    // Wrap the code in <pre><code> as the default MarkdownDeep.NET implementation does, but add a class of
        //    // "prettyprint" which is what Google Code Prettify uses to identify code blocks.
        //    // http://google-code-prettify.googlecode.com/svn/trunk/README.html
        //    var sb = new StringBuilder();
        //    sb.Append("<pre class=\"prettyprint\"><code>");
        //    sb.Append(code);
        //    sb.Append("</code></pre>\n\n");
        //    return sb.ToString();
        //}

        public static MvcHtmlString MultilineActionLink(
            this HtmlHelper html,
            string linkText,
            string action,
            string controller,
            object routeValues,
            object htmlAttributes
        )
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            UrlHelper urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string url = urlHelper.Action(action, controller, routeValues);
            TagBuilder anchor = new TagBuilder("a")
            {
                InnerHtml = linkText
            };

            anchor.Attributes["href"] = url;
            anchor.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(anchor.ToString());
        }
        public static string Pluralize(this HtmlHelper html, int? count, string word)
        {
            string retWord = word;
            int cnt = count ?? 0;
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
            _ = includeJavaScript;

            if (rootItems == null)
            {
                throw new ArgumentNullException(nameof(rootItems));
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "<ul id='{0}'>\r\n", treeId);

            if (!rootItems.Any())
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "<li id='{0}'>{1}</li>", treeItemId, emptyContent);
            }

            foreach (T item in rootItems)
            {
                if (itemContent != null && childrenProperty != null)
                {
                    RenderLi(sb, item, itemContent, treeItemId);
                    AppendChildren(sb, item, childrenProperty, itemContent, treeId, treeItemId);
                }
            }

            sb.AppendLine("</ul>");

            //if (includeJavaScript)
            //{
            //    sb.AppendFormat(CultureInfo.InvariantCulture,
            //        @"<script type='text/javascript'>
            //        $(document).ready(function() {{
            //            $('#{0}').treeview({{ animated: 'fast' }});
            //        }});
            //    </script>", treeId);
            //}

            return sb.ToString();
        }

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, string treeId, string treeItemId)
        {
            IEnumerable<T> children = childrenProperty(root);
            if (children != null)
            {
                if (!children.Any())
                {
                    sb.AppendLine("</li>");
                    return;
                }

                sb.AppendFormat(CultureInfo.InvariantCulture, "\r\n<ul id='{0}'>", treeId);
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
            sb.AppendFormat(CultureInfo.InvariantCulture, "<li id='{0}'>{1}", treeItemId, itemContent(item));
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
            _ = treeId;
            _ = includeJavaScript;
            _ = emptyContent;

            if (rootItems == null)
            {
                throw new ArgumentNullException(nameof(rootItems));
            }

            StringBuilder sb = new StringBuilder();

            foreach (T item in rootItems)
            {
                if (itemContent != null && childrenProperty != null)
                {
                    RenderTr(sb, item, itemContent);
                    AppendListChildren(sb, item, childrenProperty, itemContent);
                }
            }

            sb.AppendLine("</tr>");

            //if (includeJavaScript)
            //{
            //    sb.AppendFormat(CultureInfo.InvariantCulture,
            //        @"<script type='text/javascript'>
            //        $(document).ready(function() {{
            //            $('#{0}').treeview({{ animated: 'fast' }});
            //        }});
            //    </script>", treeId);
            //}

            return sb.ToString();
        }

        private static void AppendListChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            IEnumerable<T> children = childrenProperty(root);
            if (children != null)
            {
                if (!children.Any())
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
            sb.AppendFormat(CultureInfo.InvariantCulture, "<tr>{0}", itemContent(item));
        }
    }
}
