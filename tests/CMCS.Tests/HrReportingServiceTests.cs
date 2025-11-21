using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Xunit;

namespace CMCS.Tests
{
    public class HrReportingServiceTests
    {
        [Fact]
        public void GenerateCsv_ReturnsRows()
        {
            var repo = new InMemoryClaimRepository();
            var claim = repo.Add(new Claim { HoursWorked = 12, HourlyRate = 450, SubmittedBy = "alice" });
            claim.Status = ClaimStatus.Approved;
            repo.Update(claim);
            repo.AddApproval(new Approval { ClaimId = claim.Id, ApprovedBy = "carol", IsApproved = true });

            var service = new HrReportingService(repo);
            var csv = service.GenerateCsv();

            Assert.Contains("ClaimId", csv);
            Assert.Contains(claim.Id.ToString(), csv);
        }
    }
}
