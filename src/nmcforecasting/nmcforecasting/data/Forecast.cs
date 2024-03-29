using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace nmcforecasting
{
    public class Forecast
    {
        private IEnumerable<ForecastEntry> _entries;
        
        public class ForecastEntry {
            public int DaysUntilDelivery { get; }
            public double Probability { get; }
            public double Percentile { get; }

            internal ForecastEntry(int daysUntilDelivery, double probability, double percentile) {
                DaysUntilDelivery = daysUntilDelivery;
                Probability = probability;
                Percentile = percentile;
            }
        }

        internal Forecast((int cycleTimeDays, int _, double probability, double percentile)[] distribution) {
            _entries = distribution.Select(x => new ForecastEntry(
                x.cycleTimeDays,
                x.probability,
                x.percentile
            ));
        }

        public ForecastEntry[] Entries => _entries.ToArray();
    }
}