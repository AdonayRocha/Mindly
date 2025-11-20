using Microsoft.EntityFrameworkCore;
using Mindly.Models;

namespace Mindly.Data
{
    public class MindlyContext : DbContext
    {
        public MindlyContext(DbContextOptions<MindlyContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; } = null!;
        public DbSet<Psicologo> Psicologos { get; set; } = null!;
        public DbSet<Rotina> Rotinas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}