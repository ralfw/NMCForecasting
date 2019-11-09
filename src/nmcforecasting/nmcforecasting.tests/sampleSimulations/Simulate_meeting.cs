using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations
{
    public class Simulate_meeting
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_meeting(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Run()
        {
            var rnd = new Random();

            var donald_arrivals = new[] {1,	3,	-7,	2,	4,	-4,	2,	2,	-5,	2};
            var daisy_arrivals = new[] { -3, 1,	-1,	1,	2,	1,	2,	-2,	2,	-3, };
            var dagobert_arrivals = new[] { -2,	2,	-4,	4,	-2,	10,	-5,	3,	-2,	-4,};
            
            int PickSingleEvent(int[] histData) => histData[rnd.Next(histData.Length)];
            int[] SimulateCombinedEvent() => new[]{PickSingleEvent(donald_arrivals), PickSingleEvent(daisy_arrivals), PickSingleEvent(dagobert_arrivals)};
            int[][] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => SimulateCombinedEvent()).ToArray();

            var simulations = MCSimulation(10000);

            int Interpretation(int[] arrivals) {
                return arrivals.Max();
            }

            var simulatedHistoricalData = simulations.Select(Interpretation).ToArray();

            (S e, int f)[] Histogram<S, T>(IEnumerable<T> historicalData, Func<T, S> getKey)
                => historicalData.GroupBy(getKey)
                                 .OrderBy(x => x.Key)
                                 .Select(x => (x.Key, x.Count()))
                                 .ToArray();

            var histogram = Histogram(simulatedHistoricalData, x => x);
            
            
            (S e, int f, double p, double percentile)[] Distribution<S>((S e, int f)[] histogram) {
                var n = (double)histogram.Sum(x => x.f);
                var percentile = 0.0;
                return histogram.OrderBy(x => x.e)
                                .Select(x => {
                                                var p = x.f / n;
                                                percentile += p;
                                                return (x.e, x.f, p, percentile * 100.0);
                                             })
                                .ToArray();
            }
            
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine("Q2: risk of 6 or lower");
            foreach (var x in distribution.OrderBy(o => o.e))
                _testOutputHelper.WriteLine($"{x.e}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
    }
}