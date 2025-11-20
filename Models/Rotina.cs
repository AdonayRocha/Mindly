using System;
using System.ComponentModel.DataAnnotations;
namespace Mindly.Models
{
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
        public Paciente Paciente { get; set; }
    }
}