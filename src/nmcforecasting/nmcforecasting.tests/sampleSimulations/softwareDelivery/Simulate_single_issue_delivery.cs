using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    public class Simulate_single_issue_delivery
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_single_issue_delivery(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        
        [Fact]
        public void Forecast_based_on_all_issues() {
            var issues = IssueRepository.Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Select(x => x.CycleTime.Days).ToArray();
            
            var distribution = Statistics.Distribution(issueCycleTimes);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        [Fact]
        public void Forecast_feature_delivery() {
            var issues = IssueRepository.Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Where(x => x.Tags.Contains("feature")).Select(x => x.CycleTime.Days).ToArray();
            
            var distribution = Statistics.Distribution(issueCycleTimes);
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine($"Feature forecast");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        [Fact]
        public void Forecast_bug_fix_delivery()
        {
            var issues = IssueRepository.Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Where(x => x.IsBugfix).Select(x => x.CycleTime.Days).ToArray();
            
            var distribution = Statistics.Distribution(issueCycleTimes);
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine($"Feature forecast");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
    }
}