using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.RestApi.Formatters
{
    public interface IValueFormatter
    {
        void FormatValue(object source, PropertyInfo property);
        //void FormatValue(object source, string name, string value);
    }
}