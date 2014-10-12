using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace Sitecore.RestApi.Formatters
{
    public interface IPropertyFormatter
    {
        void Format(Item source, object value);
    }
}
