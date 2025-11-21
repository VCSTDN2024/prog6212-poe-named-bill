using CMCS.Data;
using CMCS.Models;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimRepositoryTests
    {
        [Fact]
        public void CanAddAndRetrieveClaim()
        {
            var repo = new InMemoryClaimRepository();
            var claim = new Claim { HoursWorked = 5, HourlyRate = 100 };
            var added = repo.Add(claim);
            var fetched = repo.Get(added.Id);
            Assert.NotNull(fetched);
            Assert.Equal(500, fetched.TotalAmount);
        }

        [Fact]
        public void CanApproveClaim()
        {
            var repo = new InMemoryClaimRepository();
            var claim = repo.Add(new Claim { HoursWorked = 2, HourlyRate = 50 });
            claim.Status = ClaimStatus.Approved;
            repo.Update(claim);
            var fetched = repo.Get(claim.Id);
            Assert.Equal(ClaimStatus.Approved, fetched.Status);
        }
    }
}