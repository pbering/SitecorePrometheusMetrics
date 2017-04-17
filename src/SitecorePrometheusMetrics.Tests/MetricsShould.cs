using System;
using System.Threading.Tasks;
using FluentAssertions;
using SitecorePrometheusMetrics.Core;
using Xunit;

namespace SitecorePrometheusMetrics.Tests
{
    public class MetricsShould
    {
        [Fact]
        public void count_times_published_to_target_concurrent()
        {
            //// Arrange
            var increments = 1000;
            Metrics.Instance.Reset();

            //// Act
            Parallel.For(0, increments, i => Metrics.Instance.Increment("web"));

            //// Assert
            Metrics.Instance.GetCounters()["web"].Should().Be(increments);
        }

        [Fact]
        public void count_times_published_to_target_sequential()
        {
            //// Arrange
            var increments = 1000;
            Metrics.Instance.Reset();

            //// Act
            for (var i = 0; i < increments; i++)
            {
                Metrics.Instance.Increment("web");
            }

            //// Assert
            Metrics.Instance.GetCounters()["web"].Should().Be(increments);
        }

        [Fact]
        public void set_gauge_sequential()
        {
            //// Arrange
            var ticks = DateTime.Now.Ticks;
            Metrics.Instance.Reset();

            //// Act
            Metrics.Instance.Set("time", ticks);

            //// Assert
            Metrics.Instance.GetGauges()["time"].Should().Be(ticks);
        }
    }
}