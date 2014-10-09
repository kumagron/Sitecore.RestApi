using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.RestApi.Helpers;
using Sitecore.Data.Fields;

namespace Sitecore.RestApi.Models
{
    public class ItemQueryRepository: GenericRepository
    {
        public ItemQuery Get(string queryName)
        {
            var rootId = SettingsHelper.GetQueriesRootId();
            var itemQueriesRootId = Db.GetItem(rootId);

            if (itemQueriesRootId == null) return null;

            var item =
                itemQueriesRootId.Children.SingleOrDefault(
                    n => n["Name"].Equals(queryName, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                throw new Exception(
                    string.Format(
                        "The item query '{0}' with id {1} could not be found. Please check the setting value for RestfulSearch.ItemQueriesRootId.",
                        queryName, rootId));
            }

            var defaultParamsField = new NameValueListField(item.Fields["Default Params"]);
            
            var itemQuery = new ItemQuery
                                {
                                    Name = item["Name"],
                                    Query = item["Query"],
                                    DefaultParams = defaultParamsField.NameValues
                                };

            return itemQuery;
        }
    }
}