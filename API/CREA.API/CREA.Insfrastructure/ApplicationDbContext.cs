using System.Security.Claims;
using System.Text.Json;
using CREA.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CREA.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                .HasColumnName("UsuarioId");
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Nome).HasMaxLength(150).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.SenhaHash).IsRequired();
        });

      
    }
}

