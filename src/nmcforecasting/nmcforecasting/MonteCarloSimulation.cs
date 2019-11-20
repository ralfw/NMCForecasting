using System;
using System.Linq;

namespace nmcforecasting
{
    public class MonteCarloSimulation<TData,TResult>
    {
        private const int DEFAULT_NUMBER_OF_SIMULATION_RUNS = 10000;
        
        private readonly int _numberOfSimulationRuns;
        private readonly Func<TData[], TResult> _simulate;
        private readonly Func<int,int> _nextRnd;

        public MonteCarloSimulation(Func<TData[],TResult> simulate, 
            int numberOfSimulationRuns = DEFAULT_NUMBER_OF_SIMULATION_RUNS) { 
            _simulate = simulate;
            _numberOfSimulationRuns = numberOfSimulationRuns;

            var rnd = new Random();
            _nextRnd = maxValue => rnd.Next(maxValue);
        }
        internal MonteCarloSimulation(Func<TData[],TResult> simulate, int numberOfSimulationRuns, Func<int,int> nextRnd) { 
            _simulate = simulate;
            _numberOfSimulationRuns = numberOfSimulationRuns;
            _nextRnd = nextRnd;
        }
        

        public TResult[] Simulate(params TData[][] logs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                .Select(_ => {
                    var samples = Compile_samples(logs);
                    return _simulate(samples);
                })
                .ToArray();

        private TData[] Compile_samples(TData[][] logs)
            => logs.Select(PickRandomly).ToArray();
        
        private TData PickRandomly(TData[] log)
            => log[_nextRnd(log.Length)];
    }
}