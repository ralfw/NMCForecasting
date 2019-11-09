using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    public class Simulate_multi_issues_with_TP_and_stories
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_multi_issues_with_TP_and_stories(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }
        
        

        [Fact]
        public void Forecast_10_stories_with_single_issue_forecast()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_STORIES = 10;
            
            var issues = IssueRepository.Import().ToArray();
            var issuesPerStory = issues.IssuesPerStory();
            
            var sut = new MonteCarloSimulation();
            var issueSimulationresult = sut.SimulateIssuesDerivedFromStories(
                Enumerable.Range(1,NUMBER_OF_STORIES).Select(_ => issuesPerStory).ToArray()
            );
            var issueDistribution = Statistics.Distribution(issueSimulationresult);
            
            _testOutputHelper.WriteLine("issues from stories forecast");
            var deDE = new CultureInfo("de-DE");
            foreach (var x in issueDistribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");

            
            
            _testOutputHelper.WriteLine("delivery forecast");

            var number_of_issues = issueDistribution.First(x => x.percentile > 80.0).value;
            _testOutputHelper.WriteLine($"Number of issues selected: {number_of_issues}");

            var tp = issues.BusinessDayThroughputs();
            var deliverySimulationresult = sut.SimulateIssueDeliveryBasedOnThroughput(START_DATE, number_of_issues, tp);
            var distribution = Statistics.Distribution(deliverySimulationresult);
            
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        [Fact]
        public void Forecast_10_stories_for_all_issue_forecasts()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_STORIES = 10;
            
            var issues = IssueRepository.Import().ToArray();
            
            var sut = new MonteCarloSimulation();
            var simulationresult = sut.SimulateStoryDeliveryBasedOnThroughput(START_DATE, NUMBER_OF_STORIES, issues);
            var distribution = Statistics.Distribution(simulationresult);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
    }
}