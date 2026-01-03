using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace JCarrollOnlineV2.Helpers
{
    public static class HandlebarsEmailHelper
    {
        private static readonly Dictionary<string, HandlebarsTemplate<object, object>> _templateCache 
            = new Dictionary<string, HandlebarsTemplate<object, object>>();
        
        private static readonly object _lock = new object();

        public static string RenderTemplate(string templateName, object model)
        {
            HandlebarsTemplate<object, object> template = GetOrCompileTemplate(templateName);
            return template(model);
        }

        private static HandlebarsTemplate<object, object> GetOrCompileTemplate(string templateName)
        {
            lock (_lock)
            {
                if (_templateCache.ContainsKey(templateName))
                {
                    return _templateCache[templateName];
                }

                string templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
                string templateFilePath = Path.Combine(templateFolderPath, $"{templateName}.hbs");

                if (!File.Exists(templateFilePath))
                {
                    throw new FileNotFoundException($"Template file not found: {templateFilePath}");
                }

                string templateContent = File.ReadAllText(templateFilePath);
                HandlebarsTemplate<object, object> template = Handlebars.Compile(templateContent);
                
                _templateCache[templateName] = template;
                
                return template;
            }
        }

        public static void ClearCache()
        {
            lock (_lock)
            {
                _templateCache.Clear();
            }
        }
    }
}