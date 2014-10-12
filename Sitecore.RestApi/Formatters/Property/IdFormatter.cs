using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.RestApi.Formatters.Property
{
    public class IdFormatter: Formatter, IPropertyFormatter
    {
        public void Format(Item source, object value)
        {
            var id = value as ID;

            if (id.IsNull) return;
 
            Set(id.Guid.ToString("D"));
        }
    }
}