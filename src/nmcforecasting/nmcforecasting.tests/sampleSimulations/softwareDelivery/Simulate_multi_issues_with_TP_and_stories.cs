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
        
        
        class Issue {
            public DateTime StartDate;
            public DateTime CompletionDate;
            public string[] Tags;
            public string StoryID;
            
            public TimeSpan CycleTime => CompletionDate - StartDate;
        }


        IEnumerable<Issue> Import(string filename)
        {
            using var csvReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(filename) {Delimiters = new[] {";"}};
            csvReader.ReadFields(); // skip headers
            
            while (csvReader.EndOfData is false) {
                var row = csvReader.ReadFields();
                yield return new Issue {
                    StartDate = DateTime.Parse(row[0], new CultureInfo("de-DE")),
                    CompletionDate = DateTime.Parse(row[1], new CultureInfo("de-DE")),
                    Tags = row[2].Split(','),
                    StoryID = row[3]
                };
            }
        }


        int[] Compile_throughput(Issue[] issues) {
            var firstStartDate = issues.Min(x => x.StartDate);
            var lastCompletionDate = issues.Max(x => x.CompletionDate);
            var allDates = Enumerable.Range(0, (lastCompletionDate - firstStartDate).Days + 1)
                                     .Select(offset => firstStartDate.AddDays(offset))
                                     .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
            
            var dailyTP = new Dictionary<DateTime,int>();
            foreach (var date in allDates)
                dailyTP[date] = 0;
            
            foreach (var issue in issues)
                dailyTP[issue.CompletionDate] += 1;

            return dailyTP.Values.ToArray();
        }


        (int[] issueFrequencies, double bug_ratio) Compile_issue_story_ratios(Issue[] issues)
        {
            var features = issues.Where(x => !string.IsNullOrWhiteSpace(x.StoryID)).OrderBy(x => x.StoryID).ToArray();
            return (
                features.GroupBy(x => x.StoryID).Select(x => x.Count()).ToArray(), 
                (issues.Length - features.Length) / (double)features.Length
            );
        }


        int SimulateDelivery(DateTime startDate, int numberOfIssues, int[] throughput, Random rnd) {
            var date = startDate;
            while (numberOfIssues > 0) {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) {
                    var tp_on_date = throughput[rnd.Next(throughput.Length)];
                    numberOfIssues -= tp_on_date;
                }
                date = date.AddDays(1);
            }
            return (date - startDate).Days;
        }
        

        [Fact]
        public void Forecast_10_stories_with_single_issue_forecast()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_STORIES = 10;
            
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issue_story_ratios = Compile_issue_story_ratios(issues);
            
            // simulate number of issues
            var rnd = new Random();

            int SimulateNumberOfIssuesForStories(int number_of_stories) {
                var numberOfFeatureIssues = 
                    Enumerable.Range(1, number_of_stories)
                              .Select(_ => issue_story_ratios.issueFrequencies[rnd.Next(issue_story_ratios.issueFrequencies.Length)])
                              .Sum();
                var numberOfBugIssues = (int)Math.Round(numberOfFeatureIssues * issue_story_ratios.bug_ratio);
                return numberOfFeatureIssues + numberOfBugIssues;
            }
            
            int[] MCSimulation_features(int n) => Enumerable.Range(1, n)
                                                            .Select(_ => SimulateNumberOfIssuesForStories(NUMBER_OF_STORIES))
                                                            .ToArray();

            var issueSimulationResult = MCSimulation_features(10000);
            var issueHistogram = Histogram(issueSimulationResult);
            var issueDistribution = Distribution(issueHistogram);
            
            _testOutputHelper.WriteLine("issues from stories forecast");
            var deDE = new CultureInfo("de-DE");
            foreach (var x in issueDistribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");

            
            
            _testOutputHelper.WriteLine("delivery forecast");

            var number_of_issues = issueDistribution.First(x => x.percentile > 80.0).value;
            _testOutputHelper.WriteLine($"Number of issues selected: {number_of_issues}");
            
            var tp = Compile_throughput(issues);
            
            int[] MCSimulation_CT(int n) => Enumerable.Range(1, n).Select(_ => SimulateDelivery(START_DATE, number_of_issues, tp, rnd)).ToArray();

            var simulationResults = MCSimulation_CT(10000);
            var histogram = Histogram(simulationResults);
            var distribution = Distribution(histogram);
            
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        [Fact]
        public void Forecast_10_stories_for_all_issue_forecasts()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_STORIES = 10;
            
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issue_story_ratios = Compile_issue_story_ratios(issues);
            
            // simulate number of issues
            var rnd = new Random();

            int SimulateNumberOfIssuesForStories(int number_of_stories) {
                var numberOfFeatureIssues = 
                    Enumerable.Range(1, number_of_stories)
                              .Select(_ => issue_story_ratios.issueFrequencies[rnd.Next(issue_story_ratios.issueFrequencies.Length)])
                              .Sum();
                var numberOfBugIssues = (int)Math.Round(numberOfFeatureIssues * issue_story_ratios.bug_ratio);
                return numberOfFeatureIssues + numberOfBugIssues;
            }
            
            int[] MCSimulation_features(int n) => Enumerable.Range(1, n)
                                                            .Select(_ => SimulateNumberOfIssuesForStories(NUMBER_OF_STORIES))
                                                            .ToArray();

            var issueSimulationResult = MCSimulation_features(10000);
            var issueHistogram = Histogram(issueSimulationResult);
            
            var tp = Compile_throughput(issues);
            
            
            _testOutputHelper.WriteLine("Simulate delivery for...");
            var totalDeliveryHistogram = new Dictionary<int,int>();
            foreach(var issueForecast in issueHistogram) {
                _testOutputHelper.WriteLine($"  {issueForecast.value} issues");
                
                int[] MCSimulation_CT(int n) => Enumerable.Range(1, n).Select(_ => SimulateDelivery(START_DATE, issueForecast.value, tp, rnd)).ToArray();
                
                var deliverySimulation = MCSimulation_CT(10000);
                var deliveryHistogram = Histogram(deliverySimulation);

                foreach (var delivery in deliveryHistogram) {
                    if (totalDeliveryHistogram.ContainsKey(delivery.value) is false)
                        totalDeliveryHistogram.Add(delivery.value, 0);
                    totalDeliveryHistogram[delivery.value] += delivery.f;
                }
            }
            
            var distribution = Distribution(totalDeliveryHistogram.Select(x => (x.Key, x.Value)).ToArray());
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.value))
                _testOutputHelper.WriteLine($"{x.value}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        
        (int value, int f)[] Histogram(IEnumerable<int> historicalData)
            => historicalData.GroupBy(x => x)
                .OrderBy(x => x.Key)
                .Select(x => (x.Key, x.Count()))
                .ToArray();
            
        (int value, int f, double p, double percentile)[] Distribution((int ct, int f)[] histogram) {
            var n = (double)histogram.Sum(x => x.f);
            var percentile = 0.0;
            return histogram.OrderBy(x => x.ct)
                .Select(x => {
                    var p = x.f / n;
                    percentile += p;
                    return (x.ct, x.f, p, percentile * 100.0);
                })
                .ToArray();
        }


        [Fact]
        public void Compile_issue_story_ratios_test()
        {
            var issues = new[]
            {
                new Issue{StoryID = "1"},
                new Issue{StoryID = "2"},
                new Issue{StoryID = "1"},
                new Issue(),
                new Issue{StoryID = "2"},
                new Issue{StoryID = "3"},
                new Issue{StoryID = ""},
                new Issue{StoryID = "2"}
            }; // 8 issues, 6 features, 2 bugs

            var result = Compile_issue_story_ratios(issues);
            
            Assert.Equal(new[]{2,3,1}, result.issueFrequencies);
            Assert.Equal(0.33, result.bug_ratio, 2);
        }
    }
}