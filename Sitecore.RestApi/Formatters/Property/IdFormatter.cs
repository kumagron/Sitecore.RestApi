using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Formatters.Property
{
    public class IdFormatter: Formatter, IPropertyFormatter
    {
        public void FormatProperty(PropertyArgs propertyArgs)
        {
            var id = propertyArgs.GetValue() as ID;

            if (id.IsNull) return;
 
            Set(id.Guid.ToString("D"));
        }
    }
}