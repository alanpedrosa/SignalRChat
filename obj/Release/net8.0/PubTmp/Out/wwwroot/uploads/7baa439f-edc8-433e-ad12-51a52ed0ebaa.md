
# PedrosaChat

Sistema de chat em tempo real utilizando SignalR, desenvolvido em ASP.NET Core MVC, com integração de Inteligência Artificial (IA) via OpenRouter.

Este projeto está em desenvolvimento.

## Funcionalidades

- Chat em tempo real com SignalR
- Gerenciamento de salas e usuários
- Mensagens públicas e privadas
- Upload de arquivos (imagens, documentos, etc.)
- Notificações sonoras
- Modo claro e escuro
- Integração com IA (OpenRouter)
- Resumo de conversas via IA
- Botão de “Chamar Atenção” (som e notificação)
- Tela de administração (gestão de usuários e salas)
- Sistema de login com controle de Admin

## Tecnologias Utilizadas

- ASP.NET Core MVC (.NET 8)
- SignalR
- Bootstrap 5 + Icons
- Entity Framework Core
- OpenRouter API
- SQL Server
- HTML, CSS, JavaScript

## Instalação e Execução Local

### 1. Clone o repositório

```
git clone https://github.com/seuusuario/PedrosaChat.git
cd PedrosaChat
```

### 2. Configure o banco de dados

- Crie o banco `PedrosaChat` no SQL Server.
- Execute os scripts de criação das tabelas que estão na pasta `/Database`.

### 3. Configure o arquivo `appsettings.json`

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PedrosaChat;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "OpenRouter": {
    "ApiKey": "SUA_API_KEY_AQUI",
    "BaseUrl": "https://openrouter.ai/api/v1",
    "Model": "mistralai/mixtral-8x7b"
  }
}
```

### 4. Execute o projeto

```
dotnet restore
dotnet build
dotnet run
```

Acesse no navegador:

```
https://localhost:7048
```

## Login Padrão (Admin)

| Usuário | Senha | Acesso  |
|---------|-------|---------|
| admin   | admin | Admin   |

## Estrutura de Pastas

```
/Controllers       -> Lógicas de usuário, sala, chat, IA
/Models            -> Entidades (Usuario, Sala)
/Data              -> DbContext e acesso ao banco
/wwwroot           -> Sons, ícones, CSS, JS
/Views             -> Frontend Razor
/Database          -> Scripts SQL
```

## Integração com IA (OpenRouter)

Permite consultas a modelos como:

- GPT-4 Turbo
- Mistral
- Claude 3

Funcionalidades disponíveis:

- Modo IA ativo no chat
- Geração de resumo da conversa

Documentação OpenRouter: https://openrouter.ai/docs

## Funcionalidades Extras

- Sons personalizados para notificações
- Chamar Atenção: som e animação no frontend
- Suporte a Tema Escuro e Claro
- Upload de arquivos diretamente no chat

## Contribuição

1. Faça um fork.
2. Crie uma branch: `git checkout -b feature/sua-feature`
3. Commit suas mudanças: `git commit -m 'Adiciona nova feature'`
4. Push na sua branch: `git push origin feature/sua-feature`
5. Abra um Pull Request.

## Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## Desenvolvido por

Alan Pedrosa  
https://linkedin.com/in/alanpedrosa
