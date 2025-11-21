using System.Collections.Generic;

namespace CMCS.Models
{
    public class ClaimReviewSummary
    {
        public int ClaimId { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public bool MeetsPolicy => FailedChecks.Count == 0;
        public double RiskScore { get; set; }
        public List<string> PassedChecks { get; } = new();
        public List<string> FailedChecks { get; } = new();
        public List<string> PolicyFlags { get; } = new();
        public string Recommendation { get; set; } = "Manual review required";
    }
}
