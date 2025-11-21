using System.Collections.Generic;
using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimWorkflowServiceTests
    {
        private readonly ClaimWorkflowService _workflow;
        private readonly InMemoryClaimRepository _repo;

        public ClaimWorkflowServiceTests()
        {
            _repo = new InMemoryClaimRepository();
            _workflow = new ClaimWorkflowService(_repo, new ClaimReviewService(new ClaimAutomationService()));
        }

        [Fact]
        public void AutoVerify_Succeeds_ForCleanClaim()
        {
            var claim = _repo.Add(new Claim
            {
                HoursWorked = 15,
                HourlyRate = 300,
                SubmittedBy = "alice",
                Notes = "Lectures",
                AttachedFiles = new List<string> { "proof.pdf" }
            });

            var result = _workflow.AutoVerify(claim.Id, "bob");

            Assert.True(result.Success);
            Assert.Equal(ClaimStatus.Verified, _repo.Get(claim.Id)!.Status);
        }

        [Fact]
        public void AutoApprove_RequiresVerifiedStatus()
        {
            var claim = _repo.Add(new Claim { HoursWorked = 10, HourlyRate = 200, SubmittedBy = "alice" });
            var result = _workflow.AutoApprove(claim.Id, "carol");

            Assert.False(result.Success);
            Assert.Contains("Only verified", result.Message);
        }
    }
}
