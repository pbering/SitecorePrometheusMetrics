using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SitecorePrometheusMetrics.Core
{
    public sealed class Metrics
    {
        private static readonly Lazy<Metrics> _lazy = new Lazy<Metrics>(() => new Metrics());
        private readonly ConcurrentDictionary<string, long> _counters;
        private readonly ConcurrentDictionary<string, long> _gauges;

        private Metrics()
        {
            _counters = new ConcurrentDictionary<string, long>();
            _gauges = new ConcurrentDictionary<string, long>();
        }

        public static Metrics Instance => _lazy.Value;

        public void Reset()
        {
            _counters.Clear();
            _gauges.Clear();
        }

        public Dictionary<string, long> GetCounters()
        {
            return _counters.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public Dictionary<string, long> GetGauges()
        {
            return _gauges.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void Increment(string name)
        {
            _counters.AddOrUpdate(name, 1, (key, value) => value + 1);
        }

        public void ZeroCounter(string name)
        {
            _counters.TryAdd(name, 0);
        }

        public void ZeroGauge(string name)
        {
            _gauges.TryAdd(name, 0);
        }

        public void Set(string name, long value)
        {
            _gauges.AddOrUpdate(name, value, (key, current) => value);
        }
    }
}