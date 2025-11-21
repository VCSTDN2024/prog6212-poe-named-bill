using CMCS.Services;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimAutomationServiceTests
    {
        private readonly ClaimAutomationService _service = new();

        [Fact]
        public void Calculate_ReturnsTotalForValidData()
        {
            var result = _service.Calculate(10, 500);

            Assert.True(result.IsValid);
            Assert.Equal(5000, result.TotalAmount);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData(0, 200)]
        [InlineData(-5, 200)]
        [InlineData(50, -10)]
        public void Calculate_FlagsInvalidInputs(double hours, double rate)
        {
            var result = _service.Calculate(hours, rate);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void Calculate_WarnsForLargeClaims()
        {
            var result = _service.Calculate(170, 500);

            Assert.True(result.Warnings.Count > 0);
            Assert.True(result.TotalAmount > 0);
        }
    }
}
