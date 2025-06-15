using SignalR.Models;

namespace SignalR.Interfaces
{
    public interface ISalaRepository
    {
        Task<List<Sala>> ObterTodasAsync();
        Task<Sala?> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(Sala sala);
        Task AtualizarAsync(Sala sala);
        Task RemoverAsync(Sala sala);
        Task<bool> ExisteAsync(Guid id);
    }
}
