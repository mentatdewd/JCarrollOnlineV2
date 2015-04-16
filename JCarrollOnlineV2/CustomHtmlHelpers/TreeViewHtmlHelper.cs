using JCarrollOnlineV2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.CustomHtmlHelpers
{
    public static class TreeViewHtmlHelper
    {
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
    }

    public static class HierarchicalListView1HtmlHelper
    {
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

            //sb.AppendFormat("<table id='{0}'>\r\n", treeId);

            //if (rootItems.Count() == 0)
            //{
            //    sb.AppendFormat("<tr>{0}</tr>", emptyContent);
            //}

            foreach (T item in rootItems)
            {
                RenderTr(sb, item, itemContent);
                AppendChildren(sb, item, childrenProperty, itemContent);
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

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
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
                    AppendChildren(sb, item, childrenProperty, itemContent);
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






















    public static class HiearchicalListViewHtmlHelper
    {
        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        public static string HierarchicalListView<T>(this HtmlHelper html, string tableId, string headerId, string rowId, string tdId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent) where T : class
        {
            return html.HierarchicalListView(tableId, headerId, rowId, tdId, rootItems, childrenProperty, itemContent, true, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>    
        public static string HierarchicalListView<T>(this HtmlHelper html, string tableId, string headerId, string rowId, string tdId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript) where T : class
        {
            return html.HierarchicalListView(tableId, headerId, rowId, tdId, rootItems, childrenProperty, itemContent, includeJavaScript, null);
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
        public static string HierarchicalListView<T>(this HtmlHelper html, string tableId, string headerId, string rowId, string tdId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript, string emptyContent) where T : class
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<table id='{0}'>\r\n<tr id='{1}'>", tableId, rowId);

            var properties = rootItems.First().GetType().GetProperty("Entity").GetValue(rootItems.First()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<string> propertyNames = new List<string>();

            foreach (var prop in properties)
            {
                DisplayAttribute attr = (DisplayAttribute)prop.GetCustomAttribute(typeof(DisplayAttribute));
                sb.AppendFormat("<th id='{0}'>{1}</th>", headerId, attr.Name);
                propertyNames.Add(prop.Name);
            }
            sb.AppendFormat("</tr>\r\n");

            if (rootItems.Count() == 0)
            {
                sb.AppendFormat("<tr id='{0}'>{1}</tr>", emptyContent);
            }

            foreach (T item in rootItems)
            {
                RenderTr(sb, item, itemContent, propertyNames, rowId, tdId);
                AppendChildren(sb, item, childrenProperty, itemContent, propertyNames, rowId, tdId);
            }

            sb.Append("</table>");
            if (includeJavaScript)
            {
                //                sb.AppendFormat(
                //                    @"<script type='text/javascript'>
                //                    $(document).ready(function() {{
                //                        $('#{0}').treeview({{ animated: 'fast' }});
                //                    }});
                //                </script>", treeId);
            }

            return sb.ToString();
        }

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, List<string> props, string rowId, string tdId)
        {
            var children = childrenProperty(root);
            if (children != null)
            {
                if (children.Count() == 0)
                {
                    return;
                }

                foreach (T item in children)
                {
                    RenderTr(sb, item, itemContent, props, rowId, tdId);
                    AppendChildren(sb, item, childrenProperty, itemContent, props, rowId, tdId);
                }
            }
            else
            {
            }
        }

        private static void RenderTr<T>(StringBuilder sb, T item, Func<T, string> itemContent, List<string> props, string rowId, string tdId)
        {
            sb.AppendFormat("<tr id='{0}' style='margin-left:20px;'>", rowId);
            foreach (var prop in props)
            {
                var entityProp = item.GetType().GetProperty("Entity");
                var entityValue = entityProp.GetValue(item);

                var depthProp = item.GetType().GetProperty("Depth");
                int depthValue = (int)depthProp.GetValue(item);

                var propProp = entityValue.GetType().GetProperty(prop);
                var propValue = propProp.GetValue(entityValue);

                sb.AppendFormat("<td id='{0}'>", tdId);//{1}", tdId, propValue);
                for (int i = 0; i < depthValue; i++ )
                {
                    sb.Append("<img src='images/rtable-fork.gif' alt='Line' title='Line' width='14' height='19'>");
                }
                sb.AppendFormat("{0}</td>", propValue);
            }
            sb.Append("</tr>\r\n");
        }
    }
}