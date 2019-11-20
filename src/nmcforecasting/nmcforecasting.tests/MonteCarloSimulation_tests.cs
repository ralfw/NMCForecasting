using System;
using System.Collections.Generic;
using Xunit;

namespace nmcforecasting.tests
{
    public class MonteCarloSimulation_tests
    {
        [Fact]
        public void Single_issue_single_resource() {
            var historicalCTs = new[] {1, 2, 3};
            var randomNumbers = new Queue<int>(new[] {2, 1, 0});
            var sut = new SoftwareDeliverySimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssueDeliveryByResources(historicalCTs);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{3,2,1}, result);
        }
        
        [Fact]
        public void Two_issues_single_resource() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new SoftwareDeliverySimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssueDeliveryByResources(historicalCTsA, historicalCTsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{23,31,12}, result);
        }
        
        [Fact]
        public void Two_issues_two_resources() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new SoftwareDeliverySimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssueDeliveryByResources(2, historicalCTsA, historicalCTsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{20,30,10}, result);
        }


        [Fact]
        public void Single_throughput_history() {
            var historicalTPs = new[] {1, 2, 3};
            var randomNumbers = new Queue<int>(new[] {2,2,2,2, 0,1,2,0,1,2});
            var sut = new SoftwareDeliverySimulation(2, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssueDeliveryBasedOnThroughput(new DateTime(2019, 10, 3), 10, historicalTPs);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{5, 7}, result);
        }
        
        [Fact]
        public void Multiple_throughput_histories() {
            var historicalTPsA = new[] {1, 2, 3};
            var historicalTPsB = new[] {0, 1, 2};
            var randomNumbers = new Queue<int>(new[] {
                1,2, // 2+2=4
                2,0, // 3+0=3/7
                0,2, // 1+2=3/10
                
                0,1, // 1+1=2
                1,0, // 2+0=2/4
                2,0, // 3+0=3/7
                0,0, // 1+0=1/8
                0,0, // 1+0=1/9
                1,2 // 2+2=4/13
            }); //2,2,3,1,1,
            var sut = new SoftwareDeliverySimulation(2, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssueDeliveryBasedOnThroughput(new DateTime(2019, 10, 3), 10, historicalTPsA, historicalTPsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{4, 7}, result);
        }


        [Fact]
        public void Story_refinement()
        {
            var historicalStoryRefinementsA = new[] {1, 2, 3};
            var historicalStoryRefinementsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            
            var sut = new SoftwareDeliverySimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateIssuesDerivedFromStories(historicalStoryRefinementsA, historicalStoryRefinementsB);
            
            Assert.Equal(new[]{23,31,12}, result);
        }


        [Fact]
        public void Story_delivery()
        {
            /*
             * Stories:
             *     1: 1 issue
             *     2: 2
             *     3: 3
             *     4: 4
             *  dark: 1
             *
             *  TP:
             *     30.9.    0
             *     1.10.    1
             *     2.10.    2
             *     3.10.    3
             *     (4.10.
             *     5.10.)
             *     6.10.    
             *     7.10.    4
             *     => [0,1,2,3,4]
             */
            var historicalData = new[] {
                new Issue(new DateTime(2019,10,1), new DateTime(2019,10,3), null, "1", false), 
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,7), null, "2", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "3", false),
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,1), null, "", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,4), null, "2", false),
                new Issue(new DateTime(2019,10,1), new DateTime(2019,10,2), null, "3", false),
                new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "4", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,3), null, "3", false),
                new Issue(new DateTime(2019,10,2), new DateTime(2019,10,7), null, "4", false),
                new Issue(new DateTime(2019,10,3), new DateTime(2019,10,7), null, "4", false),
                new Issue(new DateTime(2019,9,30), new DateTime(2019,10,2), null, "4", false),
            };
            var randomNumbers = new Queue<int>(new[]
            {
                // Simulate issues derived from stories
                0,1,
                2,3,
                // => [1+2=3, 3+4=7]
                
                // Simulate issue delivery
                // 3 issues to deliver
                0,1,2, // 3 days - 1
                2,3, // 2 days - 1
                
                // 7 issues to deliver
                1,2,3,1, // 4 days - 1
                1,1,1,1,1,1,1 // 9 days due to weekend - 1
            });
            var sut = new SoftwareDeliverySimulation(2, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateStoryDeliveryBasedOnThroughput(new DateTime(2019, 10, 1), 2, historicalData);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{2,1,3,8}, result);
        }
    }
}