using Xunit;

namespace nmcforecasting.tests
{
    public class Backlog_tests
    {
        [Fact]
        public void Single_resource_delivery()
        {
            var sut = new Backlog(new[]{1,2,39});
            Assert.Equal(42, sut.CalculateDeliveryTime(1));
        }
        
        [Fact]
        public void Two_resources_each_one_issue()
        {
            var sut = new Backlog(new[]{1,41});
            Assert.Equal(41, sut.CalculateDeliveryTime(2));
        }
        
        [Fact]
        public void Two_resources_with_the_first_one_getting_smaller_issues() {
            var sut = new Backlog(new[]{6,20,4,9});
            Assert.Equal(20, sut.CalculateDeliveryTime(2));
        }
        
        [Fact]
        public void Two_resources_with_the_second_one_getting_smaller_issues() {
            var sut = new Backlog(new[]{20,6,4,9});
            Assert.Equal(20, sut.CalculateDeliveryTime(2));
        }
        
        [Fact]
        public void Growing_number_of_resources() {
            var sut = new Backlog(new[]{1,2,3,4,5,6,7,8,9});
            Assert.Equal(45, sut.CalculateDeliveryTime(1));
            Assert.Equal(25, sut.CalculateDeliveryTime(2));
            Assert.Equal(18, sut.CalculateDeliveryTime(3));
        }
    }
}