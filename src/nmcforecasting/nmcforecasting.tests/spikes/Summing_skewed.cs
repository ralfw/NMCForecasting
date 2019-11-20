using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using nmcforecasting.supporting;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.spikes
{
    public class Summing_skewed
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Summing_skewed(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }
        

        [Fact]
        public void Run()
        {
            var data = new[] {
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                2,
                2,
                2,
                2,
                2,
                3,
                3,
                3,
                4,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                15,
                15,
                15
            };

            var dist = Statistics.Distribution(data);
            var deDE = new CultureInfo("de-DE");
            foreach (var x in dist.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.frequency}\t{x.probability.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
            
            
            var mcs = new MonteCarloSimulation<int,int>(values => values.Sum());
            var results = mcs.Simulate(data, data, data, data, data);

            _testOutputHelper.WriteLine("---");
            dist = Statistics.Distribution(results);
            foreach (var x in dist.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.frequency}\t{x.probability.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
    }
}