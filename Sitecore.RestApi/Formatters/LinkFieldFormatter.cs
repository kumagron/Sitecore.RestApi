using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Links;

namespace Sitecore.RestApi.Formatters
{
    public class LinkFieldFormatter : ValueFormatter, IFieldValueFormatter
    {
        public void FormatValue(Field field)
        {
            var linkField = new LinkField(field);

            if (string.IsNullOrEmpty(linkField.Url) && linkField.TargetID.IsNull) return;

            var linkUrl = linkField.TargetItem != null ? LinkManager.GetItemUrl(linkField.TargetItem) : linkField.Url;

            var value = new
                       {
                           text = linkField.Text, 
                           url = linkUrl,
                           anchor = linkField.Anchor,
                           target = linkField.Target
                       };

            SetValue(value);
        }
    }
}