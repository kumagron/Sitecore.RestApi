using System;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.RestApi.Models
{
    public class ItemProfile
    {
        public string Name { get; set; }
       
        public IList<string> ItemPropertyNames { get; set; }
        public IList<string> FieldPropertyNames { get; set; }
        public IList<string> HiddenFieldNames { get; set; }
        public bool ShowFields { get; set; }
        public bool CamelCaseName { get; set; }
        public IList<FormatterArgs> ValueFormatters { get; set; }
        public IDictionary<string, Func<Item, object>> ItemPropertyFormatters { get; set; }
    }
}