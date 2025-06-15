using SignalR.Models;

namespace SignalR.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObterTodosAsync();
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<Usuario?> ObterPorUinAsync(long uin);
        Task<bool> UsuarioEhAdminAsync(long uin);
        Task<Usuario?> AutenticarAsync(string nome, string senha);
        Task AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task RemoverAsync(Usuario usuario);
        Task<bool> ExisteAsync(Guid id);
    }
}
