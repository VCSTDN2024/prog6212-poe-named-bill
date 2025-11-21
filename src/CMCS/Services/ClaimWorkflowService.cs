using System;
using System.Collections.Generic;
using CMCS.Data;
using CMCS.Models;

namespace CMCS.Services
{
    public class ClaimWorkflowService
    {
        private readonly IClaimRepository _repo;
        private readonly ClaimReviewService _reviewService;

        public ClaimWorkflowService(IClaimRepository repo, ClaimReviewService reviewService)
        {
            _repo = repo;
            _reviewService = reviewService;
        }

        public ClaimReviewSummary GetSummary(int claimId)
        {
            var claim = _repo.Get(claimId) ?? throw new ArgumentException("Claim not found", nameof(claimId));
            return _reviewService.Analyze(claim);
        }

        public WorkflowActionResult AutoVerify(int claimId, string verifier)
        {
            var claim = _repo.Get(claimId);
            if (claim == null) return Fail("Claim not found");
            var summary = _reviewService.Analyze(claim);
            if (!summary.MeetsPolicy)
            {
                return Fail("Policy checks failed. Manual review required.", summary);
            }

            _repo.VerifyClaim(claimId, verifier);
            return new WorkflowActionResult
            {
                Success = true,
                Message = "Claim auto-verified by workflow engine.",
                Summary = summary
            };
        }

        public WorkflowActionResult AutoApprove(int claimId, string approver)
        {
            var claim = _repo.Get(claimId);
            if (claim == null) return Fail("Claim not found");
            if (claim.Status != ClaimStatus.Verified)
            {
                return Fail("Only verified claims can be auto-approved");
            }

            var summary = _reviewService.Analyze(claim);
            if (!summary.MeetsPolicy || summary.RiskScore > 0.5)
            {
                return Fail("Risk score too high for auto-approval", summary);
            }

            claim.Status = ClaimStatus.Approved;
            _repo.Update(claim);
            _repo.AddApproval(new Approval
            {
                ClaimId = claimId,
                ApprovedBy = approver,
                ApprovedAt = DateTime.UtcNow,
                IsApproved = true
            });

            return new WorkflowActionResult
            {
                Success = true,
                Message = "Claim auto-approved and ready for HR processing.",
                Summary = summary
            };
        }

        public IReadOnlyCollection<WorkflowActionResult> BulkApprove(IEnumerable<int> claimIds, string approver)
        {
            var results = new List<WorkflowActionResult>();
            foreach (var id in claimIds)
            {
                results.Add(AutoApprove(id, approver));
            }
            return results;
        }

        private static WorkflowActionResult Fail(string message, ClaimReviewSummary? summary = null) => new()
        {
            Success = false,
            Message = message,
            Summary = summary
        };
    }
}
