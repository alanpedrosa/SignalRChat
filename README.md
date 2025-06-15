
# PedrosaChat - SignalR

![GitHub repo size](https://img.shields.io/github/repo-size/seuusuario/PedrosaChat)
![GitHub contributors](https://img.shields.io/github/contributors/seuusuario/PedrosaChat)
![GitHub last commit](https://img.shields.io/github/last-commit/seuusuario/PedrosaChat)
![GitHub license](https://img.shields.io/github/license/seuusuario/PedrosaChat)

---

## 💬 Sobre o Projeto

**PedrosaChat** é um sistema de chat em tempo real desenvolvido em **ASP.NET Core com SignalR**, integrando também funcionalidades de **Inteligência Artificial (IA)** via OpenRouter.

> ⚠️ **Status:** Projeto em desenvolvimento contínuo.

---

## 🚀 Funcionalidades

- Chat em tempo real com SignalR
- Salas dinâmicas e persistentes no banco de dados
- Lista de participantes online/offline
- Envio de mensagens públicas e privadas
- Upload de arquivos e imagens
- Visualização de imagens em Lightbox
- Integração com IA (OpenRouter):
  - Geração de respostas
  - Resumo de conversas
  - Perguntas baseadas na seleção de texto
- Notificações sonoras e alertas de atenção
- Tema Claro e Escuro
- Layout responsivo para Desktop e Mobile

---

## 🏗️ Estrutura do Projeto

```
SignalR/
├── Controllers
├── Data
├── Hubs
├── Interfaces
├── Models
├── Repositories
├── Services
├── Views
├── wwwroot
│   ├── css
│   ├── js
│   └── sounds
├── appsettings.json
└── Program.cs
```

---

## 🧠 Tecnologias Utilizadas

- .NET 8
- ASP.NET Core MVC
- SignalR
- Entity Framework Core
- SQL Server
- Bootstrap 5
- JavaScript (ES6)
- OpenRouter API (IA)

---

## 💻 Como Executar Localmente

### Pré-requisitos:

- Visual Studio 2022 ou superior
- .NET SDK 8
- SQL Server

### Passos:

1. Clone o repositório:

```bash
git clone https://github.com/seuusuario/PedrosaChat.git
cd PedrosaChat
```

2. Configure o `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SignalR;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "OpenRouter": {
    "ApiKey": "SUA_API_KEY_OPENROUTER",
    "BaseUrl": "https://openrouter.ai/api/v1",
    "Model": "mistralai/mistral-7b"
  }
}
```

3. Crie o banco de dados:

```bash
dotnet ef database update
```

4. Execute o projeto:

```bash
dotnet run
```

Acesse o sistema via:

```
https://localhost:7233
```

---

## 📃 Licença

Distribuído sob a licença [MIT](LICENSE).

---

## 🚧 Aviso

Este projeto está em desenvolvimento. Algumas funcionalidades podem ser alteradas, melhoradas ou adicionadas futuramente.

---

## ✨ Autor

**Alan Pedrosa**  
[LinkedIn](https://www.linkedin.com) | [GitHub](https://github.com/seuusuario)

---
