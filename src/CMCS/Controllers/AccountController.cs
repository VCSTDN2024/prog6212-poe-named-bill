using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly InMemoryClaimRepository _repo;

        public AccountController(IClaimRepository repo)
        {
            _repo = repo as InMemoryClaimRepository ?? throw new ArgumentNullException(nameof(repo));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string role)
        {
            if (string.IsNullOrWhiteSpace(username)) return View();
            // for demo: accept username and role
            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("role", role ?? "Lecturer");
            return RedirectToAction("Index", "Claims");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("role");
            return RedirectToAction("Index", "Claims");
        }
    }
}
