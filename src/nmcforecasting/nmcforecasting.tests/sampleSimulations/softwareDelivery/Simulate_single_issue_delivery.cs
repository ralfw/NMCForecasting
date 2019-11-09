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
    public class Simulate_single_issue_delivery
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Simulate_single_issue_delivery(ITestOutputHelper testOutputHelper) {
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
        public void Forecast_based_on_all_issues()
        {
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Select(x => x.CycleTime.Days).ToArray();
            
            /*
            var rnd = new Random();
            int SimulateDelivery(int[] cycleTimes) => cycleTimes[rnd.Next(cycleTimes.Length)];
            int[] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => SimulateDelivery(issueCycleTimes)).ToArray();
            */

            var histogram = Histogram(issueCycleTimes);
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            foreach (var x in distribution.OrderBy(o => o.ct))
                _testOutputHelper.WriteLine($"{x.ct}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        [Fact]
        public void Forecast_feature_delivery()
        {
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Where(x => x.Tags.Contains("feature")).Select(x => x.CycleTime.Days).ToArray();
            
            var histogram = Histogram(issueCycleTimes);
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine($"Feature forecast");
            foreach (var x in distribution.OrderBy(o => o.ct))
                _testOutputHelper.WriteLine($"{x.ct}\t{x.f}\t{x.p.ToString("0.000", deDE)}\t{x.percentile.ToString("0.0", deDE)}");
        }
        
        [Fact]
        public void Forecast_bug_fix_delivery()
        {
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var issueCycleTimes = issues.Where(x => x.Tags.Contains("bug")).Select(x => x.CycleTime.Days).ToArray();
            
            var histogram = Histogram(issueCycleTimes);
            var distribution = Distribution(histogram);
            
            var deDE = new CultureInfo("de-DE");
            _testOutputHelper.WriteLine($"Feature forecast");
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
    }
}