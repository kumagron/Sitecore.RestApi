using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.RestApi.Converters;
using Sitecore.RestApi.Formatters;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Helpers
{
    public class FormatHelper
    {
        public static async Task<JToken> GenerateJTokenAsync(object source, ItemProfile profile,
                                        Func<object, string, dynamic> convertPropertyFunc,
                                        Func<PropertyArgs, IEnumerable<FormatterArgs>, dynamic> formatValueFunc)
        {
            var propertyNames = source is Item ? profile.ItemPropertyNames : profile.FieldPropertyNames;
            var valueFormatters = source is Item
                                      ? profile.ValueFormatters.Where(n => !(n.Source is IFieldValueFormatter))
                                      : profile.ValueFormatters;

            var tasks = propertyNames.Select(n =>
                Task.FromResult(
                    FormatProperty(source, n, profile.CamelCaseName, valueFormatters, convertPropertyFunc, formatValueFunc)));

            var results = await Task.WhenAll(tasks);

            var ret = new JObject();

            foreach (var result in results)
            {
                var value = result.Value ?? String.Empty;

                ret.Add(result.Key, (value as JToken) ?? JToken.FromObject(value));
            }

            return ret;
        }

        public static KeyValuePair<string, object> FormatProperty(object source, string propertyName, bool forceCamelCase,
                                              IEnumerable<FormatterArgs> valueFormatters,
                                              Func<object, string, dynamic> convertValueFunc,
                                              Func<PropertyArgs, IEnumerable<FormatterArgs>, dynamic> formatValueFunc)
        {
            var type = source.GetType();
            var prop = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var propertyArgs = new PropertyArgs {Source = source, Property = prop};
            var name = forceCamelCase ? CamelCaseName(propertyName) : propertyName;

            //if (prop == null) return new KeyValuePair<string, object>(name, null);

            var value = prop != null ? prop.GetValue(source) : String.Empty;
            var convertedValue = convertValueFunc != null ? convertValueFunc(source, name) : null;

            var properValue =
                convertedValue ??
                (formatValueFunc(propertyArgs, valueFormatters) ??
                 value);

            return new KeyValuePair<string, object>(name, properValue);
        }

        private static string CamelCaseName(string originalName)
        {
            var n = originalName;
            return n.Substring(0, 1).ToLower() + n.Substring(1, n.Length - 1);
        }

        public static dynamic FormatValues(PropertyArgs propertyArgs, IEnumerable<FormatterArgs> formatterArgs)
        {
            //var parameters = new object[] { source, property };
            var task = InvokeFormattersAsync(formatterArgs, propertyArgs);

            return task.Result;
        }

        public static dynamic FormatFieldValues(PropertyArgs propertyArgs,
                                                 IEnumerable<FormatterArgs> formatterArgs)
        {
            var propertyIsValue = propertyArgs.Property.Name.Equals("value", StringComparison.OrdinalIgnoreCase);

            var query = propertyIsValue
                            ? formatterArgs.Where(n => n.Source is IFieldValueFormatter)
                            : formatterArgs.Where(n => !(n.Source is IFieldValueFormatter));

            if (!query.Any()) return propertyArgs.Property.GetValue(propertyArgs.Source);

            if (propertyIsValue)
            {
                var task = InvokeFormattersAsync(query, propertyArgs);

                return task.Result;
            }

            return FormatValues(propertyArgs, query);
        }

        public static async Task<object> InvokeFormattersAsync(IEnumerable<FormatterArgs> formatters, PropertyArgs propertyArgs)
        {
            Func<FormatterArgs, object> formatValue = formatter =>
            {
                var source = formatter.Source as Formatter;
                formatter.Method.Invoke(source, new[] {propertyArgs.Source, propertyArgs.Property});

                return source.IsFormatted ? source.FormattedObject : null;
            };

            var tasks = formatters.Select(n => Task.FromResult(formatValue(n))).ToList();

            var results = await Task.WhenAll(tasks);

            return results.SingleOrDefault(n => n != null);
        }

        public static IEnumerable<FormatterArgs> GetFormatters(IEnumerable<Item> valueFormatterItems)
        {
            var valueFormaters = valueFormatterItems.Select(n => Type.GetType(n["Type"])).Where(n => n != null);

            var formatValueParams = typeof(IValueFormatter).GetMethod("FormatValue").GetParameters();

            var query = valueFormaters.Select(n =>
            {
                var obj = Activator.CreateInstance(n);
                var method =
                    n.GetMethods()
                     .SingleOrDefault(
                         m =>
                         m.Name.Equals("FormatValue",
                                       StringComparison.OrdinalIgnoreCase) ||
                         formatValueParams.Equals(m.GetParameters()));

                if (method == null) return null;

                return new FormatterArgs
                {
                    Source = obj,
                    Method = method
                };
            }).Where(n => n != null);

            return query;
        }
    }
}