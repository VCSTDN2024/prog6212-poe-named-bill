using System.Collections.Generic;
using CMCS.Models;
using CMCS.Services;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimReviewServiceTests
    {
        private readonly ClaimReviewService _service = new(new ClaimAutomationService());

        [Fact]
        public void Analyze_FlagsMissingDocuments()
        {
            var claim = new Claim
            {
                Id = 1,
                HoursWorked = 20,
                HourlyRate = 400,
                SubmittedBy = "alice"
            };

            var summary = _service.Analyze(claim);

            Assert.Contains(summary.FailedChecks, c => c.Contains("No supporting"));
            Assert.False(summary.MeetsPolicy);
        }

        [Fact]
        public void Analyze_ApprovesCleanClaim()
        {
            var claim = new Claim
            {
                Id = 2,
                HoursWorked = 16,
                HourlyRate = 300,
                SubmittedBy = "alice",
                Notes = "Tutorial prep",
                AttachedFiles = new List<string> { "evidence.pdf" }
            };

            var summary = _service.Analyze(claim);

            Assert.True(summary.MeetsPolicy);
            Assert.True(summary.RiskScore <= 0.3);
        }
    }
}
