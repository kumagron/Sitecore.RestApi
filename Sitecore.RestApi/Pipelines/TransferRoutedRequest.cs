using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.RestApi.Pipelines
{
    public class TransferRoutedRequest : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(args.Context));
            
            if (routeData != null)
            {
                args.AbortPipeline();
            }

            //Assert.ArgumentNotNull(args, "args");
            //HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);
            //RouteData routeData = RouteTable.Routes.GetRouteData(args.Context);
            //if (routeData != null)
            //{
            //    RouteValueDictionary dictionary = (routeData.Route as Route).ValueOrDefault(r => r.Defaults);
            //    if ((dictionary == null) || !dictionary.ContainsKey("scIsFallThrough"))
            //    {
            //        args.AbortPipeline();
            //    }
            //}
        }
    }
}