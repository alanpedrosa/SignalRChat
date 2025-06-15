using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Interfaces;
using SignalR.Models;

namespace SignalR.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private readonly SignalRDbContext _context;

        public SalaRepository(SignalRDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sala>> ObterTodasAsync()
        {
            return await _context.Salas.ToListAsync();
        }

        public async Task<Sala?> ObterPorIdAsync(Guid id)
        {
            return await _context.Salas.FindAsync(id);
        }

        public async Task AdicionarAsync(Sala sala)
        {
            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Sala sala)
        {
            _context.Salas.Update(sala);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Sala sala)
        {
            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _context.Salas.AnyAsync(e => e.Id == id);
        }
    }
}
