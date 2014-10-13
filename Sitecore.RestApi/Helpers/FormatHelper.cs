using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.RestApi.Converters;
using Sitecore.RestApi.Formatters;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Helpers
{
    public class FormatHelper
    {
        private ItemProfile ItemProfile { get; set; }

        public FormatHelper(ItemProfile profile)
        {
            ItemProfile = profile;
        }

        public async Task<object> FormatObjectAsync(object source)
        {
            var propertyNames = source is Item ? ItemProfile.ItemPropertyNames : ItemProfile.FieldPropertyNames;
            var type = source.GetType();
            var tasks = propertyNames.Select(
                name => 
                    Task.FromResult(
                        FormatProperty(
                            new PropertyArgs
                                {
                                    Name = name, 
                                    Info = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance), 
                                    Source = source
                                }))).ToList();

            var result = await Task.WhenAll(tasks);

            return result.ToDictionary(n => n.Key, n => n.Value);
        }

        private KeyValuePair<string, object> FormatProperty(PropertyArgs propertyArgs)
        {
            var source = propertyArgs.Source;
            var propertyName = propertyArgs.Name;
            var name = ItemProfile.CamelCaseName ? CamelCaseName(propertyName) : propertyName;

            //Try property formatting if available
            if (source is Item && ItemProfile.PropertyFormatters.ContainsKey(name.ToLower()))
            {
                var prop = InvokeFormatter(ItemProfile.PropertyFormatters[name.ToLower()], propertyArgs);

                if (prop != null)
                {
                    return new KeyValuePair<string, object>(name, prop);
                }
            }

            var valueFormatters = GetValueFormatters(source);
            var propValue = propertyArgs.Info != null ? (propertyArgs.GetValue() ?? string.Empty) : string.Empty;

            var result = FormatValue(propertyArgs, valueFormatters).Result ?? propValue;

            return new KeyValuePair<string, object>(name, result);
        }

        private IEnumerable<FormatterArgs> GetValueFormatters(object source)
        {
            var valueFormatters =
                source is Item
                    ? ItemProfile.ValueFormatters.Where(n => !(typeof(IFieldValueFormatter).IsAssignableFrom(n.Type)))
                    : ItemProfile.ValueFormatters;

            return valueFormatters;
        }

        private static string CamelCaseName(string originalName)
        {
            var n = originalName;
            return n.Substring(0, 1).ToLower() + n.Substring(1, n.Length - 1);
        }

        private static async Task<object> FormatValue(PropertyArgs propertyArgs, IEnumerable<FormatterArgs> formatterArgs)
        {
            if (propertyArgs.Source is Field && propertyArgs.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
            {
                var fvQuery = formatterArgs.Where(n => typeof (IFieldValueFormatter).IsAssignableFrom(n.Type));
                return await InvokeFormattersAsync(propertyArgs, fvQuery);
            }

            var query = formatterArgs.Where(n => !(typeof(IFieldValueFormatter).IsAssignableFrom(n.Type)));
            return await InvokeFormattersAsync(propertyArgs, query);
        }

        private static async Task<object> InvokeFormattersAsync(PropertyArgs propertyArgs, IEnumerable<FormatterArgs> formatters)
        {
            var tasks = formatters.Select(n => Task.FromResult(InvokeFormatter(n, propertyArgs))).ToList();
            var results = await Task.WhenAll(tasks);

            return results.SingleOrDefault(n => n != null);
        }

        private static object InvokeFormatter(FormatterArgs formatterArgs, PropertyArgs propertyArgs)
        {
            var source = Activator.CreateInstance(formatterArgs.Type);
            formatterArgs.Method.Invoke(source, new object[] {propertyArgs});

            var formatterSource = source as Formatter;

            return formatterSource.IsFormatted ? formatterSource.FormattedObject : null;
        }

        public static IEnumerable<FormatterArgs> GetFormatters(IEnumerable<Item> valueFormatterItems)
        {
            var valueFormaters = valueFormatterItems.Select(n => Type.GetType(n["Type"])).Where(n => n != null);
            Func<ParameterInfo, bool> isPropertyArgs = info => typeof(PropertyArgs).IsAssignableFrom(info.ParameterType);
            
            var query = valueFormaters.Select(n =>
            {
                var obj = Activator.CreateInstance(n);
                var method =
                    n.GetMethods()
                     .SingleOrDefault(m => m.GetParameters().SingleOrDefault(isPropertyArgs) != null);

                if (method == null) return null;

                return new FormatterArgs
                {
                    Type = n,
                    Method = method
                };
            }).Where(n => n != null);

            return query;
        }
    }
}