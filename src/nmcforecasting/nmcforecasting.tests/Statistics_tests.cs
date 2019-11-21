using Xunit;

namespace nmcforecasting.tests
{
    public class Statistics_tests
    {
        [Fact]
        public void Histogram()
        {
            var result = Statistics.Histogram(new[] {1, 4, 2, 3, 2, 3, 4, 3, 4, 4});
            Assert.Equal(new[]{(1,1), (2,2), (3,3), (4,4)}, result);
        }


        [Fact]
        public void Distribution() {
            var result = Statistics.Distribution(new[] {(3, 3), (1, 1), (2, 2), (4, 4)});
            
            Assert((1,1,0.1,10.0), result[0]);
            Assert((2,2,0.2,30.0), result[1]);
            Assert((3,3,0.3,60.0), result[2]);
            Assert((4,4,0.4,100.0), result[3]);

            void Assert((int v, int f, double p, double pc) expected, (int v, int f, double p, double pc) value) {
                Xunit.Assert.Equal(expected.v, value.v);
                Xunit.Assert.Equal(expected.f, value.f);
                Xunit.Assert.Equal(expected.p, value.p, 3);
                Xunit.Assert.Equal(expected.pc, value.pc, 3);
            }
        }

        [Fact]
        public void DistributionKPIs()
        {
            var data = new[] { 1,
                2,
                2,
                2,
                2,
                3,
                3,
                5,
                7,
                9};
            var dist = Statistics.Distribution(data);
            var result = Statistics.DistributionKPIs(dist);
            Assert.Equal(2, result.firstMode);
            Assert.Equal(3.6, result.mean);
            Assert.Equal(2.0, result.median);
        }
    }
}