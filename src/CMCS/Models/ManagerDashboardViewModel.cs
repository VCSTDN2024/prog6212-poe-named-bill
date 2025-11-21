using System.Collections.Generic;
using CMCS.ViewModels;

namespace CMCS.Models
{
    public class ManagerDashboardViewModel
    {
        public string? CurrentUser { get; set; }
        public IEnumerable<CoordinatorClaimRow> VerifiedClaims { get; set; } = new List<CoordinatorClaimRow>();
    }
}
