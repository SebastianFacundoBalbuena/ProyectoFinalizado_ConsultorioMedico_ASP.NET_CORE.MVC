using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Models;

public partial class ConsultorioContext : DbContext
{
    public ConsultorioContext()
    {
    }

    public ConsultorioContext(DbContextOptions<ConsultorioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Especialidad> Especialidads { get; set; }

    public virtual DbSet<HistorialMedico> HistorialMedicos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Especialidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Especial__3214EC07CFAC5E21");

            entity.ToTable("Especialidad");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialMedico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC0750CCF7E2");

            entity.ToTable("HistorialMedico");

            entity.Property(e => e.Diagnostico)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Tratamiento)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.HistorialMedicos)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK__Historial__IdPac__52593CB8");

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.HistorialMedicos)
                .HasForeignKey(d => d.IdTurno)
                .HasConstraintName("FK__Historial__IdTur__5165187F");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicos__3214EC0760843DAA");

            entity.Property(e => e.Apellido)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEspecialidadNavigation).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.IdEspecialidad)
                .HasConstraintName("fk_IdEspecialidad");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Paciente__3214EC07DBBB7899");

            entity.Property(e => e.Apellido)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Dni).HasColumnName("DNI");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Sexo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TipoDeSangre)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Turnos__3214EC07B9546532");

            entity.Property(e => e.Motivo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.IdMedico)
                .HasConstraintName("fk_idMedico");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("fk_IdPaciente");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0702DB7A44");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImagenUrl)
                .IsUnicode(false)
                .HasColumnName("ImagenURL");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("fk_paciente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
