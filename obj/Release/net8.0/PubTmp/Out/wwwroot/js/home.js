
let notificacaoHabilitada = true;
let janelaEmFoco = true;
let arquivosSelecionados = [];

let modoIaAtivo = false;


window.onblur = () => janelaEmFoco = false;
window.onfocus = () => janelaEmFoco = true;

let currentUser = "";
let currentSala = "";

//gera UIN

let uin = Math.floor(100000000 + Math.random() * 900000000);

localStorage.setItem("uin", uin);

console.log("Gerado UIN:", uin);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", function (user, message, hora) {
    const li = document.createElement("li");
    li.className = "list-group-item";
    li.innerHTML = `<strong>${user}</strong> <small class="text-muted">[${hora}]</small>: ${message}`;
    document.getElementById("messagesList").appendChild(li);
    li.setAttribute("data-last", "true");

    if (notificacaoHabilitada && user !== currentUser) {
        const audio = document.getElementById("notificacaoAudio");
        audio?.play().catch(err => console.warn("Som bloqueado pelo navegador:", err));
    }

    scrollToLastMessage();

    if (modoIaAtivo && message && typeof message === "string" && message.trim().toLowerCase().startsWith("/ia")) {
        const comando = message.trim().substring(3).trim() + " (responda em português)";

        fetch("/api/ia/comando", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ comando })
        })
            .then(resp => resp.json())
            .then(data => {
                const respostaIa = data.resposta || "🤖 Não consegui entender sua solicitação.";

                // Faz a IA responder como ChatIA (somente no front-end)
                const li = document.createElement("li");
                li.className = "list-group-item list-group-item-light";
                const horaAtual = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                li.innerHTML = `<strong>ChatIA</strong> <small class="text-muted">[${horaAtual}]</small>: 🤖 ${respostaIa}`;
                document.getElementById("messagesList").appendChild(li);
                scrollToLastMessage();
            })
            .catch(err => console.error("Erro IA:", err));
    }


});
connection.on("ReceivePrivateMessage", function (user, message, hora, destinos) {
    const li = document.createElement("li");
    li.className = "list-group-item list-group-item-warning";
    li.innerHTML = `<strong>🔒 ${user}</strong> <small class="text-muted">[${hora}]</small>: ${message}`;

    document.getElementById("messagesList").appendChild(li);
    li.setAttribute("data-last", "true");
    scrollToLastMessage();

    if (message.includes("chamando sua atenção")) {
        vibrarTela();
    }

    if (notificacaoHabilitada && user !== currentUser) {
        const audio = document.getElementById("notificacaoAudio");
        audio?.play().catch(err => console.warn("Som bloqueado pelo navegador:", err));
    }
});

function toggleSom() {
    notificacaoHabilitada = !notificacaoHabilitada;
    document.getElementById("btnSom").innerText = notificacaoHabilitada ? "🔊 Som Ativo" : "🔇 Som Inativo";

}

connection.on("UserListUpdated", function (users) {
    const list = document.getElementById("usersList");
    list.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.className = "list-group-item d-flex justify-content-between align-items-center";
        li.innerHTML = `
                        ${user}
                        <span class="badge bg-success">🟢</span>
                    `;
        list.appendChild(li);
    });
});

connection.start().then(() => {
    document.getElementById("userSection").style.display = "block";
}).catch(err => console.error(err.toString()));

