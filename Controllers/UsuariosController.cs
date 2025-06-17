using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalR.Interfaces;
using SignalR.Models;
using SignalR.Services;
using SignalR.ViewModels;

namespace SignalR.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly ICriptografiaService _criptografiaService;

        public UsuariosController(IUsuarioRepository usuarioRepository, ISalaRepository salaRepository, ICriptografiaService criptografiaService)
        {
            _usuarioRepository = usuarioRepository;
            _salaRepository = salaRepository;
            _criptografiaService = criptografiaService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            var salas = await _salaRepository.ObterTodasAsync();
            ViewBag.Salas = new SelectList(salas, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            usuario.Id = Guid.NewGuid();
            usuario.DataEntrada = DateTime.Now;
            usuario.DataAlteracao = null;

            if (usuario.UIN == 0)
                usuario.UIN = new Random().NextInt64(100000000, 999999999);

            usuario.Sala = await _salaRepository.ObterPorIdAsync(usuario.SalaId);

            if (usuario.Sala == null)
                ModelState.AddModelError("SalaId", "Selecione uma sala válida.");

            if (ModelState.IsValid)
            {
                // 🔐 Criptografia da senha
                usuario.Senha = _criptografiaService.GerarHash(usuario.Senha);

                await _usuarioRepository.AdicionarAsync(usuario);
                return RedirectToAction(nameof(Index));
            }

            var salas = await _salaRepository.ObterTodasAsync();
            ViewBag.Salas = new SelectList(salas, "Id", "Nome", usuario.SalaId);

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> AlterarSenha(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var model = new AlterarSenhaViewModel
            {
                UsuarioId = usuario.Id,
                NomeUsuario = usuario.Nome
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
        {


            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _usuarioRepository.ObterPorIdAsync(model.UsuarioId);

            if (usuario == null)
                return NotFound();

            if (!_criptografiaService.VerificarSenha(model.SenhaAtual, usuario.Senha))
            {
                ModelState.AddModelError("SenhaAtual", "Senha atual incorreta.");
                return View(model);
            }

            usuario.Senha = _criptografiaService.GerarHash(model.NovaSenha);
            usuario.DataAlteracao = DateTime.Now;

            await _usuarioRepository.AtualizarAsync(usuario);

            TempData["Mensagem"] = "Senha alterada com sucesso!";

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioRepository.ObterPorIdAsync(id.Value);
            if (usuario == null) return NotFound();

            var salas = await _salaRepository.ObterTodasAsync();
            ViewBag.Salas = new SelectList(salas, "Id", "Nome", usuario.SalaId);

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();

            if (usuario.UIN == 0)
                usuario.UIN = new Random().NextInt64(100000000, 999999999);

            usuario.Sala = await _salaRepository.ObterPorIdAsync(usuario.SalaId);

            if (usuario.Sala == null)
                ModelState.AddModelError("SalaId", "Selecione uma sala válida.");

            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuario.Senha = _criptografiaService.GerarHash(usuario.Senha);
            }


            if (ModelState.IsValid)
            {
                usuario.DataAlteracao = DateTime.Now;

                try
                {
                    await _usuarioRepository.AtualizarAsync(usuario);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    if (!await _usuarioRepository.ExisteAsync(usuario.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            var salas = await _salaRepository.ObterTodasAsync();
            ViewBag.Salas = new SelectList(salas, "Id", "Nome", usuario.SalaId);

            return View(usuario);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioRepository.ObterPorIdAsync(id.Value);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario != null)
            {
                await _usuarioRepository.RemoverAsync(usuario);
            }
            return RedirectToAction(nameof(Index));
        }           

    }
}

