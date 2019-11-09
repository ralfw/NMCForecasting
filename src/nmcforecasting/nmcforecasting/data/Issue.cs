using System;

// ReSharper disable once CheckNamespace
namespace nmcforecasting
{
    public class Issue {
        public Issue(DateTime started, DateTime completed, string[] tags, string storyId, bool isBugfix) {
            Started = started;
            Completed = completed;
            Tags = tags;
            StoryID = storyId;
            IsBugfix = isBugfix;
        }

        public DateTime Started { get; }
        public DateTime Completed { get; }
        public string[] Tags { get; }
        public string StoryID { get; }
        public bool IsBugfix { get; }

        public TimeSpan CycleTime => Completed.Subtract(Started);
    }
}