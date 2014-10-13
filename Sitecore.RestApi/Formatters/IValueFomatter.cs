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
        void Format(PropertyArgs propertyArgs);
        //void Format(object source, PropertyInfo property);
    }
}