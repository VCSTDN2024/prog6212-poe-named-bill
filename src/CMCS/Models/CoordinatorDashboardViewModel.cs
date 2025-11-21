using System.Collections.Generic;
using CMCS.ViewModels;

namespace CMCS.Models
{
    public class CoordinatorDashboardViewModel
    {
        public string? CurrentUser { get; set; }
        public IEnumerable<CoordinatorClaimRow> PendingClaims { get; set; } = new List<CoordinatorClaimRow>();
    }
}
