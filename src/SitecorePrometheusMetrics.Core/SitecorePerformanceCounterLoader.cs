using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.Diagnostics.PerformanceCounters;

namespace SitecorePrometheusMetrics.Core
{
    public class SitecorePerformanceCounterLoader
    {
        public SitecorePerformanceCounterLoader()
        {
            var counterTypes = new List<string>
            {
                "Sitecore.Diagnostics.PerformanceCounters.CachingCount,Sitecore.Kernel",
                "Sitecore.Diagnostics.PerformanceCounters.DataCount,Sitecore.Kernel",
                "Sitecore.Diagnostics.PerformanceCounters.JobsCount,Sitecore.Kernel",
                "Sitecore.Diagnostics.PerformanceCounters.PresentationCount,Sitecore.Kernel",
                "Sitecore.Diagnostics.PerformanceCounters.SecurityCount,Sitecore.Kernel",
                "Sitecore.Diagnostics.PerformanceCounters.SystemCount,Sitecore.Kernel"
            };

            var counters = new List<BaseCounter>();

            foreach (var counterRef in counterTypes)
            {
                var type = Type.GetType(counterRef);

                if (type == null)
                {
                    continue;
                }

                var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

                counters.AddRange(properties.Select(property => property.GetValue(null)).OfType<BaseCounter>());
            }

            Counters = counters.Distinct();
        }

        public IEnumerable<BaseCounter> Counters { get; }
    }
}