using System;
using System.Linq;
using Sitecore.Caching;
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
            context.Response.ContentType = "text/plain; version=0.0.4";

            var metrics = CacheManager.GetAllCaches()
                                      .Where(c => c != null && c.Enabled)
                                      .Select(c => new SitecoreCacheMetric(c))
                                      .ToList();

            var statistics = CacheManager.GetStatistics();

            metrics.Insert(0, new SitecoreCacheMetric("instance", statistics.TotalCount, statistics.TotalSize));

            foreach (var metric in metrics)
            {
                context.Response.Write(metric);
            }

            context.ApplicationInstance.CompleteRequest();
        }
    }
}