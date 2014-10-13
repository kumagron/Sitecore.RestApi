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
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Formatters
{
    public class LinkFieldFormatter : Formatter, IFieldValueFormatter
    {
        public void Format(PropertyArgs propertyArgs)
        {
            var linkField = new LinkField(propertyArgs.Source as Field);

            if (string.IsNullOrEmpty(linkField.Url) && linkField.TargetID.IsNull) return;

            var linkUrl = linkField.TargetItem != null ? LinkManager.GetItemUrl(linkField.TargetItem) : linkField.Url;

            var value = new
                       {
                           text = linkField.Text,
                           url = linkUrl,
                           anchor = linkField.Anchor,
                           target = linkField.Target
                       };

            Set(value);
        }
    }
}