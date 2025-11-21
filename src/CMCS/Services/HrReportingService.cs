using System.Globalization;
using System.Linq;
using System.Text;
using CMCS.Data;
using CMCS.Models;

namespace CMCS.Services
{
    public class HrReportingService
    {
        private readonly IClaimRepository _repo;

        public HrReportingService(IClaimRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<HrReportRow> BuildReportRows()
        {
            var approved = _repo.GetByStatus(ClaimStatus.Approved);
            foreach (var claim in approved)
            {
                yield return new HrReportRow
                {
                    ClaimId = claim.Id,
                    Lecturer = claim.SubmittedBy ?? "Unknown",
                    Department = _repo.GetUserByName(claim.SubmittedBy ?? string.Empty)?.Department ?? "Unknown",
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    TotalAmount = claim.TotalAmount,
                    SubmittedAt = claim.SubmittedAt,
                    ApprovedAt = _repo.GetApprovalsForClaim(claim.Id).LastOrDefault()?.ApprovedAt
                };
            }
        }

        public string GenerateCsv()
        {
            var rows = BuildReportRows().ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ClaimId,Lecturer,Department,Hours,Rate,Total,SubmittedAt,ApprovedAt");
            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(',', new[]
                {
                    row.ClaimId.ToString(),
                    Escape(row.Lecturer),
                    Escape(row.Department),
                    row.HoursWorked.ToString("F2", CultureInfo.InvariantCulture),
                    row.HourlyRate.ToString("F2", CultureInfo.InvariantCulture),
                    row.TotalAmount.ToString("F2", CultureInfo.InvariantCulture),
                    row.SubmittedAt.ToString("u"),
                    row.ApprovedAt?.ToString("u") ?? string.Empty
                }));
            }
            return sb.ToString();
        }

        public (int totalClaims, double totalAmount) GetAggregate()
        {
            var rows = BuildReportRows().ToList();
            return (rows.Count, rows.Sum(r => r.TotalAmount));
        }

        private static string Escape(string value)
        {
            if (value.Contains(',') || value.Contains('"'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
