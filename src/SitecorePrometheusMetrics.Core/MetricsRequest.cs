using System;
using Sitecore.Pipelines.HttpRequest;

namespace SitecorePrometheusMetrics.Core
{
    public class MetricsRequest : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            var context = args.Context;

            if (!context.Request.Url.PathAndQuery.Equals("/metrics", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            args.AbortPipeline();

            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write("hello");

            context.ApplicationInstance.CompleteRequest();
        }
    }
}