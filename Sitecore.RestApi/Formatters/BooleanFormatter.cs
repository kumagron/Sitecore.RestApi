using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Formatters
{
    public class BooleanFormatter : ValueFormatter, IValueFormatter
    {
        //public object FormatValue(object source, string name, string value)
        //{
        //    bool result;

        //    if (!bool.TryParse(value, out result))
        //        return null;

        //    return result;
        //}

        public void FormatValue(object source, string name, string value)
        {
            bool result;

            if (!bool.TryParse(value, out result))
                SetValue(value);
        }
    }
}