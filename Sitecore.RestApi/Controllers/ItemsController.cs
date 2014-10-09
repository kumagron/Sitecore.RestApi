using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Items;
using Sitecore.RestApi.Helpers;
using Sitecore.RestApi.Models;

namespace Sitecore.RestApi.Controllers
{
    public class ItemRestApiController : ApiController
    {
        private ItemConverter _itemConverter;

        public ItemConverter ItemConverter
        {
            get { return _itemConverter ?? (_itemConverter = GetItemConverter()); }
        }

        public ItemRepository ItemRepository { get; internal set; }

        public ItemRestApiController()
        {
            ItemRepository = new ItemRepository();
        }

        public ItemConverter GetItemConverter()
        {
            var profile = HttpContext.Current.Request.QueryString["profile"];
            var itemProfile = new ItemProfileRepository().Get(!string.IsNullOrEmpty(profile) ? profile : "default");

            return new ItemConverter(itemProfile);
        }
    }

    public class ItemsController : ItemRestApiController
    {
        public JToken Get(string id)
        {
            Data.ID result;

            return
                Data.ID.TryParse(id, out result)
                    ? ItemConverter.ConvertItem(ItemRepository.Get(id: result))
                    : ItemConverter.ConvertItems(GetItemsFromQueryName(queryName: id));
        }

        public JToken GetAll(string query)
        {
            return ItemConverter.ConvertItems(ItemRepository.Find(query));
        }

        private IEnumerable<Item> GetItemsFromQueryName(string queryName)
        {
            var itemQueryRepository = new ItemQueryRepository();
            var itemQuery = itemQueryRepository.Get(queryName);
            var query = itemQuery.Query.ToLower();

            Func<string, NameValueCollection, bool, string> formatQuery =
                (q, nv, decode) =>
                {
                    if (!nv.AllKeys.Any()) return q;

                    foreach (var key in nv.AllKeys)
                    {
                        var value = nv.Get(key);

                        q = q.Replace(
                                "{" + key + "}",
                                decode ? HttpUtility.UrlDecode(value) : value);
                    }

                    return q;
                };

            //replace with query param values
            query = formatQuery(query, HttpContext.Current.Request.QueryString, true);

            //replace with default param values
            query = formatQuery(query, itemQuery.DefaultParams, false);

            return ItemRepository.Find(query);
        }
    }

    public class ItemChildrenController : ItemRestApiController
    {
        public JToken Get(string id)
        {
            var source = ItemRepository.Get(id);

            return ItemConverter.ConvertItems(source.Children);
        }
    }

    public class ItemAncestorsController : ItemRestApiController
    {
        public JToken Get(string id)
        {
            var source = ItemRepository.Get(id);

            return ItemConverter.ConvertItems(source.Axes.GetAncestors());
        }
    }

    public class ItemDescendantsController : ItemRestApiController
    {
        public JToken Get(string id)
        {
            var source = ItemRepository.Get(id);

            return ItemConverter.ConvertItems(source.Axes.GetDescendants());
        }
    }
}