function setUser() {
    const user = document.getElementById("userInput").value.trim();
    const sala = document.getElementById("salaInput").value.trim();

    if (!user || !sala) return alert("Preencha nome e sala.");

    currentUser = user;
    currentSala = sala;

    // Gera UIN (caso ainda não exista)
    let uin = localStorage.getItem("uin");
    if (!uin) {
        uin = Math.floor(100000000 + Math.random() * 900000000);
        localStorage.setItem("uin", uin);
    }

    // Atualiza cabeçalho
    document.getElementById("userInfo").textContent = `${user} | UIN: ${uin} | Sala: ${sala}`;

    connection.invoke("RegisterUser", user, sala)
        .then(() => {
            document.getElementById("userSection").style.display = "none";
            document.getElementById("chatSection").style.display = "block";
            document.getElementById("btnSair").style.display = "inline-block";
            document.getElementById("barraEnvio").style.display = "block";
            document.getElementById("botoesSection").style.display = "flex";

            // ⚠️ Espera o DOM atualizar para garantir que o elemento existe
            requestAnimationFrame(() => {
                const mensagem = `👋 Bem-vindo, ${user}! Seu UIN é: ${uin}`;
                const li = document.createElement("li");
                li.className = "list-group-item list-group-item-info";
                li.innerHTML = `<strong>${mensagem}</strong>`;
                document.getElementById("messagesList")?.appendChild(li);
                scrollToLastMessage();
            });
        })
        .catch(err => console.error(err.toString()));
}
connection.on("UserListUpdated", function (todos, online) {
    const list = document.getElementById("usersList");
    const select = document.getElementById("destinatarioInput");

    list.innerHTML = "";
    select.innerHTML = `<option value="Todos">Todos</option>`; // Reset

    todos.sort().forEach(user => {
        const isOnline = online.includes(user);
        const li = document.createElement("li");
        li.className = "list-group-item d-flex align-items-center justify-content-between";

        const avatar = `<div class="avatar-circle me-2">${user.charAt(0).toUpperCase()}</div>`;

        li.innerHTML = `
                    <div class="d-flex align-items-center">
                        ${avatar}
                        <span>${user}</span>
                    </div>
                    <span class="badge ${isOnline ? 'bg-success' : 'bg-danger'}">
                        ${isOnline ? '🟢' : '🔴'}
                    </span>
                `;
        list.appendChild(li);

        if (user !== currentUser && isOnline) {
            const opt = document.createElement("option");
            opt.value = user;
            opt.text = user;
            select.appendChild(opt);
        }
    });
});


document.addEventListener("keydown", function (event) {
    const barraEnvio = document.getElementById("barraEnvio");
    const userSection = document.getElementById("userSection");

    const chatAtivo = barraEnvio && barraEnvio.style.display !== "none";
    const usuarioLogado = userSection && userSection.style.display === "none";

    if (event.key === "Enter" && !event.shiftKey && chatAtivo && usuarioLogado) {
        event.preventDefault();
        event.stopPropagation();
        sendMessage();
    }
});

function sendMessage() {
    const messageInput = document.getElementById("messageInput");
    const message = messageInput.value.trim();
    const destino = document.getElementById("destinatarioInput").value;
    const destinos = destino === "Todos" ? [] : [destino];

    // 🔥 🔥 🔥 Atenção aqui:
    if (!message && arquivosSelecionados.length === 0) {
        alert("Digite uma mensagem ou selecione um arquivo.");
        return;
    }

    // 🔥 Se há arquivos, faz upload
    if (arquivosSelecionados.length > 0) {
        const uploads = arquivosSelecionados.map(arquivo => {
            const formData = new FormData();
            formData.append("arquivo", arquivo);

            return fetch("/api/upload/arquivo", {
                method: "POST",
                body: formData
            })
                .then(resp => {
                    if (!resp.ok) throw new Error(`Erro no upload: ${resp.status}`);
                    return resp.json().then(data => ({ url: data.url, nome: arquivo.name }));
                });
        });

        Promise.all(uploads)
            .then(resultados => {
                let mensagensArquivos = resultados.map(item => {
                    if (/\.(jpg|jpeg|png|gif|bmp|webp)$/i.test(item.nome)) {
                        return `
                        <br>
                        <img src="${item.url}"
                             alt="imagem"
                             title="${item.nome}"
                             style="max-width: 200px; cursor: pointer; border-radius: 6px;"
                             onclick="abrirLightbox('${item.url}', '${item.nome}')">`;
                    } else {
                        return `
                        <br>
                        <a href="${item.url}"
                           target="_blank"
                           title="${item.nome}">
                            <i class="bi bi-paperclip"></i> ${item.nome}
                        </a>`;
                    }
                }).join("");

                const mensagemFinal = message ? `${message}${mensagensArquivos}` : `${mensagensArquivos}`;

                connection.invoke("SendMessage", mensagemFinal, destinos);

                // Limpa após envio
                arquivosSelecionados = [];
                document.getElementById("fileInput").value = "";
                document.getElementById("fileLabel").title = "Anexar arquivo";
                messageInput.value = "";
            })
            .catch(err => {
                console.error("Erro ao enviar arquivo:", err);
                alert("Erro ao enviar arquivo.");
            });
    }
    // 🔥 Se não há arquivos, mas há mensagem
    else if (message) {
        connection.invoke("SendMessage", message, destinos)
            .then(() => {
                messageInput.value = "";
            })
            .catch(err => {
                console.error(err.toString());
                alert("Erro ao enviar mensagem.");
            });
    }
}

