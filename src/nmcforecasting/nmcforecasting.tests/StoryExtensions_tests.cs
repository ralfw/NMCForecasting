using System;
using FluentAssertions;
using Xunit;

namespace nmcforecasting.tests
{
    public class StoryExtensions_tests
    {
        [Fact]
        public void All_issues_belong_to_stories()
        {
            var result = new[] {
                new Issue(DateTime.Now, DateTime.Now, null, "a", false),
                new Issue(DateTime.Now, DateTime.Now, null, "b", false),
                new Issue(DateTime.Now, DateTime.Now, null, "c", false),
                new Issue(DateTime.Now, DateTime.Now, null, "b", false),
                new Issue(DateTime.Now, DateTime.Now, null, "c", false),
                new Issue(DateTime.Now, DateTime.Now, null, "c", false),
            }.IssuesPerStory();
            
            result.Should().BeEquivalentTo(new[]{1,2,3});
        }
        
        
        [Fact]
        public void Some_dark_issues()
        {
            var result = new[] {
                new Issue(DateTime.Now, DateTime.Now, null, "a", false),
                new Issue(DateTime.Now, DateTime.Now, null, null, false),
                new Issue(DateTime.Now, DateTime.Now, null, "c", false),
                new Issue(DateTime.Now, DateTime.Now, null, "", false),
                new Issue(DateTime.Now, DateTime.Now, null, "   ", false),
                new Issue(DateTime.Now, DateTime.Now, null, "c", false),
            }.IssuesPerStory();
            
            result.Should().BeEquivalentTo(new[]{1,2,3});
        }
        
        
        [Fact]
        public void Only_dark_issues()
        {
            var result = new[] {
                new Issue(DateTime.Now, DateTime.Now, null, "", false),
                new Issue(DateTime.Now, DateTime.Now, null, null, false),
                new Issue(DateTime.Now, DateTime.Now, null, "", false),
            }.IssuesPerStory();
            
            result.Should().BeEquivalentTo(new[]{3});
        }
    }
}