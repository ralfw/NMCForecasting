using System;
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

        public static (int firstMode, double mean, double median) DistributionKPIs((int value, int frequency, double probability, double percentile)[] distribution) {
            var modeFrequency = distribution.Max(x => x.frequency);
            var firstMode = distribution.First(x => x.frequency == modeFrequency).value;

            var mean = distribution.Sum(x => x.value * x.probability);

            var n = distribution.Sum(x => x.frequency);
            var iMedian = (n+1) / 2;
            var nFound = 0;
            var median = 0.0;
            for (var i = 0; i < distribution.Length; i++) {
                nFound += distribution[i].frequency;
                if (nFound >= iMedian) {
                    median = distribution[i].value;
                    break;
                }
            }
            
            return (firstMode, mean, median);
        }
        
        
        public static (int value, int f)[] Histogram(IEnumerable<int> events)
            => events.GroupBy(x => x)
                     .OrderBy(x => x.Key)
                     .Select(x => (x.Key, x.Count()))
                     .ToArray();
    }
}