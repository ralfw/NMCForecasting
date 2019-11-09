using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations
{
    public class Simulate_train_connection
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_train_connection(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }
        
        
        private int[] ice875_ad = new[] {0,2,4,24,9,15,14,3,18,29};
        private (int,int)[] ic2443_dd_ad = new[] { (0,0), (1,1), (3,6), (2,1), (6,12), (13,21), (4,8), (0,4), (3,7), (5,15)};
        private (int,int)[] rb40_dd_ad = new[] { (0,0), (1,4), (3,1), (1,7), (5,6), (2,3), (16,20), (1,1), (2,12), (1,18)};
        

        [Fact]
        public void Run_scenario_1() {
            var rnd = new Random();
            
            T PickSingleEvent<T>(T[] events) => events[rnd.Next(events.Length)];
            // for each leg of the journey random connections are chosen
            (int, (int,int), (int,int)) SimulateCombinedEvent() => (
                PickSingleEvent(ice875_ad),
                PickSingleEvent(ic2443_dd_ad),
                PickSingleEvent(rb40_dd_ad)
            );
            
            Run(SimulateCombinedEvent);
        }
        
        
        [Fact]
        public void Run_scenario_2() {
            var rnd = new Random();
            
            // a full connection is taken from the historical data
            (int, (int,int), (int,int)) SimulateCombinedEvent() {
                var i = rnd.Next(ice875_ad.Length);
                return (
                    ice875_ad[i],
                    ic2443_dd_ad[i],
                    rb40_dd_ad[i]
                );
            }
            
            Run(SimulateCombinedEvent);
        }


        void Run(Func<(int, (int,int), (int,int))> simulateTuple)
        {
            
            (int, (int,int), (int,int))[] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => simulateTuple()).ToArray();


            var simulatedTuples = MCSimulation(1000);
            var simulatedEvents = simulatedTuples.Select(CalculateTotalDelay).ToArray();
            
            (S e, int f)[] Histogram<S, T>(IEnumerable<T> historicalData, Func<T, S> getKey)
                => historicalData.GroupBy(getKey)
                    .OrderBy(x => x.Key)
                    .Select(x => (x.Key, x.Count()))
                    .ToArray();
            
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

            var histogram = Histogram(simulatedEvents, x => x);
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.e))
                _testOutputHelper.WriteLine($"{x.e}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        int CalculateTotalDelay((int ice875_ad, (int dd, int ad) ic2443, (int dd, int ad) rb40) e) {
            const int scheduled_tt_ice875_ic2443 = 16;
            const int scheduled_tt_ic2443_rb40 = 9;
            const int change_trains_time = 3;
            
            var totalDelay = 0;

            var transfer1 = scheduled_tt_ice875_ic2443 - change_trains_time - e.ice875_ad + e.ic2443.dd;
            if (transfer1 < 0) totalDelay += 60;

            var transfer2 = scheduled_tt_ic2443_rb40 - change_trains_time - e.ic2443.ad + e.rb40.dd;
            if (transfer2 < 0) totalDelay += 60;

            totalDelay += e.rb40.ad;

            return totalDelay;
        }
    }
}