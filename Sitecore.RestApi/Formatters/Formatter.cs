using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Formatters
{
    public abstract class Formatter
    {
        public object FormattedObject { get; private set; }

        public bool IsFormatted { get; private set; }

        public void Set(object value)
        {
            if (value == null || (FormattedObject != null && FormattedObject.Equals(value))) return;

            FormattedObject = value;
            IsFormatted = true;
        }
    }
}