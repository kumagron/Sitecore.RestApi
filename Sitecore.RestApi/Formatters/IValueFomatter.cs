using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Formatters
{
    public interface IValueFormatter
    {
        void FormatValue(object source, string name, string value);
    }
}