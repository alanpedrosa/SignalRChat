using Microsoft.AspNetCore.Mvc;
using SignalR.Interfaces;

namespace SignalR.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AdminController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IActionResult> Index()
        {
            var uinStr = HttpContext.Session.GetString("UIN");

            if (string.IsNullOrEmpty(uinStr) || !long.TryParse(uinStr, out long uin))
                return RedirectToAction("Index", "Login");

            var ehAdmin = await _usuarioRepository.UsuarioEhAdminAsync(uin);

            if (!ehAdmin)
                return Unauthorized(); 

            return View();
        }

        public IActionResult Salas()
        {
            return View();
        }

        public IActionResult Usuarios()
        {
            return View();
        }
    }
}
