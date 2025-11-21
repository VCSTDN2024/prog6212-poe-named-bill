using System;

namespace CMCS.Models
{
    public class HrReportRow
    {
        public int ClaimId { get; set; }
        public string Lecturer { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
