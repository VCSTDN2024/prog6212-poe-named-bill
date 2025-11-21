using CMCS.Models;

namespace CMCS.ViewModels
{
    public class CoordinatorClaimRow
    {
        public Claim Claim { get; set; } = new Claim();
        public ClaimReviewSummary Review { get; set; } = new ClaimReviewSummary();
    }
}
