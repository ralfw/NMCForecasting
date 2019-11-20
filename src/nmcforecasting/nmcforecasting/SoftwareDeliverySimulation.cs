using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using nmcforecasting.supporting;

namespace nmcforecasting
{
    public class SoftwareDeliverySimulation
    {
        private const int DEFAULT_NUMBER_OF_SIMULATION_RUNS = 10000;
        
        private readonly int _numberOfSimulationRuns;
        private readonly Func<int,int> _nextRnd;
        
        public SoftwareDeliverySimulation() : this(DEFAULT_NUMBER_OF_SIMULATION_RUNS){}
        public SoftwareDeliverySimulation(int numberOfSimulationRuns) { 
            _numberOfSimulationRuns = numberOfSimulationRuns;
            
            var rnd = new Random();
            _nextRnd = maxValue => rnd.Next(maxValue);
        }
        internal SoftwareDeliverySimulation(int numberOfSimulationRuns, Func<int,int> nextRnd) { 
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
                    /*
                     * For each new day a TP value is picked from all (!) logs passed in.
                     * Maybe one is a backend feature log, the other is a frontend feature log, and yet another one
                     * is a bug log. Then eg. the TPs (1,0,2) are picked resulting in a total TP of 3.
                     *
                     * Assumption: Several type specific TP logs are more sparsely populated than a single TP log for all
                     * sorts of issue types.
                     */
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
         * First derive the story refinement history from the issue data.
         * For the number of stories in question simulate the number of issues
         * they could lead to.
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
            return SimulateIssueDeliveryForEveryNumberOfIssues(startDate, throughputLog, issuesSimulation);
        }

        private int[] SimulateIssueDeliveryForEveryNumberOfIssues(DateTime startDate, int[] throughputLog, int[] issuesSimulation) {
            var issuesHistogram = Statistics.Histogram(issuesSimulation);
            return issuesHistogram.SelectMany(SimulateDelivery).ToArray();

            IEnumerable<int> SimulateDelivery((int numberOfIssues, int frequency) refinement) {
                var deliverySimulation = SimulateIssueDeliveryBasedOnThroughput(startDate, refinement.numberOfIssues, throughputLog).ToArray();
                return Enumerable.Range(1, refinement.frequency).SelectMany(_ => deliverySimulation);
                /*
                 * Every simulation for the delivery of a number of issues needs to be output
                 * as many times as the number of issues occurs in the histogram. Only that way
                 * each delivery time in the end shows up with the correct frequency in the
                 * final distribution.
                 *
                 * The histogram is used to reduce the number of SimulateIssueDeliveryBasedOnThroughput() calls.
                 * It's much faster to "multiply" the simulation result than running the simulation as the
                 * frequency calls for.
                 */
            }
        }
    }
}