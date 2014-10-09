using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.RestApi.Pipelines
{
    public class AbortSitecoreForKnownRoutes : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            /* This assumes all registered routes are external to Sitecore. If you have other routes registered, you may want
               to add an additional check that the requested url is actually an /api/ route, to avoid accidentally 
               remapping these other routes and aborting the pipeline. (Thanks Jason N.)
             */
            var routeCollection = RouteTable.Routes;
            var routeData = routeCollection.GetRouteData(new HttpContextWrapper(args.Context));

            if (routeData == null) return;

            args.Context.RemapHandler(routeData.RouteHandler.GetHttpHandler(args.Context.Request.RequestContext));
            args.AbortPipeline();
        }
    }
}