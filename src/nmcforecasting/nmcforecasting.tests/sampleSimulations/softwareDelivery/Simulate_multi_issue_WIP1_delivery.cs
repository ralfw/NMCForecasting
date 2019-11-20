using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    public class Simulate_multi_issue_WIP1_delivery
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_multi_issue_WIP1_delivery(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }
        

        [Fact]
        public void Forecast_8_issues()
        {
            var issues = IssueRepository.Import().ToArray();

            IEnumerable<Issue> Filter_issues(params string[] tags) => issues.Where(i => tags.All(t => i.Tags.Contains(t)));
            int[] Get_cycle_times_in_days(IEnumerable<Issue> issues) => issues.Select(x => x.CycleTime.Days).ToArray();

            var frontend_feature_cts = Get_cycle_times_in_days(Filter_issues("frontend", "feature"));
            var backend_bug_cts = Get_cycle_times_in_days(Filter_issues("backend", "bug"));
            var backend_feature_cts = Get_cycle_times_in_days(Filter_issues("backend", "feature"));
            _testOutputHelper.WriteLine($"Events found: {frontend_feature_cts.Length}, {backend_bug_cts.Length}, {backend_feature_cts.Length}");

            var sut = new SoftwareDeliverySimulation();
            var simulations = sut.SimulateIssueDeliveryByResources(
                frontend_feature_cts,
                frontend_feature_cts,
                frontend_feature_cts,
                backend_bug_cts,
                backend_feature_cts,
                backend_feature_cts,
                backend_feature_cts,
                backend_feature_cts
            );
            var distribution = Statistics.Distribution(simulations);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.frequency}\t{x.probability.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
    }
}