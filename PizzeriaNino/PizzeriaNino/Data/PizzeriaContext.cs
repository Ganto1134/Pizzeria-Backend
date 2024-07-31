using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PizzeriaNino.Models;

namespace PizzeriaNino.Data;

public partial class PizzeriaContext : DbContext
{
    public PizzeriaContext()
    {
    }

    public PizzeriaContext(DbContextOptions<PizzeriaContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Pizza> Pizze { get; set; }
    public DbSet<Ingrediente> Ingredienti { get; set; }
    public DbSet<PizzaIngrediente> PizzaIngrediente { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=Pizzeria;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PizzaIngrediente>()
            .HasKey(pi => new { pi.PizzaId, pi.IngredienteId });

        modelBuilder.Entity<PizzaIngrediente>()
            .HasOne(pi => pi.Pizza)
            .WithMany(p => p.PizzaIngredienti)
            .HasForeignKey(pi => pi.PizzaId);

        modelBuilder.Entity<PizzaIngrediente>()
            .HasOne(pi => pi.Ingrediente)
            .WithMany(i => i.PizzaIngredienti)
            .HasForeignKey(pi => pi.IngredienteId);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
