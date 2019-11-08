using System;
using Xunit;

namespace nmcforecasting.tests
{
    public class ThroughputExtensions_tests
    {
        [Fact]
        public void DailyThroughputs()
        {
            var historicalData = new[] {
                new Issue(new DateTime(2019,10,1), new DateTime(2019,10,3), null, "", false), 
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,7), null, "", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "", false),
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,1), null, "", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,4), null, "", false),
                new Issue(new DateTime(2019,10,1), new DateTime(2019,10,2), null, "", false),
                new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,7), null, "", false),
                new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "", false),
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,2), null, "", false),
            };

            var result = historicalData.DailyThroughputs();
            
            Assert.Equal(new[]{0,1,2,3,1,4}, result);
        }
    }
}