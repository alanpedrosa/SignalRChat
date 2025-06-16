
# SignalRchat

Sistema de chat em tempo real desenvolvido em ASP.NET, SignalR e JavaScript.

## Funcionalidades

- Envio de mensagens em tempo real
- Upload e download de arquivos (imagens, documentos)
- Visualização de imagens em Lightbox
- Notificações sonoras para mensagens
- Botão de chamada de atenção
- Participantes online/offline
- Modo escuro e modo claro
- Integração com IA (OpenRouter) para resumos e consultas rápidas
- Gerenciamento de usuários e salas
- Modal de resposta da IA
- Suporte a anexos com múltiplos arquivos

## Tecnologias utilizadas

- ASP.NET Core (.NET 8)
- SignalR
- Entity Framework Core
- SQL Server
- Bootstrap 5
- JavaScript (puro)
- OpenRouter API (Integração IA)

## Como rodar o projeto localmente

1. Clone este repositório:

```bash
git clone https://github.com/seu-usuario/SignalRchat.git
```

2. Abra o projeto no **Visual Studio 2022**.

3. Configure o banco de dados no arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=SignalRDb;Trusted_Connection=True;TrustServerCertificate=True;"
},
"OpenRouter": {
  "ApiKey": "SUA_API_KEY",
  "BaseUrl": "https://openrouter.ai/api/v1/chat/completions",
  "Model": "mistralai/mixtral-8x7b"
}
```

4. Execute as migrações via Package Manager ou com `update-database`.

5. Clique em **Start** no Visual Studio.

6. Acesse:  
`https://localhost:{porta}`

## Status

> Este projeto ainda está em desenvolvimento. Novas funcionalidades estão sendo planejadas e implementadas.

## Contribuições

Contribuições são bem-vindas! Fique à vontade para abrir Issues ou Pull Requests.

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](./LICENSE) para mais informações.

## Links úteis

- [Documentação do SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction)
- [Documentação ASP.NET](https://learn.microsoft.com/aspnet/core/)
- [OpenRouter API](https://openrouter.ai/docs)
- [Bootstrap](https://getbootstrap.com/)
