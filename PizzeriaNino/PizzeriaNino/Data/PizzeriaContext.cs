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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=Pizzeria;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
