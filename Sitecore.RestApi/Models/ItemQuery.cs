using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Models
{
    public class ItemQuery
    {
        public string Name { get; set; }
        public string Query { get; set; }
        public NameValueCollection DefaultParams { get; set; }
    }
}