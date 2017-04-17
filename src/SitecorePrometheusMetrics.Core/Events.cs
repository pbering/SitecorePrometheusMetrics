using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Events;
using Sitecore.Publishing;

namespace SitecorePrometheusMetrics.Core
{
    public class Events
    {
        public void PublishComplete(object sender, EventArgs args)
        {
            var now = DateTime.UtcNow;
            var eventArgs = (SitecoreEventArgs)args;
            var options = (IEnumerable<DistributedPublishOptions>)eventArgs.Parameters[0];
            var publishOptions = options.First();
            var start = publishOptions.PublishDate;
            var name = "sitecore_publish_duration_milliseconds";

            // TODO: Verify that timing is correct

            Metrics.Instance.Set(name, Convert.ToInt64(now.Subtract(start).TotalMilliseconds));
        }
    }
}