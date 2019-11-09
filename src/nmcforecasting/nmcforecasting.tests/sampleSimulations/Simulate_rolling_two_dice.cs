using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations
{
    public class Simulate_rolling_two_dice
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_rolling_two_dice(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Run()
        {
            var rnd = new Random();
            
            var historical_data = new[] {4, 5, 6, 2, 1, 5, 3, 3, 4, 1, 4, 1, 2, 2, 4, 6, 3, 5, 1};

            int PickSingleEvent() => historical_data[rnd.Next(historical_data.Length)];
            (int, int) SimulateCombinedEvent() => (PickSingleEvent(), PickSingleEvent());
            (int, int)[] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => SimulateCombinedEvent()).ToArray();

            var simulations = MCSimulation(1000);

            bool Q1Interpretation((int first, int second) e) => e.first == 2 && e.second == 4;
            int Q2Interpretation((int first, int second) e) => e.first + e.second;

            var simulatedHistoricalDataQ1 = simulations.Select(Q1Interpretation).ToArray();
            var simulatedHistoricalDataQ2 = simulations.Select(Q2Interpretation).ToArray();

            (S e, int f)[] Histogram<S, T>(IEnumerable<T> historicalData, Func<T, S> getKey)
                => historicalData.GroupBy(getKey)
                                 .OrderBy(x => x.Key)
                                 .Select(x => (x.Key, x.Count()))
                                 .ToArray();

            var histogramQ1 = Histogram(simulatedHistoricalDataQ1, x => x);
            var histogramQ2 = Histogram(simulatedHistoricalDataQ2, x => x);
            
            
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
            

            var distributionQ1 = Distribution(histogramQ1);
            var distributionQ2 = Distribution(histogramQ2);
            
            _testOutputHelper.WriteLine("Q1: first 2 then 4");
            foreach (var x in distributionQ1)
                _testOutputHelper.WriteLine(x.ToString());
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine("Q2: risk of 6 or lower");
            foreach (var x in distributionQ2.OrderBy(o => o.e))
                _testOutputHelper.WriteLine($"{x.e}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");

        }
    }
}