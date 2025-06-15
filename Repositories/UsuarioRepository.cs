using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Interfaces;
using SignalR.Models;

namespace SignalR.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SignalRDbContext _context;

        public UsuarioRepository(SignalRDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ObterTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Sala)
                .ToListAsync();
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Sala)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _context.Usuarios.AnyAsync(e => e.Id == id);
        }
        public async Task<Usuario?> AutenticarAsync(string nome, string senha)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nome == nome && u.Senha == senha);
        }

        public async Task<Usuario?> ObterPorUinAsync(long uin)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.UIN == uin);
        }

        public async Task<bool> UsuarioEhAdminAsync(long uin)
        {
            var usuario = await ObterPorUinAsync(uin);
            return usuario != null && usuario.Adm;
        }

    }
}
