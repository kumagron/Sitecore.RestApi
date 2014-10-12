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
        public PropertyInfo Property { get; set; }
    }
}