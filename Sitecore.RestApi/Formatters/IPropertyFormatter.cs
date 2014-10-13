using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Formatters
{
    public interface IPropertyFormatter
    {
        void FormatProperty(PropertyArgs propertyArgs);
    }
}