function scrollToLastMessage() {
    const list = document.getElementById("messagesList");

    // Remove marcações anteriores (se houver)
    const previous = list.querySelector('[data-last]');
    if (previous) previous.removeAttribute('data-last');

    // Marca o último item
    const last = list.lastElementChild;
    if (last) {
        last.setAttribute('data-last', 'true');

        // Se for imagem, espera ela carregar antes de dar scroll
        const img = last.querySelector('img');
        if (img) {
            img.onload = () => {
                last.scrollIntoView({ behavior: "smooth", block: "end" });
            };
        } else {
            last.scrollIntoView({ behavior: "smooth", block: "end" });
        }
    }
}

function setUser() {
    const user = document.getElementById("userInput").value.trim();
    const sala = document.getElementById("salaInput").value.trim();

    if (!user || !sala) return alert("Preencha nome e sala.");

    currentUser = user;
    currentSala = sala;

    // Exibe o nome no cabeçalho

    document.getElementById("userInfo").textContent = `${user} | UIN: ${uin} | Sala: ${sala}`; //adicionado agora

    connection.invoke("RegisterUser", user, sala)
        .then(() => {
            document.getElementById("userSection").style.display = "none";
            document.getElementById("chatSection").style.display = "block";
            document.getElementById("btnSair").style.display = "inline-block";
        })
        .catch(err => console.error(err.toString()));
}



function scrollToBottom() {
    const list = document.getElementById("messagesList");
    list.scrollTop = list.scrollHeight;
}

function sairDoChat() {
    connection.stop().then(() => location.reload());
}

// Tema escuro


document.addEventListener("DOMContentLoaded", () => {
    if (localStorage.getItem("modoEscuro") === "true") {
        document.body.classList.add("dark-mode");
        document.getElementById("btnDark").innerText = "☀️ Claro";
    }
});

function toggleDarkMode() {
    const body = document.body;
    const btn = document.getElementById("btnDark");

    body.classList.toggle("dark-mode");
    const escuro = body.classList.contains("dark-mode");
    btn.innerText = escuro ? "☀️ Claro" : "🌙 Escuro";
    localStorage.setItem("modoEscuro", escuro);
}
//Upload de arquivo

document.getElementById("fileInput").addEventListener("change", function () {
    arquivosSelecionados = Array.from(this.files) || [];

    if (arquivosSelecionados.length > 0) {
        const nomes = arquivosSelecionados.map(a => a.name).join(", ");
        document.getElementById("fileLabel").title = `📎 ${nomes}`;
    } else {
        document.getElementById("fileLabel").title = "Anexar arquivo";
    }
});

//chamar atenção
function chamarAtencao() {
    const destino = document.getElementById("destinatarioInput").value;
    if (destino === "Todos") return;

    const mensagem = `🚨 ${currentUser} está chamando sua atenção!`;
    connection.invoke("SendMessage", mensagem, [destino]);
}

document.getElementById("destinatarioInput").addEventListener("change", function () {
    const btnAtencao = document.getElementById("btnAtencao");
    btnAtencao.style.display = this.value !== "Todos" ? "inline-block" : "none";
});

function vibrarTela() {
    const el = document.querySelector('.parent'); // agora funciona
    if (!el) return;

    el.classList.add('vibrando');
    setTimeout(() => el.classList.remove('vibrando'), 500);

    const som = document.getElementById('somAtencao');
    if (som) som.play().catch(err => console.warn("Erro ao tocar som:", err));
}

function abrirLightbox(url, nome) {
    const lightboxImage = document.getElementById("lightboxImage");
    const lightboxLabel = document.getElementById("lightboxLabel");

    lightboxImage.src = url;
    lightboxLabel.textContent = nome;

    const modal = new bootstrap.Modal(document.getElementById('lightboxModal'));
    modal.show();
}



