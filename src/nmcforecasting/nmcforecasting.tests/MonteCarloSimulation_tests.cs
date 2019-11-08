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
            
            Assert.Equal(new[]{3,2,1}, result);
        }
        
        [Fact]
        public void Two_issues_single_resource() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new MonteCarloSimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryByResources(historicalCTsA, historicalCTsB);
            
            Assert.Equal(new[]{23,31,12}, result);
        }
        
        [Fact]
        public void Two_issues_two_resources() {
            var historicalCTsA = new[] {1, 2, 3};
            var historicalCTsB = new[] {10, 20, 30};
            var randomNumbers = new Queue<int>(new[] {2,1, 0,2, 1,0});
            var sut = new MonteCarloSimulation(3, maxNumber => randomNumbers.Dequeue());

            var result = sut.SimulateDeliveryByResources(2, historicalCTsA, historicalCTsB);
            
            Assert.Equal(new[]{20,30,10}, result);
        }
    }
}