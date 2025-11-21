using System.Collections.Generic;

namespace CMCS.Models
{
    public class HrDashboardViewModel
    {
        public int ApprovedClaimCount { get; set; }
        public double ApprovedClaimValue { get; set; }
        public IEnumerable<HrReportRow> ReportRows { get; set; } = new List<HrReportRow>();
        public IEnumerable<User> Lecturers { get; set; } = new List<User>();
    }
}
