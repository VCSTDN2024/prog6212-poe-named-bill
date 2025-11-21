using System.Linq;
using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using CMCS.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly IClaimRepository _repo;
        private readonly ClaimWorkflowService _workflow;

        public CoordinatorController(IClaimRepository repo, ClaimWorkflowService workflow)
        {
            _repo = repo;
            _workflow = workflow;
        }

        public IActionResult Dashboard()
        {
            if (!IsCoordinator()) return Forbid();
            var pendingClaims = _repo.GetByStatus(ClaimStatus.Pending)
                .Select(c => new CoordinatorClaimRow
                {
                    Claim = c,
                    Review = _workflow.GetSummary(c.Id)
                })
                .ToList();

            var model = new CoordinatorDashboardViewModel
            {
                CurrentUser = HttpContext.Session.GetString("username"),
                PendingClaims = pendingClaims
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AutoVerify(int id)
        {
            if (!IsCoordinator()) return Forbid();
            var result = _workflow.AutoVerify(id, HttpContext.Session.GetString("username") ?? "coordinator");
            TempData[result.Success ? "CoordinatorSuccess" : "CoordinatorError"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public IActionResult Summary(int id)
        {
            if (!IsCoordinator()) return Forbid();
            try
            {
                var summary = _workflow.GetSummary(id);
                return Json(summary);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        private bool IsCoordinator()
        {
            var role = HttpContext.Session.GetString("role");
            return !string.IsNullOrEmpty(role) && role.Equals("Coordinator", StringComparison.OrdinalIgnoreCase);
        }
    }
}
