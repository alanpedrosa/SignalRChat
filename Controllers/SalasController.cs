using Microsoft.AspNetCore.Mvc;
using SignalR.Interfaces;
using SignalR.Models;

namespace SignalR.Controllers
{
    public class SalasController : Controller
    {
        private readonly ISalaRepository _salaRepository;

        public SalasController(ISalaRepository salaRepository)
        {
            _salaRepository = salaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var salas = await _salaRepository.ObterTodasAsync();
            return View(salas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sala sala)
        {
            if (ModelState.IsValid)
            {
                sala.Id = Guid.NewGuid();
                await _salaRepository.AdicionarAsync(sala);
                return RedirectToAction(nameof(Index));
            }
            return View(sala);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var sala = await _salaRepository.ObterPorIdAsync(id.Value);
            if (sala == null) return NotFound();

            return View(sala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Sala sala)
        {
            if (id != sala.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _salaRepository.AtualizarAsync(sala);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    if (!await _salaRepository.ExisteAsync(sala.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(sala);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var sala = await _salaRepository.ObterPorIdAsync(id.Value);
            if (sala == null) return NotFound();

            return View(sala);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sala = await _salaRepository.ObterPorIdAsync(id);
            if (sala != null)
            {
                await _salaRepository.RemoverAsync(sala);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
