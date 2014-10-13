﻿using System;
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
        public static async Task<JToken> GenerateJTokenAsync(object source, ItemProfile profile)
        {
            var propertyNames = source is Item ? profile.ItemPropertyNames : profile.FieldPropertyNames;
            var type = source.GetType();
            var tasks = propertyNames.Select(n =>
                Task.FromResult(
                    FormatProperty(
                        new PropertyArgs { 
                            Name = n, 
                            Info = type.GetProperty(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance),
                            Source = source
                        }, profile)));

            var results = await Task.WhenAll(tasks);

            var ret = new JObject();

            foreach (var result in results)
            {
                var value = result.Value ?? String.Empty;

                ret.Add(result.Key, (value as JToken) ?? JToken.FromObject(value));
            }

            return ret;
        }

        private static KeyValuePair<string, object> FormatProperty(PropertyArgs propertyArgs, ItemProfile profile)
        {
            var source = propertyArgs.Source;
            var valueFormatters = source is Item
                                      ? profile.ValueFormatters.Where(n => !(typeof(IFieldValueFormatter).IsAssignableFrom(n.Type)))
                                      : profile.ValueFormatters;
            var formattedProperty =
                source is Item
                    ? FormatItemProperty(propertyArgs, profile)
                    : null;

            var propertyName = propertyArgs.Name;
            var name = profile.CamelCaseName ? CamelCaseName(propertyName) : propertyName;
            var value = propertyArgs.Info != null ? (propertyArgs.GetValue() ?? string.Empty) : string.Empty;
            
            var result =
                formattedProperty ??
                ((source is Item ? FormatValues(propertyArgs, valueFormatters).Result : FormatFieldValues(propertyArgs, valueFormatters).Result) ??
                 value);

            return new KeyValuePair<string, object>(name, result);
        }

        private static JToken FormatItemProperty(PropertyArgs propertyArgs, ItemProfile profile)
        {
            if (!(propertyArgs.Source is Item)) return null;

            var key = propertyArgs.Name.ToLower();

            if (!profile.PropertyFormatters.ContainsKey(key)) return null;

            var formatter = profile.PropertyFormatters[key];
            var result = InvokeFormatter(formatter, propertyArgs);

            return result as JToken ?? JToken.FromObject(result);
        }

        private static string CamelCaseName(string originalName)
        {
            var n = originalName;
            return n.Substring(0, 1).ToLower() + n.Substring(1, n.Length - 1);
        }

        private static async Task<dynamic> FormatValues(PropertyArgs propertyArgs, IEnumerable<FormatterArgs> formatterArgs)
        {
            var task = await InvokeFormattersAsync(propertyArgs, formatterArgs);

            return task;
        }

        private static async Task<dynamic> FormatFieldValues(PropertyArgs propertyArgs, IEnumerable<FormatterArgs> formatterArgs)
        {
            var propertyIsValue = propertyArgs.Name.Equals("value", StringComparison.OrdinalIgnoreCase);
            var query = propertyIsValue
                            ? formatterArgs.Where(n => typeof(IFieldValueFormatter).IsAssignableFrom(n.Type))
                            : formatterArgs.Where(n => !(typeof(IFieldValueFormatter).IsAssignableFrom(n.Type)));

            if (!query.Any()) return propertyArgs.GetValue();

            if (propertyIsValue)
            {
                var task = await InvokeFormattersAsync(propertyArgs, query);
                
                return task;
            }

            return FormatValues(propertyArgs, query);
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