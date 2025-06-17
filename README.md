
# 🚀 SignalRchat

Sistema de chat em tempo real desenvolvido em **ASP.NET**, **SignalR** e **JavaScript**, com integração de IA, criptografia de senhas e diversos recursos colaborativos.

---

## 🔐 Segurança — Criptografia de Senhas

✔️ As senhas dos usuários são armazenadas de forma **segura utilizando criptografia BCrypt**.

- 🔒 Criptografia aplicada no cadastro de usuários.
- 🔒 Criptografia aplicada na alteração de senha.
- 🔒 Durante o login, a senha informada é validada contra o hash armazenado.
- 🔒 As senhas **não são armazenadas em texto puro no banco de dados**.

---

## 🚀 Funcionalidades

- ✅ Envio de mensagens em tempo real.
- ✅ Upload e download de arquivos (imagens e documentos).
- ✅ Visualização de imagens em **Lightbox**.
- ✅ Notificações sonoras para mensagens.
- ✅ Botão de **chamada de atenção**.
- ✅ Participantes online/offline.
- ✅ **Modo escuro e modo claro**.
- ✅ **Integração com IA (OpenRouter)** para geração de resumos e consultas rápidas.
- ✅ **Gerenciamento de usuários e salas**.
- ✅ Modal para respostas da IA.
- ✅ Suporte a anexos com múltiplos arquivos.
- ✅ 🔐 **Criptografia de senhas com BCrypt**.
- ✅ Controle de usuários online e suas sessões (em dev).

---

## 🧰 Tecnologias Utilizadas

- C# .NET 8
- ASP.NET Core MVC
- SignalR
- Entity Framework Core
- SQL Server
- Bootstrap 5
- JavaScript (puro)
- OpenRouter API (Integração IA)
- **BCrypt.Net** (para criptografia de senhas)

---

## 💻 Como rodar o projeto localmente

1. Clone este repositório:

```bash
git clone https://github.com/seu-usuario/SignalRchat.git
```

2. Abra o projeto no **Visual Studio 2022**.

3. Configure a string de conexão e a chave da IA no arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=SignalRDb;Trusted_Connection=True;TrustServerCertificate=True;"
},
"OpenRouter": {
  "ApiKey": "SUA_API_KEY",
  "BaseUrl": "https://openrouter.ai/api",
  "Model": "mistralai/mixtral-8x7b"
}
```

4. Execute as migrações:

- No Package Manager Console:
```bash
update-database
```

5. Execute o projeto:

- Clique em **Start** no Visual Studio ou pressione `F5`.

6. Acesse no navegador:

```
https://localhost:{porta}
```

---

## 🚧 Status

> **Este projeto está em desenvolvimento.**  
> Novas funcionalidades estão sendo planejadas e implementadas constantemente.

---

## 🤝 Contribuições

Contribuições são bem-vindas! Fique à vontade para abrir **Issues** ou enviar **Pull Requests**.

---

## 📜 Licença

Este projeto está licenciado sob a licença **MIT**.  
Veja o arquivo [LICENSE](./LICENSE) para mais informações.

---

## 🔗 Links úteis

- [Documentação do SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction)
- [Documentação do ASP.NET](https://learn.microsoft.com/aspnet/core/)
- [Documentação da OpenRouter API](https://openrouter.ai/docs)
- [Bootstrap](https://getbootstrap.com/)

---

## 👨‍💻 Desenvolvido por **Alan Pedrosa**
