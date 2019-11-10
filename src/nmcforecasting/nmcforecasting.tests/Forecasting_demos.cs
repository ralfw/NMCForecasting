using System;
using Xunit;
using Xunit.Abstractions;

namespace nmcforecasting.tests
{
    public class Forecasting_demos
    {
        private readonly Issue[] _historicalData = {
            new Issue(new DateTime(2019,10,1), new DateTime(2019,10,3), null, "a", false), 
            new Issue(new DateTime(2019,9,30), new DateTime(2019,10,7), null, "b", false),
            new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "c", false),
            new Issue(new DateTime(2019,9,30), new DateTime(2019,10,1), null, "", true),
            new Issue(new DateTime(2019,10,2), new DateTime(2019,10,4), null, "d", false),
            new Issue(new DateTime(2019,10,1), new DateTime(2019,10,2), null, "b", false),
            new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "d", false),
            new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "c", false),
            new Issue(new DateTime(2019,10,2), new DateTime(2019,10,7), null, "d", false),
            new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "d", false),
            new Issue(new DateTime(2019,9,30), new DateTime(2019,10,2), null, "c", false)
        };
        
        private readonly ITestOutputHelper _testOutputHelper;

        public Forecasting_demos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        
        [Fact]
        public void Issue_delivery_based_on_resources() {
            var sut = new Forecasting();
            
            var result = sut.WhenWillTheIssuesBeDone(2, 5, _historicalData);
            
            foreach(var x in result.Entries)
                _testOutputHelper.WriteLine($"{x.DaysUntilDelivery}\t{x.Probability}\t{x.Percentile}");
        }
        
        
        [Fact]
        public void Issue_delivery_based_on_throughput() {
            var sut = new Forecasting();
            
            var result = sut.WhenWillTheIssuesBeDone(new DateTime(2019,11,18), 10, _historicalData);
            
            foreach(var x in result.Entries)
                _testOutputHelper.WriteLine($"{x.DaysUntilDelivery}\t{x.Probability}\t{x.Percentile}");
        }
        
        
        [Fact]
        public void Story_delivery() {
            var sut = new Forecasting();
            
            var result = sut.WhenWillTheStoriesBeDone(new DateTime(2019,11,18), 10, _historicalData);
            
            foreach(var x in result.Entries)
                _testOutputHelper.WriteLine($"{x.DaysUntilDelivery}\t{x.Probability}\t{x.Percentile}");
        }
    }
}