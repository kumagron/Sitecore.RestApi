using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.RestApi.Helpers;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Formatters.Property
{
    public class FieldsFormatter: Formatter, IPropertyFormatter
    {
        private ItemProfile ItemProfile { get; set; }

        public FieldsFormatter()
        {
            var context = HttpContext.Current.Request;
            var profileName = context.QueryString["profile"];
            ItemProfile = new ItemProfileRepository().Get(!string.IsNullOrEmpty(profileName) ? profileName : "default");
        }

        public void Format(PropertyArgs propertyArgs)
        {
            var item = propertyArgs.Source as Item;
            Func<string, bool> nameIsHidden =
                s => ItemProfile.HiddenFieldNames.Any(n => s.IndexOf(n, StringComparison.OrdinalIgnoreCase) != -1);

            var query = from obj in item.Fields
                        where !nameIsHidden(obj.Name)
                        select obj;

            var tasks = query.Select(obj => Task.FromResult(FormatHelper.GenerateJTokenAsync(obj, ItemProfile))).ToArray();

            Task.WaitAll(tasks);

            var result = tasks.Select(n => n.Result.Result).Where(n => n.HasValues);

            Set(result);
        }
    }
}