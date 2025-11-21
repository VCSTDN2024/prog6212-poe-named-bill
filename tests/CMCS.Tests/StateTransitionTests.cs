using CMCS.Data;
using CMCS.Models;
using Xunit;

namespace CMCS.Tests
{
    public class StateTransitionTests
    {
        [Fact]
        public void VerifyClaim_SetsStatusToVerified_AndCreatesApproval()
        {
            var repo = new InMemoryClaimRepository();
            var claim = repo.Add(new Claim { HoursWorked = 1, HourlyRate = 10 });
            Assert.Equal(ClaimStatus.Pending, claim.Status);

            repo.VerifyClaim(claim.Id, "bob");

            var fetched = repo.Get(claim.Id);
            Assert.Equal(ClaimStatus.Verified, fetched.Status);
            var approvals = repo.GetApprovalsForClaim(claim.Id);
            Assert.Contains(approvals, a => a.ApprovedBy == "bob" && a.IsApproved);
        }

        [Fact]
        public void ApproveAndReject_UpdateStatusAndAddApproval()
        {
            var repo = new InMemoryClaimRepository();
            var claim = repo.Add(new Claim { HoursWorked = 2, HourlyRate = 20 });

            // Approve
            claim.Status = ClaimStatus.Approved;
            repo.Update(claim);
            var fetched = repo.Get(claim.Id);
            Assert.Equal(ClaimStatus.Approved, fetched.Status);

            // Reject
            var claim2 = repo.Add(new Claim { HoursWorked = 3, HourlyRate = 10 });
            claim2.Status = ClaimStatus.Rejected;
            repo.Update(claim2);
            var fetched2 = repo.Get(claim2.Id);
            Assert.Equal(ClaimStatus.Rejected, fetched2.Status);
        }
    }
}
