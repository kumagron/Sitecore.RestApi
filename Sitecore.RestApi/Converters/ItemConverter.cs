using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.RestApi.Formatters;
using Sitecore.RestApi.Helpers;
using Sitecore.RestApi.Models;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;

namespace Sitecore.RestApi.Converters
{
    public class ItemConverter
    {
        private ItemProfile ItemProfile { get; set; }
        private IEnumerable<Formatter> ValueFormatters { get; set; }
        private Dictionary<string, Func<Item, object>> PropertyConverters { get; set; }

        public ItemConverter(ItemProfile itemProfile)
        {
            ItemProfile = itemProfile;
            ValueFormatters = GetFormatters(ItemProfile.ValueFormatters);
            PropertyConverters = new Dictionary<string, Func<Item, object>>
                                         {
                                             {"id", PropertyConverter.ConvertId},
                                             {"fields", ConvertItemFields},
                                             {"kupal", PropertyConverter.ConvertKupal}
                                         };
        }

        public JToken ConvertItems(IEnumerable<Item> source)
        {
            var tasks = source.Select(n => Task.FromResult(ConvertItem(n))).ToArray();

            Task.WaitAll(tasks);

            var results = tasks.Select(n => n.Result);

            return JArray.FromObject(results);
        }

        public JToken ConvertItem(Item source)
        {
            var propertyNames = ItemProfile.ItemPropertyNames.ToList();

            if (ItemProfile.ShowFields && ItemProfile.FieldPropertyNames.Any())
                propertyNames.Add("Fields");
            else //make sure it doesn't get accidentally added
            {
                propertyNames.Remove(
                    propertyNames.SingleOrDefault(n => n.Equals("Fields", StringComparison.OrdinalIgnoreCase)));
            }

            var item = GenerateJTokenAsync(source,
                                       propertyNames,
                                       ValueFormatters.Where(n => !(n.Source is IFieldValueFormatter)),
                                       ConvertItemProperty,
                                       FormatValues);

            return item.Result;
        }

        private JToken ConvertItemProperty(object source, string name)
        {
            if (!(source is Item)) return null;

            var key = name.ToLower();

            if (!PropertyConverters.ContainsKey(key)) return null;

            var converter = PropertyConverters[key];
            var result = converter((Item)source);
            var jTokenResult = result as JToken;

            return jTokenResult ?? JToken.FromObject(result);
        }

        public JToken ConvertItemFields(Item item)
        {
            Func<string, bool> nameIsHidden = 
                s => ItemProfile.HiddenFieldNames.Any(n => s.IndexOf(n, StringComparison.OrdinalIgnoreCase) != -1);

            var query = from obj in item.Fields
                        where !nameIsHidden(obj.Name)
                        select obj;

            var tasks = query.Select(obj => Task.FromResult(GenerateJTokenAsync(obj, ItemProfile.FieldPropertyNames, ValueFormatters, null, FormatFieldValues))).ToArray();

            Task.WaitAll(tasks);

            return JArray.FromObject(tasks.Select(n => n.Result.Result).Where(n => n.HasValues));
        }

        private async Task<JToken> GenerateJTokenAsync(object source, IEnumerable<string> propertyNames, 
                                        IEnumerable<Formatter> valueFormatters,
                                        Func<object, string, dynamic> convertValueFunc,
                                        Func<object, string, string, IEnumerable<Formatter>, dynamic> formatValueFunc)
        {
            var tasks = propertyNames.Select(n => 
                Task.FromResult(
                    PropertyConverter.Convert(source, n, ItemProfile.CamelCaseName, valueFormatters, convertValueFunc, formatValueFunc)));

            var results = await Task.WhenAll(tasks);

            var ret = new JObject();

            foreach (var result in results)
            {
                var value = result.Value ?? string.Empty;

                ret.Add(result.Key, (value as JToken) ?? JToken.FromObject(value));
            }

            return ret;
        }

        private static dynamic FormatValues(object source, string name, dynamic value, IEnumerable<Formatter> formatterArgs)
        {
            var parameters = new object[] {source, name, value};
            var task = InvokeFormattersAsync(formatterArgs, parameters);

            return task.Result;
        }

        private static dynamic FormatFieldValues(object source, string name, dynamic value,
                                                 IEnumerable<Formatter> formatterArgs)
        {
            var propertyIsValue = name.Equals("value", StringComparison.OrdinalIgnoreCase);

            var query = propertyIsValue
                            ? formatterArgs.Where(n => n.Source is IFieldValueFormatter)
                            : formatterArgs.Where(n => !(n.Source is IFieldValueFormatter));

            if (!query.Any()) return value;

            if (propertyIsValue)
            {
                var task = InvokeFormattersAsync(query, new[] {source});

                return task.Result;
            }

            return FormatValues(source, name, value, query);
        }

        private static async Task<object> InvokeFormattersAsync(IEnumerable<Formatter> formatters, object[] parameters)
        {
            Func<Formatter, object> formatValue = formatter =>
                                                      {
                                                          var source = formatter.Source as ValueFormatter;
                                                          formatter.Method.Invoke(source, parameters);
                                                          
                                                          return source.IsFormatted ? source.FormattedValue : null;
                                                      };

            var tasks = formatters.Select(n => Task.FromResult(formatValue(n))).ToList();

            var results = await Task.WhenAll(tasks);

            return results.SingleOrDefault(n => n != null);
        }

        private static IEnumerable<Formatter> GetFormatters(IEnumerable<Type> valueFormaters)
        {
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

                return new Formatter
                {
                    Source = obj,
                    Method = method
                };
            }).Where(n => n != null);

            return query;
        }
    }
}