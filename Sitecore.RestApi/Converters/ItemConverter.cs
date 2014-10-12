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

        public ItemConverter(ItemProfile itemProfile)
        {
            ItemProfile = itemProfile;
            ItemProfile.ItemPropertyFormatters = new Dictionary<string, Func<Item, object>>
                                         {
                                             {"fields", ConvertItemFields}
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
            var item = FormatHelper.GenerateJTokenAsync(source, ItemProfile, ConvertItemProperty, FormatHelper.FormatValues);

            return item.Result;
        }

        private JToken ConvertItemProperty(object source, string name)
        {
            if (!(source is Item)) return null;

            var key = name.ToLower();

            if (!ItemProfile.ItemPropertyFormatters.ContainsKey(key)) return null;

            var converter = ItemProfile.ItemPropertyFormatters[key];
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

            var tasks = query.Select(obj => Task.FromResult(FormatHelper.GenerateJTokenAsync(obj, ItemProfile, null, FormatHelper.FormatFieldValues))).ToArray();

            Task.WaitAll(tasks);

            return JArray.FromObject(tasks.Select(n => n.Result.Result).Where(n => n.HasValues));
        }
    }
}