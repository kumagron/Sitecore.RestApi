using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Sitecore.Pipelines;

namespace Sitecore.RestApi.Pipelines
{
    public class RegisterWebApiRoute
    {
        public void Process(PipelineArgs args)
        {
            var config = GlobalConfiguration.Configuration;
            
            config.Routes.MapHttpRoute(
                    name: "ItemChildrenRoute",
                    routeTemplate: "api/items/{id}/children",
                    defaults: new { controller = "ItemChildren" });

            config.Routes.MapHttpRoute(
                    name: "ItemAncestorsRoute",
                    routeTemplate: "api/items/{id}/ancestors",
                    defaults: new { controller = "ItemAncestors" });

            config.Routes.MapHttpRoute(
                    name: "ItemDescendantsRoute",
                    routeTemplate: "api/items/{id}/descendants",
                    defaults: new { controller = "ItemDescendants" });

            if (!config.Routes.ContainsKey("DefaultApi"))
            {
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new {id = RouteParameter.Optional});
            }
        }
    }
}