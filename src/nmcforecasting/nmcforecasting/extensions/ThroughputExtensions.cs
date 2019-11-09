using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace nmcforecasting
{
    public static class ThroughputExtensions
    {
        public static int[] BusinessDayThroughputs(this IEnumerable<Issue> issues) {
            var a_issues = issues.ToArray();
            var calendar = Build_delivery_calendar_without_weekends(a_issues);
            return Compile_daily_throughputs(calendar, a_issues);
        }


        private static IEnumerable<DateTime> Build_delivery_calendar_without_weekends(Issue[] issues) {
            var firstStartDate = issues.Min(x => x.Started);
            var lastCompletionDate = issues.Max(x => x.Completed);
            return Enumerable.Range(0, (lastCompletionDate - firstStartDate).Days + 1)
                             .Select(offset => firstStartDate.AddDays(offset))
                             .Where(date => !OnWeekend(date));  
        }

        private static int[] Compile_daily_throughputs(IEnumerable<DateTime> calendar, Issue[] issues) {
            var throughputs = new Dictionary<DateTime,int>();
            foreach (var date in calendar) throughputs[date] = 0;

            foreach (var issue in issues) {
                if (OnWeekend(issue.Completed)) throw new InvalidOperationException("Throughput calculation does not support issue delivery on weekends!");
                throughputs[issue.Completed] += 1;
            }

            return throughputs.Values.ToArray();
        }


        private static bool OnWeekend(DateTime date) 
            => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}