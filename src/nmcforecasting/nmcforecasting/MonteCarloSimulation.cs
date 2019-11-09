using System;
using System.Collections.Generic;
using System.Linq;

namespace nmcforecasting
{
    public class MonteCarloSimulation
    {
        private const int DEFAULT_NUMBER_OF_SIMULATION_RUNS = 10000;
        
        private readonly int _numberOfSimulationRuns;
        private readonly Func<int,int> _nextRnd;
        
        public MonteCarloSimulation() : this(DEFAULT_NUMBER_OF_SIMULATION_RUNS){}
        public MonteCarloSimulation(int numberOfSimulationRuns) { 
            _numberOfSimulationRuns = numberOfSimulationRuns;
            
            var rnd = new Random();
            _nextRnd = maxValue => rnd.Next(maxValue);
        }
        internal MonteCarloSimulation(int numberOfSimulationRuns, Func<int,int> nextRnd) { 
            _numberOfSimulationRuns = numberOfSimulationRuns;
            _nextRnd = nextRnd;
        }
        
        

        public int[] SimulateDeliveryByResources(params int[][] cycleTimeLogs)
            => SimulateDeliveryByResources(1, cycleTimeLogs);
        public int[] SimulateDeliveryByResources(int numberOfResources, params int[][] cycleTimeLogs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                         .Select(_ => SimulateOneDeliveryByResources(numberOfResources, cycleTimeLogs))
                         .ToArray();
        
        private int SimulateOneDeliveryByResources(in int numberOfResources, in int[][] cycleTimeLogs)
            => SimulateBacklog(cycleTimeLogs).CalculateDeliveryTime(numberOfResources);

        private Backlog SimulateBacklog(IEnumerable<int[]> cycleTimeLogs)
            => new Backlog(cycleTimeLogs.Select(PickRandomly).ToArray());



        public int[] SimulateDeliveryBasedOnThroughput(DateTime startDate, int numberOfIssues, params int[][] throughputLogs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                         .Select(_ => SimulateOneDeliveryBasedOnThroughput(startDate, numberOfIssues, throughputLogs))
                         .ToArray();

        private int SimulateOneDeliveryBasedOnThroughput(DateTime startDate, int numberOfIssues, params int[][] throughputLogs) {
            var date = startDate.Subtract(new TimeSpan(1,0,0,0));
            while (numberOfIssues > 0) {
                date = date.AddDays(1);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) {
                    numberOfIssues -= throughputLogs.Sum(PickRandomly);
                }
            }
            return (date - startDate).Days;
        }


        public int[] SimulateStoryRefinement(double darkIssuesFraction, params int[][] issuesPerStoryLogs)
        {
            throw new NotImplementedException();
        }
        
        
        private int PickRandomly(int[] log)
            => log[_nextRnd(log.Length)];
    }
}