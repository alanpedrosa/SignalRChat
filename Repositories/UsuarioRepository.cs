using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Interfaces;
using SignalR.Models;
using SignalR.Services;

namespace SignalR.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SignalRDbContext _context;
        private readonly ICriptografiaService _criptografiaService;

        public UsuarioRepository(SignalRDbContext context, ICriptografiaService criptografiaService)
        {
            _context = context;
            _criptografiaService = criptografiaService;
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
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nome == nome);

            if (usuario != null)
            {
                bool senhaValida = _criptografiaService.VerificarSenha(senha, usuario.Senha);

                if (senhaValida)
                    return usuario;
            }

            return null;
        }
        public async Task AtualizarSenhaAsync(Guid usuarioId, string novaSenha)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            usuario.Senha = novaSenha;
            usuario.DataAlteracao = DateTime.Now;

            await _context.SaveChangesAsync();
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
