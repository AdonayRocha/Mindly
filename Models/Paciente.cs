using System.ComponentModel.DataAnnotations;

namespace Mindly.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string Observacao { get; set; }
    }
}