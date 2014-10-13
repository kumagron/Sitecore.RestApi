using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Formatters
{
    public interface IValueFormatter
    {
        void FormatValue(PropertyArgs propertyArgs);
        //void FormatValue(object source, PropertyInfo property);
    }
}