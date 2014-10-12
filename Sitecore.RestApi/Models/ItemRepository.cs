using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.RestApi.Helpers;

namespace Sitecore.RestApi.Models
{
    public class GenericRepository
    {
        public Sitecore.Data.Database Db { get; set; }

        public GenericRepository()
        {
            var siteName = SettingsHelper.GetSiteConfigName();
            var site = Sitecore.Configuration.Factory.GetSite(siteName);

            if (site == null)
                throw new Exception(string.Format("Site configuration for '{0}' could not be found!", siteName));

            Db = Sitecore.Data.Database.GetDatabase(site.Database.Name);
        }
    }

    public class ItemRepository: GenericRepository
    {
        public Sitecore.Data.Items.Item Get(string id)
        {
            return Db.GetItem(id);
        }

        public Sitecore.Data.Items.Item Get(Sitecore.Data.ID id)
        {
            return Db.GetItem(id);
        }

        public IEnumerable<Sitecore.Data.Items.Item> Find(string query)
        {
            var items = Db.SelectItems(query);

            return items.Any() ? items : new Sitecore.Data.Items.Item[] {};
        }
    }
}