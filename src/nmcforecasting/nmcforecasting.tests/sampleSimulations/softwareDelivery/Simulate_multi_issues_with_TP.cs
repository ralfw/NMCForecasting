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
        public void Forecast_8_issues()
        {
            DateTime START_DATE = new DateTime(2019,11,18);
            const int NUMBER_OF_ISSUES = 8;
            
            var issues = Import("sampleSimulations/softwareDelivery/sampleData/issue_log.csv").ToArray();
            var tp = Compile_throughput(issues);
            _testOutputHelper.WriteLine($"TP sum: {tp.Sum()}");
            
            var rnd = new Random();
            int[] MCSimulation(int n) => Enumerable.Range(1, n).Select(_ => SimulateDelivery(START_DATE, NUMBER_OF_ISSUES, tp, rnd)).ToArray();

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

        

        [Fact]
        public void Compile_throughput_test()
        {
            var result = Compile_throughput(new[] {
                new Issue{StartDate = new DateTime(2019,10,1), CompletionDate = new DateTime(2019,10,2)},
                new Issue{StartDate = new DateTime(2019,10,2), CompletionDate = new DateTime(2019,10,4)},
                new Issue{StartDate = new DateTime(2019,10,2), CompletionDate = new DateTime(2019,10,7)},
                new Issue{StartDate = new DateTime(2019,10,3), CompletionDate = new DateTime(2019,10,4)}
            });
            
            Assert.Equal(new[]{0,1,0,2,1}, result);
        }
    }
}