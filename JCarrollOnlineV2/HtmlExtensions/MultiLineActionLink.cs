using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace JCarrollOnlineV2.HtmlExtensions
{
    public static class HtmlExtensions
    {
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
    }
}