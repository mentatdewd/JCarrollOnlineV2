using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JCarrollOnlineV2.HtmlHelperExtensions
{
    public static class HtmlHelperExtensions
    {
        public static async Task<int> GetPostCountAsync(this HtmlHelper helper, string author)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            int count = await db.ForumThreadEntries.Where(i => i.AuthorId == author).AsQueryable().CountAsync();
            return count;
        }
        public static async Task<int> GetParentPostNumberAsync(this HtmlHelper helper, int? forumThreadEntryId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            if (forumThreadEntryId != null)
            {
                ForumThreadEntry fte = await db.ForumThreadEntries.FindAsync(forumThreadEntryId);
                return fte.PostNumber;
            }
            else
                return 1;
        }
        public static async Task<DateTime> GetLatestThreadPostDateAsync(this HtmlHelper helper, int? rootId)
        {
            if (rootId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                ForumThreadEntry fte = await db.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static string ExternalLink(this HtmlHelper helper, string URI, string label)
        {
            return string.Format("<a href='{0}'>{1}</a>", URI, label);
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
        public static string Pluralize(this HtmlHelper html, int count, string word)
        {
            string retWord = word;
            if (count == 0 || count > 1)
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
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            return html.TreeView(treeId, rootItems, childrenProperty, itemContent, true, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>    
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript)
        {
            return html.TreeView(treeId, rootItems, childrenProperty, itemContent, includeJavaScript, null);
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
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript, string emptyContent)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<ul id='{0}'>\r\n", treeId);

            if (rootItems.Count() == 0)
            {
                sb.AppendFormat("<li>{0}</li>", emptyContent);
            }

            foreach (T item in rootItems)
            {
                RenderLi(sb, item, itemContent);
                AppendChildren(sb, item, childrenProperty, itemContent);
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

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            var children = childrenProperty(root);
            if (children != null)
            {
                if (children.Count() == 0)
                {
                    sb.AppendLine("</li>");
                    return;
                }

                sb.AppendLine("\r\n<ul>");
                foreach (T item in children)
                {
                    RenderLi(sb, item, itemContent);
                    AppendChildren(sb, item, childrenProperty, itemContent);
                }

                sb.AppendLine("</ul></li>");
            }
            else
            {
                sb.AppendLine("</li>");
            }
        }

        private static void RenderLi<T>(StringBuilder sb, T item, Func<T, string> itemContent)
        {
            sb.AppendFormat("<li>{0}", itemContent(item));
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
