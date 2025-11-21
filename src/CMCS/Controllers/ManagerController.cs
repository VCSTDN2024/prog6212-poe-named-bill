using System;
using System.Linq;
using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using CMCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IClaimRepository _repo;
        private readonly ClaimWorkflowService _workflow;

        public ManagerController(IClaimRepository repo, ClaimWorkflowService workflow)
        {
            _repo = repo;
            _workflow = workflow;
        }

        public IActionResult Dashboard()
        {
            if (!IsManager()) return Forbid();
            var verifiedClaims = _repo.GetByStatus(ClaimStatus.Verified)
                .Select(c => new CoordinatorClaimRow
                {
                    Claim = c,
                    Review = _workflow.GetSummary(c.Id)
                })
                .ToList();

            var model = new ManagerDashboardViewModel
            {
                CurrentUser = HttpContext.Session.GetString("username"),
                VerifiedClaims = verifiedClaims
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AutoApprove(int id)
        {
            if (!IsManager()) return Forbid();
            var result = _workflow.AutoApprove(id, HttpContext.Session.GetString("username") ?? "manager");
            TempData[result.Success ? "ManagerSuccess" : "ManagerError"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public IActionResult BulkApprove(int[]? ids)
        {
            if (!IsManager()) return Forbid();
            var approver = HttpContext.Session.GetString("username") ?? "manager";
            var results = _workflow.BulkApprove(ids ?? Array.Empty<int>(), approver);
            var successCount = results.Count(r => r.Success);
            TempData[successCount > 0 ? "ManagerSuccess" : "ManagerError"] = successCount > 0
                ? $"Auto-approved {successCount} claims"
                : "No claims met the auto-approval policy.";
            return RedirectToAction(nameof(Dashboard));
        }

        private bool IsManager()
        {
            var role = HttpContext.Session.GetString("role");
            return !string.IsNullOrEmpty(role) && role.Equals("Manager", StringComparison.OrdinalIgnoreCase);
        }
    }
}
