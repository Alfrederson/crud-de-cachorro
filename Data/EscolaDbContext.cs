using Microsoft.EntityFrameworkCore;
using escola_dos_catioros.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace escola_dos_catioros.Data;

public class EscolaDbContext(DbContextOptions<EscolaDbContext> options) : DbContext(options)
{
    public DbSet<Catioro> Catioros { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catioro>()
            .HasIndex(c => c.Nome)
            .IsUnique();

        // Turma -> Matricula
        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Turma)
            .WithMany(t => t.Matriculas)
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Catioro -> Matricula
        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Catioro)
            .WithMany()
            .HasForeignKey(m => m.CatioroId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Matricula>()
            .HasIndex(m => new { m.TurmaId, m.CatioroId })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}