
using System.ComponentModel.DataAnnotations;

namespace SignalR.ViewModels
{
    public class AlterarSenhaViewModel
    {
        public Guid UsuarioId { get; set; }

        public string NomeUsuario { get; set; }

        [Required(ErrorMessage = "Informe a senha atual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Informe a nova senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        [Compare("NovaSenha", ErrorMessage = "A confirmação não corresponde à nova senha.")]
        public string ConfirmarNovaSenha { get; set; }
    }
}
