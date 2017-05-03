using System.Text;
using Sitecore.Caching;

namespace SitecorePrometheusMetrics.Core
{
    internal class SitecoreCacheMetric
    {
        private static readonly string _prefix = "sitecore_cachestats_";
        private static readonly string _newLine = "\n";
        private readonly string _originalName;
        private string _name;

        public SitecoreCacheMetric(ICacheInfo cache) : this(cache.Name, cache.Count, cache.Size)
        {
        }

        public SitecoreCacheMetric(string name, long count, long size)
        {
            _originalName = name.ToLowerInvariant();

            Count = count;
            Size = size;
        }

        public long Count { get; }
        public long Size { get; }

        private string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _originalName.Replace(" - ", "_")
                                         .Replace(", ", "_")
                                         .Replace(".", "_")
                                         .Replace("[", "_")
                                         .Replace("]", "")
                                         .Replace("(", "_")
                                         .Replace(")", "")
                                         .Replace(" ", "_");

                    _name = _prefix + _name.Trim('_');
                }

                return _name;
            }
        }

        public override string ToString()
        {
            var content = new StringBuilder();
            var objectsName = Name + "_objects";

            content.AppendFormat("#TYPE {0} gauge", objectsName);
            content.AppendFormat("{0}", _newLine);
            content.AppendFormat("{0}{{name=\"{1}\"}}", objectsName, _originalName);
            content.AppendFormat(" {0}", Count);
            content.AppendFormat("{0}", _newLine);

            var bytesName = Name + "_bytes";

            content.AppendFormat("#TYPE {0} gauge", bytesName);
            content.AppendFormat("{0}", _newLine);
            content.AppendFormat("{0}{{name=\"{1}\"}}", bytesName, _originalName);
            content.AppendFormat(" {0}", Size);
            content.AppendFormat("{0}", _newLine);

            return content.ToString();
        }
    }
}