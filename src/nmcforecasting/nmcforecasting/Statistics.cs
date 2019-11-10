using System.Collections.Generic;
using System.Linq;

namespace nmcforecasting
{
    public static class Statistics
    {
        public static (int value, int frequency, double probability, double percentile)[] Distribution(IEnumerable<int> events)
            => Distribution(Histogram(events));
        
        public static (int value, int frequency, double probability, double percentile)[] Distribution((int value, int frequency)[] histogram) {
            var n = (double)histogram.Sum(x => x.frequency);
            var percentile = 0.0;
            return histogram.OrderBy(x => x.value)
                            .Select(x => {
                                var p = x.frequency / n;
                                percentile += p;
                                return (x.value, x.frequency, p, percentile * 100.0);
                            })
                            .ToArray();
        }
        
        
        public static (int value, int f)[] Histogram(IEnumerable<int> events)
            => events.GroupBy(x => x)
                     .OrderBy(x => x.Key)
                     .Select(x => (x.Key, x.Count()))
                     .ToArray();
    }
}