// Controle de ativação do modo IA
document.getElementById("btnModoIa").addEventListener("click", () => {
    modoIaAtivo = !modoIaAtivo;
    const btn = document.getElementById("btnModoIa");
    btn.textContent = modoIaAtivo ? "🤖 Modo IA: ON" : "🤖 Modo IA: OFF";
});

document.getElementById("btnResumoIa").addEventListener("click", () => {
    const mensagens = Array.from(document.querySelectorAll("#messagesList li"))
        .map(li => li.innerText)
        .join("\n");

    fetch("/api/ia/resumo", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ conversa: mensagens })
    })
        .then(resp => resp.json())
        .then(data => {
            const li = document.createElement("li");
            li.className = "list-group-item list-group-item-secondary";
            li.innerHTML = `<strong>🤖 Resumo IA:</strong> ${data.resumo}`;
            document.getElementById("messagesList").appendChild(li);
            scrollToLastMessage();
        });
});

document.getElementById("messagesList").addEventListener("mouseup", function (e) {
    if (!modoIaAtivo) return;

    const selection = window.getSelection().toString().trim();
    if (selection.length > 0) {
        // Confirma se o usuário quer perguntar para IA
        if (confirm(`Deseja perguntar à IA sobre:\n\n"${selection}"`)) {
            consultarIaSelecionado(selection);
        }
    }
});

function consultarIaSelecionado(textoSelecionado) {
    document.getElementById("iaResposta").innerHTML = "🔄 Processando...";

    fetch("/api/ia/comando", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ comando: textoSelecionado + " (responda em português)" })
    })
        .then(resp => resp.json())
        .then(data => {
            const resposta = data.resposta || "🤖 Não consegui gerar uma resposta.";

            document.getElementById("iaResposta").innerHTML = resposta;

            const modal = new bootstrap.Modal(document.getElementById('iaModal'));
            modal.show();
        })
        .catch(err => {
            console.error(err);
            document.getElementById("iaResposta").innerHTML = "❌ Erro ao consultar IA.";
        });


}

function copiarIa() {
    const texto = document.getElementById("iaResposta").innerText;

    if (navigator.clipboard) {
        navigator.clipboard.writeText(texto)
            .then(() => {
                alert("Resposta copiada para a área de transferência!");
            })
            .catch(err => {
                console.error('Erro ao copiar: ', err);
                fallbackCopyText(texto);
            });
    } else {
        // Se não suportar navigator.clipboard (navegadores antigos)
        fallbackCopyText(texto);
    }
}

// Função fallback para navegadores que não suportam navigator.clipboard
function fallbackCopyText(text) {
    const textarea = document.createElement("textarea");
    textarea.value = text;
    textarea.style.position = "fixed";  // Evita que a página role
    textarea.style.opacity = "0";
    document.body.appendChild(textarea);
    textarea.focus();
    textarea.select();

    try {
        const sucesso = document.execCommand('copy');
        if (sucesso) {
            alert("Resposta copiada para a área de transferência!");
        } else {
            alert("Não foi possível copiar.");
        }
    } catch (err) {
        console.error('Erro ao copiar: ', err);
        alert("Erro ao copiar.");
    }

    document.body.removeChild(textarea);
}

//------------------------------------------daqui pra baixo vai ser verificado ainda!!!-------------------------------------------------

//// =====================
//// 🔗 Variáveis Globais
//// =====================
//let notificacaoHabilitada = true;
//let janelaEmFoco = true;
//let arquivosSelecionados = [];
//let modoIaAtivo = false;
//let currentUser = "";
//let currentSala = "";
//let uin = Math.floor(100000000 + Math.random() * 900000000);
//localStorage.setItem("uin", uin);
//console.log("Gerado UIN:", uin);

//// ===========================
//// 🔗 Conexão com SignalR
//// ===========================
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/chatHub")
//    .build();

//// ===========================
//// 🔗 Eventos de Conexão
//// ===========================
//window.onblur = () => janelaEmFoco = false;
//window.onfocus = () => janelaEmFoco = true;

