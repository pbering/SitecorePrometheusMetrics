using System.Text;
using Sitecore.Diagnostics.PerformanceCounters;

namespace SitecorePrometheusMetrics.Core
{
    internal class SitecorePerformanceCounterMetric
    {
        private static readonly string _newLine = "\n";
        private readonly AmountPerSecondCounter _counter;
        private string _name;

        public SitecorePerformanceCounterMetric(AmountPerSecondCounter counter)
        {
            _counter = counter;
        }

        private string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _counter.Name
                                    .Replace(" / sec", "")
                                    .Replace(" | ", "_")
                                    .Replace(" ", "_");

                    _name = (_counter.Category.Replace(".", "_") + "_" + _name).ToLowerInvariant();
                }

                return _name;
            }
        }

        public override string ToString()
        {
            var content = new StringBuilder();

            content.AppendFormat("#TYPE {0} counter", Name);
            content.AppendFormat("{0}", _newLine);
            content.AppendFormat("{0}{{name=\"{1}\",category=\"{2}\"}}", Name, _counter.Name.ToLowerInvariant(), _counter.Category.ToLowerInvariant());
            content.AppendFormat(" {0}", _counter.Value);
            content.AppendFormat("{0}", _newLine);

            return content.ToString();
        }
    }
}