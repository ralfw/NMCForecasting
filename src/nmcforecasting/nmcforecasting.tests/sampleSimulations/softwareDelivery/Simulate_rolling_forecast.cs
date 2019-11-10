using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Bson;
using Xunit;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    public class Simulate_rolling_forecast
    {
        [Fact]
        public void Run()
        {
            const int INITIAL_NUMBER_OF_ISSUES = 5;
            var sut = new Forecasting();
            var issues = IssueRepository.Import().ToList();

            var cts = issues.Select(x => x.CycleTime.Days).ToArray();
            var ctsDistribution = Statistics.Distribution(cts);
            Export("CTs 2019-11-06.csv", ctsDistribution);
            var tps = issues.BusinessDayThroughputs();
            var tpsDistribution = Statistics.Distribution(tps);
            Export("TPs 2019-11-06.csv", tpsDistribution);
            

            // initial forecast
            var numberOfIssues = INITIAL_NUMBER_OF_ISSUES;
            var fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 6), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 6), fc);

            // updating forecast after first issue got delivered
            issues.Add(new Issue(new DateTime(2019,11,6), new DateTime(2019,11,7),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 7), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 7), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,7), new DateTime(2019,11,8),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 8), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 8), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,6), new DateTime(2019,11,11),null,null,false));
            numberOfIssues += -1 + 2;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 11), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 11), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,11), new DateTime(2019,11,13),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 13), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 13), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,8), new DateTime(2019,11,14),null,null,false));
            numberOfIssues += -1 + 1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 14), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 14), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,11), new DateTime(2019,11,15),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 15), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 15), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,13), new DateTime(2019,11,18),null,null,false));
            numberOfIssues += -1 + 2;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 18), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 18), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,15), new DateTime(2019,11,19),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 19), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 19), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,18), new DateTime(2019,11,20),null,null,false));
            numberOfIssues += -1;
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 20), numberOfIssues, issues.ToArray());
            Export(new DateTime(2019, 11, 20), fc); 
            
            issues.Add(new Issue(new DateTime(2019,11,18), new DateTime(2019,11,21),null,null,false));
            numberOfIssues += -1;
            Assert.Equal(0, numberOfIssues);
            
            // rear mirror wisdom
            fc = sut.WhenWillTheIssuesBeDone(new DateTime(2019, 11, 6), INITIAL_NUMBER_OF_ISSUES+5, issues.ToArray());
            File.Move("2019-11-06.csv", "2019-11-06-v1.csv", true);
            Export(new DateTime(2019, 11, 6), fc); 
            File.Move("2019-11-06.csv", "2019-11-06-v2.csv", true);
            
            cts = issues.Select(x => x.CycleTime.Days).ToArray();
            ctsDistribution = Statistics.Distribution(cts);
            Export("CTs 2019-11-21.csv", ctsDistribution);
            tps = issues.BusinessDayThroughputs();
            tpsDistribution = Statistics.Distribution(tps);
            Export("TPs 2019-11-21.csv", tpsDistribution);
        }

        void Export(DateTime startDate, Forecast forecast) {
            var deDE = new CultureInfo("de-DE");
            File.WriteAllLines($"{startDate:yyyy-MM-dd}.csv", Serialize());

            IEnumerable<string> Serialize()
                => forecast.Entries.Select(e => $"{startDate.AddDays(e.DaysUntilDelivery).ToString("d", deDE)}\t{e.Probability.ToString(deDE)}\t{e.Percentile.ToString(deDE)}");
        }


        void Export(string filename, (int value, int frequency, double probability, double precentile)[] distribution) {
            var deDE = new CultureInfo("de-DE");
            File.WriteAllLines(filename, Serialize());

            IEnumerable<string> Serialize()
                => distribution.Select(e => $"{e.value}\t{e.frequency}\t{e.probability.ToString(deDE)}\t{e.precentile.ToString(deDE)}");

        }
    }
}