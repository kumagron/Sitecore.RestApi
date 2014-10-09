using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Helpers
{
    public class SettingsHelper
    {
        public static string GetQueriesRootId()
        {
            return Configuration.Settings.GetSetting("RestfulSearch.ItemQueriesRootId");
        }

        public static string GetItemProfilesRootId()
        {
            return Configuration.Settings.GetSetting("Restful.ItemProfilesRootId");
        }
    }
}