using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests
{
    public class Simulate_multi_issue_WIP3_delivery
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_multi_issue_WIP3_delivery(ITestOutputHelper testOutputHelper) {
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
        
        

        [Fact]
        public void Forecast_8_issues()
        {
            const int N_RESOURCES = 3;
            var issues = Import("sampleData/issue_log.csv").ToArray();

            IEnumerable<Issue> Filter_issues(params string[] tags) => issues.Where(i => tags.All(t => i.Tags.Contains(t)));
            int[] Get_cycle_times_in_days(IEnumerable<Issue> issues) => issues.Select(x => x.CycleTime.Days).ToArray();

            var frontend_feature_cts = Get_cycle_times_in_days(Filter_issues("frontend", "feature"));
            var backend_bug_cts = Get_cycle_times_in_days(Filter_issues("backend", "bug"));
            var backend_feature_cts = Get_cycle_times_in_days(Filter_issues("backend", "feature"));
            _testOutputHelper.WriteLine($"Events found: {frontend_feature_cts.Length}, {backend_bug_cts.Length}, {backend_feature_cts.Length}");
            
            var rnd = new Random();

            int SimulateDelivery() {
                var resources = Distribute_issues_among_resources(new[] {
                        frontend_feature_cts[rnd.Next(frontend_feature_cts.Length)],
                        frontend_feature_cts[rnd.Next(frontend_feature_cts.Length)],
                        frontend_feature_cts[rnd.Next(frontend_feature_cts.Length)],
                        backend_bug_cts[rnd.Next(backend_bug_cts.Length)],
                        backend_feature_cts[rnd.Next(backend_feature_cts.Length)],
                        backend_feature_cts[rnd.Next(backend_feature_cts.Length)],
                        backend_feature_cts[rnd.Next(backend_feature_cts.Length)],
                        backend_feature_cts[rnd.Next(backend_feature_cts.Length)]
                    }, 
                    N_RESOURCES);
                return resources.Max();
            }

            int[] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => SimulateDelivery()).ToArray();

            var simulationResults = MCSimulation(10000);
            var histogram = Histogram(simulationResults);
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.ct))
                _testOutputHelper.WriteLine($"{x.ct}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        
        
        (int ct, int f)[] Histogram(IEnumerable<int> historicalData)
            => historicalData.GroupBy(x => x)
                .OrderBy(x => x.Key)
                .Select(x => (x.Key, x.Count()))
                .ToArray();
            
        (int ct, int f, double p, double percentile)[] Distribution((int ct, int f)[] histogram) {
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


        int[] Distribute_issues_among_resources(int[] issueCycleTimes, int numberOfResources) {
            var issues = new Queue<int>(issueCycleTimes);
            
            var resources = new int[numberOfResources];
            while (issues.Any()) {
                var least_busyness = resources.Min();

                var i_resource_with_least_busyness = 0;
                while (resources[i_resource_with_least_busyness] != least_busyness)
                    i_resource_with_least_busyness++;

                resources[i_resource_with_least_busyness] += issues.Dequeue();
            }
            return resources;
        }

        
        [Fact]
        public void Distribute_issues_among_resources_test()
        {
            var result = Distribute_issues_among_resources(new[] {3, 2, 5, 1, 4, 6, 2, 3}, 3);
            Assert.Equal(10, result.Max());
        }
    }
}