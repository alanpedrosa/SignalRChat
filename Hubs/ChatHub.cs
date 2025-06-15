using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, (string userName, string sala)> _usuarios = new();
    private static readonly Dictionary<string, DateTime> _offlineTemporarios = new(); // Nome → Data de saída
    private static readonly TimeSpan _tempoExibicao = TimeSpan.FromMinutes(3);

    public async Task RegisterUser(string userName, string sala)
    {
        _usuarios[Context.ConnectionId] = (userName, sala);
        await Groups.AddToGroupAsync(Context.ConnectionId, sala);

        // Se estava como offline temporário, remove
        _offlineTemporarios.Remove(userName);

        await BroadcastUserList(sala);
        await Clients.Group(sala).SendAsync("ReceiveMessage", "Sistema", $"{userName} entrou na sala '{sala}'", DateTime.Now.ToString("HH:mm:ss"));
    }

    public async Task SendMessage(string message, List<string> destinos)
    {
        if (!_usuarios.TryGetValue(Context.ConnectionId, out var remetente))
            return;

        var (user, sala) = remetente;
        var hora = DateTime.Now.ToString("HH:mm:ss");

        if (destinos == null || destinos.Count == 0 || destinos.Contains("Todos"))
        {
            // Mensagem pública
            await Clients.Group(sala).SendAsync("ReceiveMessage", user, message, hora);
        }
        else
        {
            // Mensagem privada
            var conexoesPrivadas = _usuarios
                .Where(u => destinos.Contains(u.Value.userName))
                .Select(u => u.Key)
                .ToList();

            // Também mostrar para quem enviou
            conexoesPrivadas.Add(Context.ConnectionId);

            await Clients.Clients(conexoesPrivadas).SendAsync("ReceivePrivateMessage", user, message, hora, destinos);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_usuarios.TryGetValue(Context.ConnectionId, out var data))
        {
            var (user, sala) = data;

            _usuarios.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sala);

            // Adiciona como offline temporário
            _offlineTemporarios[user] = DateTime.Now;

            // Agendamento para remoção após tempo limite
            _ = Task.Run(async () =>
            {
                await Task.Delay(_tempoExibicao);
                _offlineTemporarios.Remove(user);
                await BroadcastUserList(sala);
            });

            await Clients.Group(sala).SendAsync("ReceiveMessage", "Sistema", $"{user} saiu da sala.", DateTime.Now.ToString("HH:mm:ss"));
            await BroadcastUserList(sala);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private Task BroadcastUserList(string sala)
    {
        var online = _usuarios
            .Where(u => u.Value.sala == sala)
            .Select(u => u.Value.userName)
            .ToList();

        var offline = _offlineTemporarios.Keys.ToList();

        var todos = online
            .Union(offline)
            .Distinct()
            .ToList();

        return Clients.Group(sala).SendAsync("UserListUpdated", todos, online);
    }


}
