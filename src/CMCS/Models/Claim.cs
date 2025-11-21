using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public enum ClaimStatus { Pending, Verified, Approved, Rejected }
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        [Required]
    [Range(1, 1000)]
        public double HoursWorked { get; set; }

        [Required]
    [Range(1, 10000)]
        public double HourlyRate { get; set; }

    public string? SubmittedBy { get; set; }

        public string? Notes { get; set; }

        public List<string> AttachedFiles { get; set; } = new();

    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public double TotalAmount => HoursWorked * HourlyRate;
    }
}