using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimRepository _repo;
        private readonly IWebHostEnvironment _env;
        private readonly ClaimAutomationService _automation;

        public ClaimsController(IClaimRepository repo, IWebHostEnvironment env, ClaimAutomationService automation)
        {
            _repo = repo;
            _env = env;
            _automation = automation;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Login", "Account");
            var claims = _repo.GetAll();
            return View(claims);
        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Login", "Account");
            var role = HttpContext.Session.GetString("role");
            if (role != "Lecturer") return Forbid();
            ViewBag.AutomationSummary = new ClaimCalculationResult();
            return View(new Claim());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Claim model)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Lecturer") return Forbid();

            var calculation = _automation.Calculate(model.HoursWorked, model.HourlyRate);
            ViewBag.AutomationSummary = calculation;
            if (!calculation.IsValid)
            {
                foreach (var error in calculation.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            ViewBag.AutomationWarnings = calculation.Warnings;

            if (!ModelState.IsValid) return View(model);
            model.SubmittedAt = DateTime.UtcNow;
            model.SubmittedBy = HttpContext.Session.GetString("username");
            var added = _repo.Add(model);

            var files = Request.Form.Files;
            var savedFiles = new List<string>();
            if (files != null && files.Count > 0)
            {
                var uploadRoot = Path.Combine(_env.ContentRootPath, "uploads");
                var claimFolder = Path.Combine(uploadRoot, added.Id.ToString());
                Directory.CreateDirectory(claimFolder);
                const long maxFileSize = 5 * 1024 * 1024; // 5 MB per file
                long totalSize = 0;
                var allowed = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
                foreach (var f in files)
                {
                    if (f.Length == 0) continue;
                    totalSize += f.Length;
                    var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
                    if (!allowed.Contains(ext))
                    {
                        ModelState.AddModelError("files", $"File '{f.FileName}' has an unsupported type. Allowed: .pdf, .docx, .xlsx, .png, .jpg.");
                        continue;
                    }
                    if (f.Length > maxFileSize)
                    {
                        ModelState.AddModelError("files", $"File '{f.FileName}' exceeds max size of 5 MB.");
                        continue;
                    }
                    var safeName = Path.GetFileNameWithoutExtension(f.FileName);
                    safeName = string.Concat(safeName.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Replace(' ', '_');
                    var fileName = $"{Guid.NewGuid()}_{safeName}{ext}";
                    var filePath = Path.Combine(claimFolder, fileName);
                    try
                    {
                        using var stream = System.IO.File.Create(filePath);
                        await f.CopyToAsync(stream);
                        savedFiles.Add(Path.Combine(added.Id.ToString(), fileName));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("files", $"Failed to save '{f.FileName}': {ex.Message}");
                    }
                }
                if (totalSize > 10 * 1024 * 1024)
                {
                    // cleanup
                    foreach (var sf in savedFiles)
                    {
                        var p = Path.Combine(_env.ContentRootPath, "uploads", sf);
                        if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
                    }
                    ModelState.AddModelError("files", "Total upload size exceeds allowed limit (10 MB). Please upload smaller files or fewer files.");
                }
                if (ModelState.ErrorCount > 0)
                {
                    // do not leave orphaned claim without files if user needs to correct uploads; keep claim but show errors
                    return View(model);
                }
                if (savedFiles.Count > 0)
                {
                    if (_repo is InMemoryClaimRepository mem) mem.AttachFilesToClaim(added.Id, savedFiles);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download(int id, string file)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return Forbid();
            var role = HttpContext.Session.GetString("role");
            // Only allow roles to download for this demo
            if (role == null) return Forbid();
            // sanitize file param: should be in the form "{claimId}/{filename}"
            var idPrefix1 = id.ToString() + Path.DirectorySeparatorChar;
            var idPrefix2 = id.ToString() + "/";
            if (string.IsNullOrEmpty(file) || !(file.StartsWith(idPrefix1) || file.StartsWith(idPrefix2))) return Forbid();
            var full = Path.Combine(_env.ContentRootPath, "uploads", file);
            var normalized = Path.GetFullPath(full);
            var uploadsRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "uploads"));
            if (!normalized.StartsWith(uploadsRoot)) return Forbid();
            if (!System.IO.File.Exists(normalized)) return NotFound();
            var contentType = "application/octet-stream";
            return PhysicalFile(normalized, contentType, Path.GetFileName(normalized));
        }

        [HttpPost]
        public IActionResult Verify(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Coordinator") return Forbid();
            if (_repo is InMemoryClaimRepository mem)
            {
                mem.VerifyClaim(id, HttpContext.Session.GetString("username") ?? "unknown");
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var claim = _repo.Get(id);
            if (claim == null) return NotFound();
            if (_repo is InMemoryClaimRepository mem)
            {
                ViewBag.Approvals = mem.GetApprovalsForClaim(id);
            }
            return View(claim);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Coordinator" && role != "Manager") return Forbid();
            var claim = _repo.Get(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Approved;
            _repo.Update(claim);
            if (_repo is InMemoryClaimRepository mem)
            {
                mem.AddApproval(new Approval { ClaimId = id, ApprovedBy = HttpContext.Session.GetString("username") ?? "unknown", IsApproved = true });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Coordinator" && role != "Manager") return Forbid();
            var claim = _repo.Get(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Rejected;
            _repo.Update(claim);
            if (_repo is InMemoryClaimRepository mem)
            {
                mem.AddApproval(new Approval { ClaimId = id, ApprovedBy = HttpContext.Session.GetString("username") ?? "unknown", IsApproved = false });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}