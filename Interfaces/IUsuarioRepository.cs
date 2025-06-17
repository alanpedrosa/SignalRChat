using SignalR.Models;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<List<Usuario>> ObterTodosAsync();
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task RemoverAsync(Usuario usuario);
    Task<bool> ExisteAsync(Guid id);
    Task<Usuario?> ObterPorUinAsync(long uin);
    Task<bool> UsuarioEhAdminAsync(long uin);
    Task<Usuario?> AutenticarAsync(string nome, string senha);  
    Task AtualizarSenhaAsync(Guid usuarioId, string novaSenha);
}
