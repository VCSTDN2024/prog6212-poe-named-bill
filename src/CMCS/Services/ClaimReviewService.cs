using System;
using System.Linq;
using CMCS.Models;

namespace CMCS.Services
{
    public class ClaimReviewService
    {
        private readonly ClaimAutomationService _calculator;

        public ClaimReviewService(ClaimAutomationService calculator)
        {
            _calculator = calculator;
        }

        public ClaimReviewSummary Analyze(Claim claim)
        {
            var calculation = _calculator.Calculate(claim.HoursWorked, claim.HourlyRate);
            var summary = new ClaimReviewSummary
            {
                ClaimId = claim.Id,
                SubmittedBy = claim.SubmittedBy ?? "Unknown",
                TotalAmount = calculation.TotalAmount,
                HoursWorked = calculation.HoursWorked,
                HourlyRate = calculation.HourlyRate,
                RiskScore = 0
            };

            AddCheck(summary, calculation.IsValid, "Calculation passed validation checks");
            AddThresholdCheck(summary, claim.HoursWorked <= 180, "Hours within 180 limit", "Hours exceed coordinator policy cap");
            AddThresholdCheck(summary, claim.HourlyRate <= 1000, "Hourly rate within guideline", "Hourly rate exceeds manager threshold");
            AddThresholdCheck(summary, calculation.TotalAmount <= 90000, "Total amount within monthly budget", "Total exceeds manager budget guard rail");

            if (claim.AttachedFiles?.Any() == true)
            {
                summary.PassedChecks.Add("Supporting documents present");
            }
            else
            {
                summary.FailedChecks.Add("No supporting documents uploaded");
                summary.RiskScore += 0.2;
            }

            if (!string.IsNullOrWhiteSpace(claim.Notes) && claim.Notes!.Length > 10)
            {
                summary.PassedChecks.Add("Notes include justification");
            }
            else
            {
                summary.PolicyFlags.Add("Add more detail to notes for auditing");
            }

            summary.RiskScore += calculation.Warnings.Count * 0.1;
            summary.RiskScore += calculation.Errors.Count * 0.5;

            if (!calculation.IsValid)
            {
                summary.FailedChecks.AddRange(calculation.Errors);
            }
            else if (calculation.Warnings.Count > 0)
            {
                summary.PolicyFlags.AddRange(calculation.Warnings);
            }

            summary.Recommendation = summary.MeetsPolicy
                ? "Auto-approve eligible"
                : "Requires manual review";

            summary.RiskScore = Math.Round(Math.Clamp(summary.RiskScore, 0, 1), 2);
            return summary;
        }

        private static void AddCheck(ClaimReviewSummary summary, bool condition, string successMessage)
        {
            if (condition)
            {
                summary.PassedChecks.Add(successMessage);
            }
            else
            {
                summary.FailedChecks.Add(successMessage);
            }
        }

        private static void AddThresholdCheck(ClaimReviewSummary summary, bool condition, string passText, string failText)
        {
            if (condition)
            {
                summary.PassedChecks.Add(passText);
            }
            else
            {
                summary.FailedChecks.Add(failText);
                summary.RiskScore += 0.3;
            }
        }
    }
}
