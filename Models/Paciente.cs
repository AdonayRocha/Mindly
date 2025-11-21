using System.ComponentModel.DataAnnotations;

namespace Mindly.Models
{
    /// <summary>
    /// Exemplo de requisição para criar um paciente:
    /// {
    ///   "nome": "Adonay",
    ///   "email": "adonay@example.com",
    ///   "senha": "123",
    ///   "telefone": "00000000000",
    ///   "observacao": "Transplantado renal"
    /// }
    /// </summary>
    public class Paciente
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(3, ErrorMessage = "A senha deve ter no mínimo 3 caracteres.")]
        public string Senha { get; set; }
        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "O telefone deve conter exatamente 11 dígitos numéricos.")]
        public string Telefone { get; set; }
        public string Observacao { get; set; }
    }
}