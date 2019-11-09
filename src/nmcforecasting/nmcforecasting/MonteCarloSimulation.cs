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
        
        

        public int[] SimulateIssueDeliveryByResources(params int[][] cycleTimeLogs)
            => SimulateIssueDeliveryByResources(1, cycleTimeLogs);
        public int[] SimulateIssueDeliveryByResources(int numberOfResources, params int[][] cycleTimeLogs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                         .Select(_ => SimulateOnceIssueDeliveryByResources(numberOfResources, cycleTimeLogs))
                         .ToArray();
        
        private int SimulateOnceIssueDeliveryByResources(in int numberOfResources, in int[][] cycleTimeLogs)
            => SimulateBacklog(cycleTimeLogs).CalculateDeliveryTime(numberOfResources);

        private Backlog SimulateBacklog(IEnumerable<int[]> cycleTimeLogs)
            => new Backlog(cycleTimeLogs.Select(PickRandomly).ToArray());



        public int[] SimulateIssueDeliveryBasedOnThroughput(DateTime startDate, int numberOfIssues, params int[][] throughputLogs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                         .Select(_ => SimulateOnceIssueDeliveryBasedOnThroughput(startDate, numberOfIssues, throughputLogs))
                         .ToArray();

        private int SimulateOnceIssueDeliveryBasedOnThroughput(DateTime startDate, int numberOfIssues, params int[][] throughputLogs) {
            var date = startDate.Subtract(new TimeSpan(1,0,0,0));
            while (numberOfIssues > 0) {
                date = date.AddDays(1);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) {
                    numberOfIssues -= throughputLogs.Sum(PickRandomly);
                }
            }
            return (date - startDate).Days;
        }


        public int[] SimulateIssuesDerivedFromStories(params int[][] issuesPerStoryLogs)
            => Enumerable.Range(1, _numberOfSimulationRuns)
                .Select(_ => SimulateOnceIssuesDerivedFromStories(issuesPerStoryLogs))
                .ToArray();

        private int SimulateOnceIssuesDerivedFromStories(int[][] issuesPerStoryLogs)
            => issuesPerStoryLogs.Select(PickRandomly).Sum();

        
        private int PickRandomly(int[] log)
            => log[_nextRnd(log.Length)];


        /*
         * First derive the story refinement into issues from the issue data.
         * For the number of stories in question simulate the number of issues
         * they could get refined into.
         *
         * For all the possible numbers of issues simulate how long
         * their delivery would take based on the historical issue throughput.
         */
        public int[] SimulateStoryDeliveryBasedOnThroughput(DateTime startDate, int numberOfStories, Issue[] issues) {
            var issuesPerStoryLog = issues.IssuesPerStory();
            var issuesPerStoryLog_for_all_stories = Enumerable.Range(1, numberOfStories)
                                                              .Select(_ => issuesPerStoryLog).ToArray();
            var issuesSimulation = SimulateIssuesDerivedFromStories(issuesPerStoryLog_for_all_stories);

            var throughputLog = issues.BusinessDayThroughputs();
            return issuesSimulation.SelectMany(numberOfIssues => SimulateIssueDeliveryBasedOnThroughput(startDate, numberOfIssues, throughputLog))
                                   .ToArray();
        }
    }
}