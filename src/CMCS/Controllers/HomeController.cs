using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            // In production don't reveal details
            ViewBag.Message = "An unexpected error occurred. Please try again or contact support.";
            return View();
        }
    }
}
