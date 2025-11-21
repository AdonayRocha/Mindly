using System;
using System.ComponentModel.DataAnnotations;
namespace Mindly.Models
{
    /// <summary>
    /// Exemplo de requisição para criar uma rotina:
    /// {
    ///   "pacienteId": 10,
    ///   "data": "2025-11-21T19:25:47.684Z",
    ///   "comeuBem": true,
    ///   "dormiuBem": true,
    ///   "desabafo": "Estou triste, pois, tive dificuldades ao fazer um trabalho da faculdade.",
    ///   "rotinaDia": "Acordei, trabalhei e estudei"
    /// }
    /// </summary>
    public class Rotina
    {
        public int Id { get; set; }
        [Required]
        public int PacienteId { get; set; }
        [Required]
        public DateTime Data { get; set; }
        public bool ComeuBem { get; set; }
        public bool DormiuBem { get; set; }
        public string Desabafo { get; set; }
        public string RotinaDia { get; set; }
        public Paciente? Paciente { get; set; }
    }
}