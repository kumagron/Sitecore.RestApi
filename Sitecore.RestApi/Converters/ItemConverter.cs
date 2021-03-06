﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Sitecore.RestApi.Content;
using Sitecore.RestApi.Formatters;
using Sitecore.RestApi.Formatters.Property;
using Sitecore.RestApi.Helpers;
using Sitecore.RestApi.Models;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;

namespace Sitecore.RestApi.Converters
{
    public class ItemConverter
    {
        private ItemProfile ItemProfile { get; set; }

        public ItemConverter(ItemProfile itemProfile)
        {
            ItemProfile = itemProfile;

            var methodInfo = typeof (IPropertyFormatter).GetMethods().SingleOrDefault(n => n.Name.Equals("Format"));

            ItemProfile.PropertyFormatters = new Dictionary<string, FormatterArgs>
                                                 {
                                                     {
                                                         "id",
                                                         new FormatterArgs
                                                             {
                                                                 Type = typeof(IdFormatter),
                                                                 Method = methodInfo
                                                             }
                                                     },
                                                     {
                                                         "fields",
                                                         new FormatterArgs
                                                             {
                                                                 Type = typeof(FieldsFormatter),
                                                                 Method = methodInfo
                                                             }
                                                     },
                                                     {
                                                         "foo",
                                                         new FormatterArgs
                                                             {
                                                                 Type = typeof(FooFormatter),
                                                                 Method = methodInfo
                                                             }
                                                     }
                                                 };

            if (ItemProfile.ShowFields && ItemProfile.FieldPropertyNames.Any())
                ItemProfile.ItemPropertyNames.Add("Fields");
            else //make sure it doesn't get accidentally added
            {
                ItemProfile.ItemPropertyNames.Remove(
                    ItemProfile.ItemPropertyNames.SingleOrDefault(n => n.Equals("Fields", StringComparison.OrdinalIgnoreCase)));
            }
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
            var item = new FormatHelper(ItemProfile).FormatObjectAsync(source);
            var result = item.Result;

            // This will force the xml serializer to place the field items under a fields node.
            var contentType = HttpContext.Current.Request.ContentType.ToLower();
            if (XmlContent.XmlContentTypes.Contains(contentType))
            {
                var fieldsKey = ItemProfile.CamelCaseName ? "fields" : "Fields";

                result[fieldsKey] = 
                    ItemProfile.CamelCaseName 
                    ? (object) new { field = result[fieldsKey] }
                    : new { Field = result[fieldsKey] };
            }

            return JToken.FromObject(result);
        }
    }
}