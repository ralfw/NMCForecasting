using System;
using System.Linq;

namespace nmcforecasting
{
    public class Forecasting
    {
        private readonly MonteCarloSimulation _mc;
        
        public Forecasting() => _mc = new MonteCarloSimulation();
        internal Forecasting(MonteCarloSimulation mc) => _mc = mc;
        
        
        public Forecast WhenWillTheIssueBeDone(Issue[] deliveryHistory) 
            => WhenWillTheIssuesBeDone(1, new[]{deliveryHistory});
        public Forecast WhenWillTheIssuesBeDone(int numberOfIssues, Issue[] deliveryHistory)
            => WhenWillTheIssuesBeDone(1, numberOfIssues, deliveryHistory);
        public Forecast WhenWillTheIssuesBeDone(int numberOfResources, int numberOfIssues, Issue[] deliveryHistory)
            => WhenWillTheIssuesBeDone(numberOfResources, Enumerable.Range(1, numberOfIssues).Select(_ => deliveryHistory).ToArray());
        
        public Forecast WhenWillTheIssuesBeDone(int numberOfResourses, params Issue[][] deliveryHistory) {
            var simulation = _mc.SimulateIssueDeliveryByResources(numberOfResourses, CycleTimes());
            var distribution = Statistics.Distribution(simulation);
            return new Forecast(distribution);
            
            int[][] CycleTimes() => deliveryHistory.Select(x => x.Select(y => y.CycleTime.Days).ToArray()).ToArray();
        }


        public Forecast WhenWillTheIssuesBeDone(DateTime startDate, int numberOfIssues, params Issue[][] deliveryHistory) {
            var simulation = _mc.SimulateIssueDeliveryBasedOnThroughput(startDate, numberOfIssues, Throughputs());
            var distribution = Statistics.Distribution(simulation);
            return new Forecast(distribution);
            
            int[][] Throughputs() => deliveryHistory.Select(x => x.BusinessDayThroughputs()).ToArray();
        }
        
        
        public Forecast WhenWillTheStoriesBeDone(DateTime startDate, int numberOfStories, params Issue[] deliveryHistory) {
            var simulation = _mc.SimulateStoryDeliveryBasedOnThroughput(startDate, numberOfStories, deliveryHistory);
            var distribution = Statistics.Distribution(simulation);
            return new Forecast(distribution);
        }
    }
}