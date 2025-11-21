using System.Collections.Generic;

namespace CMCS.Models
{
    public class ClaimCalculationRequest
    {
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
    }

    public class ClaimCalculationResult
    {
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();
        public bool IsValid => Errors.Count == 0;
    }
}
