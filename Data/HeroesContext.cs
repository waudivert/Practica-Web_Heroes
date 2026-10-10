using HeroesMvc.Data.Configurations;
using HeroesWeb.Data.Configurations;
using HeroesWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HeroesWeb.Data;

public partial class HeroesContext : DbContext
{
    public HeroesContext(DbContextOptions<HeroesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Heroes> Heroes { get; set; }

    public virtual DbSet<SuperPoderes> SuperPoderes { get; set; }

    public DbSet<Ciudad> Ciudades => Set<Ciudad>();

    public DbSet<Villano> Villanos => Set<Villano>();
    public DbSet<Cuartel> Cuarteles  => Set<Cuartel>();

    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Heroes>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Heroes__3214EC079409D8EF");
        });

        modelBuilder.Entity<SuperPoderes>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__SuperPod__3214EC07BDC114FB");

            entity.HasOne(d => d.Heroe)
                .WithMany(p => p.SuperPoderes)
                .HasConstraintName("FK_SuperPoderes_Heroes");

        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new CiudadConfiguration());

       modelBuilder.ApplyConfiguration(
            new VillanoConfiguration());

        modelBuilder.ApplyConfiguration(
            new CuartelConfiguration());

        modelBuilder.ApplyConfiguration(
           new VehiculoConfiguration());
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}