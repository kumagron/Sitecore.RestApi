using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Helpers
{
    internal class PropertyConverter
    {
        public static string ConvertId(Item item)
        {
            return item.ID.Guid.ToString("D");
        }

        public static dynamic ConvertKupal(Item item)
        {
            return item.Paths.Path;
        }

        private static string CamelCaseName(string originalName)
        {
            var n = originalName;
            return n.Substring(0, 1).ToLower() + n.Substring(1, n.Length - 1);
        }

        public static KeyValuePair<string, object> Convert(object source, string propertyName, bool forceCamelCase,
                                              IEnumerable<Formatter> valueFormatters,
                                              Func<object, string, dynamic> convertValueFunc,
                                              Func<object, string, string, IEnumerable<Formatter>, dynamic> formatValueFunc)
        {
            var type = source.GetType();
            var prop = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var name = forceCamelCase ? CamelCaseName(propertyName) : propertyName;

            //if (prop == null) return new KeyValuePair<string, object>(name, null);

            var value = prop != null ? prop.GetValue(source) : string.Empty;
            var convertedValue = convertValueFunc != null ? convertValueFunc(source, name) : null;
            var properValue =
                convertedValue ??
                (formatValueFunc(source, name, value.ToString(), valueFormatters) ??
                 value);

            return new KeyValuePair<string, object>(name, properValue);
        }
    }
}