//// ===========================
//// 🔔 Receber Mensagens Públicas
//// ===========================
//connection.on("ReceiveMessage", function (user, message, hora) {
//    adicionarMensagem(user, message, hora, "publica");
//});

//// ===========================
//// 🔒 Receber Mensagens Privadas
//// ===========================
//connection.on("ReceivePrivateMessage", function (user, message, hora) {
//    adicionarMensagem(user, message, hora, "privada");

//    if (message.includes("chamando sua atenção")) vibrarTela();
//});

//// ===========================
//// 👥 Atualização da Lista de Usuários
//// ===========================
//connection.on("UserListUpdated", function (todos, online) {
//    atualizarListaUsuarios(todos, online);
//});

//// ===========================
//// 🔗 Inicializar Conexão
//// ===========================
//connection.start().then(() => {
//    document.getElementById("userSection").style.display = "block";
//}).catch(err => console.error(err.toString()));

//// ==========================================================
//// 🚀 Funções Principais de Mensagens e Conexão
//// ==========================================================
//function adicionarMensagem(user, message, hora, tipo) {
//    const li = document.createElement("li");
//    li.className = tipo === "privada" ? "list-group-item list-group-item-warning" : "list-group-item";
//    li.innerHTML = `<strong>${tipo === "privada" ? "🔒 " : ""}${user}</strong> <small class="text-muted">[${hora}]</small>: ${message}`;
//    document.getElementById("messagesList").appendChild(li);
//    li.setAttribute("data-last", "true");

//    if (notificacaoHabilitada && user !== currentUser) {
//        const audio = document.getElementById("notificacaoAudio");
//        audio?.play().catch(err => console.warn("Som bloqueado:", err));
//    }

//    scrollToLastMessage();

//    if (modoIaAtivo && message.toLowerCase().startsWith("/ia")) {
//        consultarIaComando(message.substring(3).trim());
//    }
//}

//function atualizarListaUsuarios(todos, online) {
//    const list = document.getElementById("usersList");
//    const select = document.getElementById("destinatarioInput");

//    list.innerHTML = "";
//    select.innerHTML = `<option value="Todos">Todos</option>`;

//    todos.sort().forEach(user => {
//        const isOnline = online.includes(user);
//        const li = document.createElement("li");
//        li.className = "list-group-item d-flex align-items-center justify-content-between";

//        const avatar = `<div class="avatar-circle me-2">${user.charAt(0).toUpperCase()}</div>`;

//        li.innerHTML = `
//            <div class="d-flex align-items-center">
//                ${avatar}
//                <span>${user}</span>
//            </div>
//            <span class="badge ${isOnline ? 'bg-success' : 'bg-danger'}">
//                ${isOnline ? '🟢' : '🔴'}
//            </span>
//        `;
//        list.appendChild(li);

//        if (user !== currentUser && isOnline) {
//            const opt = document.createElement("option");
//            opt.value = user;
//            opt.text = user;
//            select.appendChild(opt);
//        }
//    });
//}

//// ==========================================================
//// ✍️ Funções de Usuário
//// ==========================================================
//function setUser() {
//    const user = document.getElementById("userInput").value.trim();
//    const sala = document.getElementById("salaInput").value.trim();

//    if (!user || !sala) return alert("Preencha nome e sala.");

//    currentUser = user;
//    currentSala = sala;

//    document.getElementById("userInfo").textContent = `${user} | UIN: ${uin} | Sala: ${sala}`;

//    connection.invoke("RegisterUser", user, sala)
//        .then(() => {
//            document.getElementById("userSection").style.display = "none";
//            document.getElementById("chatSection").style.display = "block";
//            document.getElementById("btnSair").style.display = "inline-block";
//            document.getElementById("barraEnvio").style.display = "block";
//            document.getElementById("botoesSection").style.display = "flex";

//            const li = document.createElement("li");
//            li.className = "list-group-item list-group-item-info";
//            li.innerHTML = `<strong>Bem-vindo, ${user}! Seu UIN é: ${uin}</strong>`;
//            document.getElementById("messagesList").appendChild(li);

//            scrollToLastMessage();
//        })
//        .catch(err => console.error(err.toString()));
//}

//function sairDoChat() {
//    connection.stop().then(() => location.reload());
//}

