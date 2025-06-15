using Microsoft.AspNetCore.Mvc;
using SignalR.Interfaces;

namespace SignalR.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public LoginController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nome, string senha)
        {
            var usuario = await _usuarioRepository.AutenticarAsync(nome, senha);

            if (usuario == null)
            {
                ViewBag.Erro = "Usuário ou senha inválidos";
                return View();
            }

            // Armazena dados do usuário logado na sessão
            HttpContext.Session.SetString("UIN", usuario.UIN.ToString());
            HttpContext.Session.SetString("Nome", usuario.Nome);
            HttpContext.Session.SetString("Adm", usuario.Adm.ToString());

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}

