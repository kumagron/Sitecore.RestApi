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
    public class FooFormatter : Formatter, IPropertyFormatter
    {
        public void Format(PropertyArgs propertyArgs)
        {
            Set(new { value = "Foo" });
        }
    }
}