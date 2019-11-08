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
            var sut = new MonteCarloSimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryByResources(historicalCTs);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{3,2,1}, result);
        }
        
        [Fact]
        public void Two_issues_single_resource() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new MonteCarloSimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryByResources(historicalCTsA, historicalCTsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{23,31,12}, result);
        }
        
        [Fact]
        public void Two_issues_two_resources() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new MonteCarloSimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryByResources(2, historicalCTsA, historicalCTsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{20,30,10}, result);
        }


        [Fact]
        public void Single_throughput_history() {
            var historicalTPs = new[] {1, 2, 3};
            var randomNumbers = new Queue<int>(new[] {2,2,2,2, 0,1,2,0,1,2});
            var sut = new MonteCarloSimulation(2, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryBasedOnThroughput(new DateTime(2019, 10, 3), 10, historicalTPs);
            
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
            var sut = new MonteCarloSimulation(2, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryBasedOnThroughput(new DateTime(2019, 10, 3), 10, historicalTPsA, historicalTPsB);
            
            Assert.Empty(randomNumbers);
            Assert.Equal(new[]{4, 7}, result);
        }
    }
}