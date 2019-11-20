using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    public class Simulate_multi_issues_with_TP
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_multi_issues_with_TP(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }
        


        [Fact]
        public void Forecast_8_issues()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_ISSUES = 8;
            
            var issues = IssueRepository.Import().ToArray();
            var tp = issues.BusinessDayThroughputs();
            _testOutputHelper.WriteLine($"TP sum: {tp.Sum()}");

            var sut = new SoftwareDeliverySimulation();
            var simulationresult = sut.SimulateIssueDeliveryBasedOnThroughput(START_DATE, NUMBER_OF_ISSUES, tp);
            var distribution = Statistics.Distribution(simulationresult);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.frequency}\t{x.probability.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
    }
}