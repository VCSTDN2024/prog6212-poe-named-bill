using System.Linq;
using System.Text;
using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class HrController : Controller
    {
        private readonly IClaimRepository _repo;
        private readonly HrReportingService _reporting;

        public HrController(IClaimRepository repo, HrReportingService reporting)
        {
            _repo = repo;
            _reporting = reporting;
        }

        public IActionResult Index()
        {
            if (!IsHr()) return Forbid();
            var rows = _reporting.BuildReportRows().Take(10).ToList();
            var aggregate = _reporting.GetAggregate();
            var model = new HrDashboardViewModel
            {
                ApprovedClaimCount = aggregate.totalClaims,
                ApprovedClaimValue = aggregate.totalAmount,
                ReportRows = rows,
                Lecturers = _repo.GetUsersByRole("Lecturer")
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult DownloadReport()
        {
            if (!IsHr()) return Forbid();
            var csv = _reporting.GenerateCsv();
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"cmcs-approved-claims-{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        public IActionResult Lecturers()
        {
            if (!IsHr()) return Forbid();
            var lecturers = _repo.GetUsersByRole("Lecturer");
            return View(lecturers);
        }

        [HttpPost]
        public IActionResult UpdateLecturer(User user)
        {
            if (!IsHr()) return Forbid();
            _repo.UpdateUser(user);
            TempData["HrSuccess"] = $"Updated profile for {user.FullName}";
            return RedirectToAction(nameof(Lecturers));
        }

        private bool IsHr()
        {
            var role = HttpContext.Session.GetString("role");
            return !string.IsNullOrEmpty(role) && role.Equals("HR", StringComparison.OrdinalIgnoreCase);
        }
    }
}
