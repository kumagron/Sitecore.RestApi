using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.RestApi.Models
{
    public class PropertyArgs
    {
        public object Source { get; set; }
        public string Name { get; set; }
        public PropertyInfo Info { get; set; }

        public object GetValue()
        {
            return Info.GetValue(Source);
        }
    }
}