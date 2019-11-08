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
            => new Backlog(cycleTimeLogs.Select(SimulateCycleTime).ToArray());

        private int SimulateCycleTime(int[] cycleTimeLog)
            => cycleTimeLog[_nextRnd(cycleTimeLog.Length)];
    }
}