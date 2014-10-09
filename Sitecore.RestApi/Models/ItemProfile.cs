using System;
using System.Collections.Generic;

namespace Sitecore.RestApi.Models
{
    public class ItemProfile
    {
        public string Name { get; set; }
       
        public IEnumerable<string> ItemPropertyNames { get; set; }
        public IEnumerable<string> FieldPropertyNames { get; set; }
        public IEnumerable<string> HiddenFieldNames { get; set; }
        public bool ShowFields { get; set; }
        public bool CamelCaseName { get; set; }
        public IEnumerable<Type> ValueFormatters { get; set; }
    }
}