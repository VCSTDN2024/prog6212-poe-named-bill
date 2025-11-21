using System;

namespace CMCS.Models
{
    public class Approval
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; }
    }
}
