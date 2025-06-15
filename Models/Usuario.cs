using System.ComponentModel.DataAnnotations;

namespace SignalR.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O UIN é obrigatório.")]
        public long UIN { get; set; }

        [Required(ErrorMessage = "Selecione uma sala.")]
        public Guid SalaId { get; set; }

        public Sala? Sala { get; set; }

        public bool Adm { get; set; }

        public bool EstaOnline { get; set; }

        public DateTime DataEntrada { get; set; } = DateTime.Now;

        public DateTime? DataAlteracao { get; set; }
    }

}
