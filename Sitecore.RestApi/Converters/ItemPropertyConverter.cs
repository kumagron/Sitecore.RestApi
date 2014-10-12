using System;
using System.Collections.Generic;
using System.Reflection;
using Sitecore.Data.Items;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Converters
{
    internal class ItemPropertyConverter
    {
        public static string ConvertId(Item item)
        {
            return item.ID.Guid.ToString("D");
        }
    }
}