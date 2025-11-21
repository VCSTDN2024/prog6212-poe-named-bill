namespace CMCS.Models
{
    public class WorkflowActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ClaimReviewSummary? Summary { get; set; }
    }
}