//// ==========================================================
//// 🔥 Envio de Mensagens e Arquivos
//// ==========================================================
//function sendMessage() {
//    const messageInput = document.getElementById("messageInput");
//    const message = messageInput.value.trim();
//    const destino = document.getElementById("destinatarioInput").value;
//    const destinos = destino === "Todos" ? [] : [destino];

//    if (!message && arquivosSelecionados.length === 0) {
//        alert("Digite uma mensagem ou selecione um arquivo.");
//        return;
//    }

//    if (arquivosSelecionados.length > 0) {
//        const uploads = arquivosSelecionados.map(arquivo => {
//            const formData = new FormData();
//            formData.append("arquivo", arquivo);

//            return fetch("/api/upload/arquivo", {
//                method: "POST",
//                body: formData
//            })
//                .then(resp => {
//                    if (!resp.ok) throw new Error(`Erro no upload: ${resp.status}`);
//                    return resp.json().then(data => ({ url: data.url, nome: arquivo.name }));
//                });
//        });

//        Promise.all(uploads)
//            .then(resultados => {
//                const mensagensArquivos = resultados.map(item => {
//                    if (/\.(jpg|jpeg|png|gif|bmp|webp)$/i.test(item.nome)) {
//                        return `<br><img src="${item.url}" title="${item.nome}" style="max-width: 200px; cursor: pointer; border-radius: 6px;" onclick="abrirLightbox('${item.url}', '${item.nome}')">`;
//                    } else {
//                        return `<br><a href="${item.url}" target="_blank" title="${item.nome}"><i class="bi bi-paperclip"></i> ${item.nome}</a>`;
//                    }
//                }).join("");

//                const mensagemFinal = message ? `${message}${mensagensArquivos}` : mensagensArquivos;

//                connection.invoke("SendMessage", mensagemFinal, destinos);

//                arquivosSelecionados = [];
//                document.getElementById("fileInput").value = "";
//                document.getElementById("fileLabel").title = "Anexar arquivo";
//                messageInput.value = "";
//            })
//            .catch(err => {
//                console.error("Erro ao enviar arquivo:", err);
//                alert("Erro ao enviar arquivo.");
//            });
//    } else {
//        connection.invoke("SendMessage", message, destinos)
//            .then(() => messageInput.value = "")
//            .catch(err => {
//                console.error(err.toString());
//                alert("Erro ao enviar mensagem.");
//            });
//    }
//}

//// ==========================================================
//// 🎯 Utilitários e Efeitos
//// ==========================================================
//function scrollToLastMessage() {
//    const list = document.getElementById("messagesList");
//    const last = list.lastElementChild;

//    const previous = list.querySelector('[data-last]');
//    if (previous) previous.removeAttribute('data-last');

//    if (last) {
//        last.setAttribute('data-last', 'true');
//        const img = last.querySelector('img');
//        if (img) img.onload = () => last.scrollIntoView({ behavior: "smooth", block: "end" });
//        else last.scrollIntoView({ behavior: "smooth", block: "end" });
//    }
//}

//function vibrarTela() {
//    const el = document.querySelector('.parent');
//    if (!el) return;

//    el.classList.add('vibrando');
//    setTimeout(() => el.classList.remove('vibrando'), 500);

//    const som = document.getElementById('somAtencao');
//    if (som) som.play().catch(err => console.warn("Erro ao tocar som:", err));
//}

//// ==========================================================
//// 💡 Lightbox
//// ==========================================================
//function abrirLightbox(url, nome) {
//    const lightboxImage = document.getElementById("lightboxImage");
//    const lightboxLabel = document.getElementById("lightboxLabel");

//    lightboxImage.src = url;
//    lightboxLabel.textContent = nome;

//    const modal = new bootstrap.Modal(document.getElementById('lightboxModal'));
//    modal.show();
//}

//// ==========================================================
//// ⚙️ Controles de UI e Tema
//// ==========================================================
//document.addEventListener("DOMContentLoaded", () => {
//    if (localStorage.getItem("modoEscuro") === "true") {
//        document.body.classList.add("dark-mode");
//        document.getElementById("btnDark").innerText = "☀️ Claro";
//    }
//});

