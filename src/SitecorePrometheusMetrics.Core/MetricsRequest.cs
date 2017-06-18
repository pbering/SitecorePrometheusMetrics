using System;
using System.Linq;
using Sitecore.Caching;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Pipelines.HttpRequest;

namespace SitecorePrometheusMetrics.Core
{
    public class MetricsRequest : HttpRequestProcessor
    {
        private readonly SitecorePerformanceCounterLoader _sitecorePerformanceCounters;
        private readonly string _titleFormat;

        public MetricsRequest()
        {
            _sitecorePerformanceCounters = new SitecorePerformanceCounterLoader();
            _titleFormat = "# ---\n# {0}\n# ---\n";
        }

        public override void Process(HttpRequestArgs args)
        {
            var context = args.Context;

            if (!context.Request.Url.PathAndQuery.Equals("/metrics", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            args.AbortPipeline();

            context.Response.Clear();

            // The version is the prometheus protocol version
            context.Response.ContentType = "text/plain; version=0.0.4";

            var hostname = Environment.MachineName.ToLowerInvariant();

            context.Response.Write(string.Format(_titleFormat, "Meta metrics"));
            context.Response.Write("#TYPE sitecore_os_hostname counter\nsitecore_os_hostname{name=\"" + hostname + "\"} 1\n");

            context.Response.Write(string.Format(_titleFormat, "Custom metrics"));

            foreach (var counter in Metrics.Instance.GetGauges())
            {
                // TODO: Rendering should be as in "SitecoreCacheMetric"
                var line = "#TYPE " + counter.Key + " gauge\n" + counter.Key + " " + counter.Value + "\n";

                context.Response.Write(line);
            }

            foreach (var counter in Metrics.Instance.GetCounters())
            {
                // TODO: Rendering should be as in "SitecoreCacheMetric"
                var line = "#TYPE " + counter.Key + " counter\n" + counter.Key + " " + counter.Value + "\n";

                context.Response.Write(line);
            }

            context.Response.Write(string.Format(_titleFormat, "Sitecore cache statistics"));

            var statistics = CacheManager.GetStatistics();

            context.Response.Write(new SitecoreCacheMetric("instance", statistics.TotalCount, statistics.TotalSize));

            var metrics = CacheManager.GetAllCaches()
                                      .Where(c => c != null && c.Enabled)
                                      .Select(c => new SitecoreCacheMetric(c))
                                      .ToList();

            context.Response.Write(string.Format(_titleFormat, "Sitecore cache instances"));

            foreach (var metric in metrics)
            {
                context.Response.Write(metric);
            }

            context.Response.Write(string.Format(_titleFormat, "Sitecore performance counters"));

            foreach (var counter in _sitecorePerformanceCounters.Counters)
            {
                var perSecondCounter = counter as AmountPerSecondCounter;

                if (perSecondCounter != null)
                {
                    context.Response.Write(new SitecorePerformanceCounterMetric(perSecondCounter));
                }
                else
                {
                    Log.Info($"Skipping unsupported counter type: '{counter.GetType().FullName}'", this);
                }
            }

            context.ApplicationInstance.CompleteRequest();
        }
    }
}