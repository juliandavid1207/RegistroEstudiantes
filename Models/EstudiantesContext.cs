using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RegistroEstudiantes.Models;

public partial class EstudiantesContext : DbContext
{
    public EstudiantesContext()
    {
    }

    public EstudiantesContext(DbContextOptions<EstudiantesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estudiante> Estudiante { get; set; }

    public virtual DbSet<Materia> Materia { get; set; }

    public virtual DbSet<Profesor> Profesor { get; set; }

    public virtual DbSet<Registro> Registro { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.IdEstudiante);

            entity.ToTable("Estudiante");

            entity.Property(e => e.IdEstudiante).HasColumnName("Id_Estudiante");
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.NumIdentificacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Num_Identificacion");
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => e.IdMateria);

            entity.Property(e => e.IdMateria).HasColumnName("Id_Materia");
            entity.Property(e => e.IdProfesor).HasColumnName("Id_Profesor");
            entity.Property(e => e.NombreMateria)
                .HasMaxLength(50)
                .HasColumnName("Nombre_Materia");
            entity.Property(e => e.NumCreditos).HasColumnName("Num_Creditos");
        });

        modelBuilder.Entity<Profesor>(entity =>
        {
            entity.HasKey(e => e.IdProfesor);

            entity.ToTable("Profesor");

            entity.Property(e => e.IdProfesor)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id_Profesor");
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.IdProfesorNavigation).WithOne(p => p.Profesor)
                .HasForeignKey<Profesor>(d => d.IdProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Profesor_Materia");
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.HasKey(e => e.IdRegistro);

            entity.ToTable("Registro");

            entity.Property(e => e.IdRegistro).HasColumnName("Id_Registro");
            entity.Property(e => e.IdEstudiante).HasColumnName("Id_Estudiante");
            entity.Property(e => e.IdMateria).HasColumnName("Id_Materia");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.Registros)
                .HasForeignKey(d => d.IdEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Estudiante");

            entity.HasOne(d => d.IdMateriaNavigation).WithMany(p => p.Registros)
                .HasForeignKey(d => d.IdMateria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Materia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
