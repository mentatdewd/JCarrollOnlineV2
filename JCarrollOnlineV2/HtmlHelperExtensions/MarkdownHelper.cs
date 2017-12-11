﻿using MarkdownDeep;
using System;
using System.Globalization;
using System.Text;

namespace JCarrollOnlineV2.HtmlHelpers
{
    public static partial class MarkdownHelper
    {
        public static string Language { get; set; }

        public static string FormatCodeBlock(Markdown md, string code)
        {
            // Wrap the code in <pre><code> as the default MarkdownDeep.NET implementation does, but add a class of
            // "prettyprint" which is what Google Code Prettify uses to identify code blocks.
            // http://google-code-prettify.googlecode.com/svn/trunk/README.html
            var sb = new StringBuilder();
            var fixedString = String.Format(CultureInfo.InvariantCulture, "<pre><code class=\"language-{0}\">", Language);
            sb.Append(fixedString);
            sb.Append(code);
            sb.Append("</code></pre>\n\n");
            return sb.ToString();
        }
    }
}