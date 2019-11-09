using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace nmcforecasting
{
    public static class StoryExtensions
    {
        public static int[] IssuesPerStory(this IEnumerable<Issue> issues) {
            var a_issues = issues.ToArray();
            
            var issuesPerStoryLog = a_issues.Where(x => !string.IsNullOrWhiteSpace(x.StoryID))
                                            .GroupBy(x => x.StoryID).Select(x => x.Count())
                                            .ToList();

            var numberOfDarkIssues = issuesPerStoryLog.Count > 0
                                        ? a_issues.Length - issuesPerStoryLog.Sum()
                                        : a_issues.Length;
            if (numberOfDarkIssues > 0) issuesPerStoryLog.Add(numberOfDarkIssues);

            return issuesPerStoryLog.ToArray();
        }
    }
}