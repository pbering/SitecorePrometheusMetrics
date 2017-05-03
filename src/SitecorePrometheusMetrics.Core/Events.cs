using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Events;
using Sitecore.Publishing;

namespace SitecorePrometheusMetrics.Core
{
    public class Events
    {
        private readonly string _countName;
        private readonly string _durationName;

        public Events()
        {
            _durationName = "sitecore_publish_duration_milliseconds";
            _countName = "sitecore_publish_total";

            Metrics.Instance.ZeroGauge(_durationName);
            Metrics.Instance.ZeroCounter(_countName);
        }

        public void PublishComplete(object sender, EventArgs args)
        {
            var now = DateTime.UtcNow;
            var eventArgs = (SitecoreEventArgs)args;
            var options = (IEnumerable<DistributedPublishOptions>)eventArgs.Parameters[0];
            var publishOptions = options.First();
            var start = publishOptions.PublishDate;

            Metrics.Instance.Set(_durationName, Convert.ToInt64(now.Subtract(start).TotalMilliseconds));
            Metrics.Instance.Increment(_countName);
        }
    }
}