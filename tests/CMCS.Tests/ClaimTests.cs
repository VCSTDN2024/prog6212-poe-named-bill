using CMCS.Models;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimTests
    {
        [Fact]
        public void TotalAmount_ComputesCorrectly()
        {
            var c = new Claim { HoursWorked = 8, HourlyRate = 75 };
            Assert.Equal(600, c.TotalAmount);
        }

        [Fact]
        public void TotalAmount_ZeroWhenZeroHours()
        {
            var c = new Claim { HoursWorked = 0, HourlyRate = 100 };
            Assert.Equal(0, c.TotalAmount);
        }
    }
}