//function toggleDarkMode() {
//    const body = document.body;
//    const btn = document.getElementById("btnDark");

//    body.classList.toggle("dark-mode");
//    const escuro = body.classList.contains("dark-mode");
//    btn.innerText = escuro ? "☀️ Claro" : "🌙 Escuro";
//    localStorage.setItem("modoEscuro", escuro);
//}

//function toggleSom() {
//    notificacaoHabilitada = !notificacaoHabilitada;
//    document.getElementById("btnSom").innerText = notificacaoHabilitada ? "🔊 Som Ativo" : "🔇 Som Inativo";
//}

//// ==========================================================
//// 🤖 IA - Modo e Resumo
//// ==========================================================
//function consultarIaComando(comando) {
//    fetch("/api/ia/comando", {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify({ comando: `${comando} (responda em português)` })
//    })
//        .then(resp => resp.json())
//        .then(data => {
//            const respostaIa = data.resposta || "Não consegui entender sua solicitação.";
//            const li = document.createElement("li");
//            li.className = "list-group-item list-group-item-light";
//            const horaAtual = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
//            li.innerHTML = `<strong>ChatIA</strong> <small class="text-muted">[${horaAtual}]</small>: ${respostaIa}`;
//            document.getElementById("messagesList").appendChild(li);
//            scrollToLastMessage();
//        })
//        .catch(err => console.error("Erro IA:", err));
//}

//// 🔍 IA Seleção no Chat
//document.getElementById("messagesList").addEventListener("mouseup", function () {
//    if (!modoIaAtivo) return;
//    const selection = window.getSelection().toString().trim();
//    if (selection) {
//        if (confirm(`Deseja perguntar à IA sobre:\n\n"${selection}"`)) {
//            consultarIaSelecionado(selection);
//        }
//    }
//});

//function consultarIaSelecionado(texto) {
//    document.getElementById("iaResposta").innerHTML = "Processando...";

//    fetch("/api/ia/comando", {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify({ comando: texto + " (responda em português)" })
//    })
//        .then(resp => resp.json())
//        .then(data => {
//            document.getElementById("iaResposta").innerHTML = data.resposta || "Não consegui gerar uma resposta.";
//            const modal = new bootstrap.Modal(document.getElementById('iaModal'));
//            modal.show();
//        });
//}

//function copiarIa() {
//    const texto = document.getElementById("iaResposta").innerText;

//    if (navigator.clipboard) {
//        navigator.clipboard.writeText(texto)
//            .then(() => alert("Resposta copiada para a área de transferência!"))
//            .catch(() => fallbackCopyText(texto));
//    } else {
//        fallbackCopyText(texto);
//    }
//}

//function fallbackCopyText(text) {
//    const textarea = document.createElement("textarea");
//    textarea.value = text;
//    textarea.style.position = "fixed";
//    textarea.style.opacity = "0";
//    document.body.appendChild(textarea);
//    textarea.focus();
//    textarea.select();
//    try {
//        const sucesso = document.execCommand('copy');
//        if (sucesso) alert("Copiado!");
//        else alert("Não foi possível copiar.");
//    } catch (err) {
//        alert("Erro ao copiar.");
//    }
//    document.body.removeChild(textarea);
//}

//// ==========================================================
//// 📎 Upload de Arquivos
//// ==========================================================
//document.getElementById("fileInput").addEventListener("change", function () {
//    arquivosSelecionados = Array.from(this.files) || [];

//    const nomes = arquivosSelecionados.map(a => a.name).join(", ");
//    document.getElementById("fileLabel").title = arquivosSelecionados.length > 0 ? `📎 ${nomes}` : "Anexar arquivo";
//});

//// ==========================================================
//// 🚨 Chamar Atenção
//// ==========================================================
//document.getElementById("destinatarioInput").addEventListener("change", function () {
//    const btnAtencao = document.getElementById("btnAtencao");
//    btnAtencao.style.display = this.value !== "Todos" ? "inline-block" : "none";
//});

//function chamarAtencao() {
//    const destino = document.getElementById("destinatarioInput").value;
//    if (destino === "Todos") return;

//    const mensagem = `${currentUser} está chamando sua atenção!`;
//    connection.invoke("SendMessage", mensagem, [destino]);
//}
