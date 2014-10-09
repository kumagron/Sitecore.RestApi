using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Formatters
{
    public abstract class ValueFormatter
    {
        public object FormattedValue { get; private set; }

        public bool IsFormatted { get; private set; }

        public void SetValue(object value)
        {
            if (value == null || (FormattedValue != null && FormattedValue.Equals(value))) return;

            FormattedValue = value;
            IsFormatted = true;
        }
    }
}