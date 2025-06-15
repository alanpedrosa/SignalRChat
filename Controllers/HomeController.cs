using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR.Data;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignalRDbContext _context;

        public HomeController(SignalRDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UIN")))
                return RedirectToAction("Index", "Login");

            ViewBag.Salas = _context.Salas.ToList();
            return View();
        }       
    }
}
