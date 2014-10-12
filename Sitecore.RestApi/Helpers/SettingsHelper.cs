using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.RestApi.Helpers
{
    public class SettingsHelper
    {
        public static string GetSiteConfigName()
        {
            var siteName = Configuration.Settings.GetSetting("Sitecore.RestApi.SiteConfigName");

            return !string.IsNullOrEmpty(siteName) ? siteName : "website";
        }

        public static string GetQueriesRootId()
        {
            return Configuration.Settings.GetSetting("Sitecore.RestApi.ItemQueriesRootId");
        }

        public static string GetItemProfilesRootId()
        {
            return Configuration.Settings.GetSetting("Sitecore.RestApi.ItemProfilesRootId");
        }
    }
}