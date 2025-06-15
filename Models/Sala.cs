using System.ComponentModel.DataAnnotations;

namespace SignalR.Models
{
    public class Sala
    {
        public Guid Id { get; set; }

        [Required]
        public string Nome { get; set; }
    }
}
