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
    public DbSet<Profissional> Profissionais => Set<Profissional>();
    public DbSet<Proprietario> Proprietarios => Set<Proprietario>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<RelatoVisita> RelatosVisita => Set<RelatoVisita>();
    public DbSet<Anexo> Anexos => Set<Anexo>();
    public DbSet<TermoConclusao> TermosConclusao => Set<TermoConclusao>();
    public DbSet<Assinatura> Assinaturas => Set<Assinatura>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

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

        modelBuilder.Entity<Profissional>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                .HasColumnName("ProfissionalId");
            e.HasIndex(p => p.NumeroRegistro).IsUnique();
            e.Property(p => p.Nome).HasMaxLength(150).IsRequired();
            e.Property(p => p.Cpf).HasMaxLength(14).IsRequired();
            e.Property(p => p.NumeroRegistro).HasMaxLength(20).IsRequired();
            e.Property(p => p.TipoRegistro).HasMaxLength(10).IsRequired();
            e.Property(p => p.Especialidade).HasMaxLength(100).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200).IsRequired();
            e.Property(p => p.Telefone).HasMaxLength(20);
            e.HasOne(p => p.Usuario)
             .WithMany()
             .HasForeignKey(p => p.UsuarioId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Proprietario>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("ProprietarioId");
            e.Property(p => p.Nome).HasMaxLength(200).IsRequired();
            e.Property(p => p.Cpf).HasMaxLength(14);
            e.Property(p => p.Email).HasMaxLength(200);
            e.Property(p => p.Telefone).HasMaxLength(20);
            e.HasIndex(p => p.UsuarioId).IsUnique().HasFilter("[UsuarioId] IS NOT NULL");
            e.HasOne(p => p.Usuario)
             .WithOne(u => u.Proprietario)
             .HasForeignKey<Proprietario>(p => p.UsuarioId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Obra>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("ObraId");
            e.Property(o => o.LocalObra).HasMaxLength(300).IsRequired();
            e.Property(o => o.NumeroArt).HasMaxLength(50).IsRequired();
            e.HasOne(o => o.Proprietario)
             .WithMany(p => p.Obras)
             .HasForeignKey(o => o.ProprietarioId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Profissional)
             .WithMany(p => p.ObrasComoResponsavel)
             .HasForeignKey(o => o.ProfissionalId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.UsuarioCriador)
             .WithMany()
             .HasForeignKey(o => o.UsuarioCriadorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RelatoVisita>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("RelatoVisitaId");
            e.HasOne(r => r.Obra)
             .WithMany(o => o.RelatoVisita)
             .HasForeignKey(r => r.ObraId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Usuario)
             .WithMany(u => u.RelatoVisita)
             .HasForeignKey(r => r.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Anexo>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("AnexoId");
            e.Property(a => a.NomeArquivo).HasMaxLength(300).IsRequired();
            e.Property(a => a.TipoArquivo).HasMaxLength(100).IsRequired();
            e.HasOne(a => a.Obra)
             .WithMany(o => o.Anexos)
             .HasForeignKey(a => a.ObraId)
             .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.RelatoVisita)
             .WithMany(r => r.Anexos)
             .HasForeignKey(a => a.RelatoVisitaId)
             .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Usuario)
             .WithMany(u => u.Anexos)
             .HasForeignKey(a => a.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TermoConclusao>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("TermoConclusaoId");
            e.HasIndex(t => t.ObraId).IsUnique();
            e.HasOne(t => t.Obra)
             .WithOne(o => o.TermoConclusao)
             .HasForeignKey<TermoConclusao>(t => t.ObraId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Assinatura>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("AssinaturaId");
            e.Property(a => a.ImagemAssinatura).IsRequired();
            e.Property(a => a.HashAssinatura).HasMaxLength(64).IsRequired();
            e.Property(a => a.IpAssinante).HasMaxLength(45).IsRequired();
            e.Property(a => a.UserAgent).HasMaxLength(512);
            e.Property(a => a.Navegador).HasMaxLength(120);
            e.Property(a => a.SistemaOperacional).HasMaxLength(120);
            e.Property(a => a.Dispositivo).HasMaxLength(120);
            e.HasIndex(a => new { a.TipoEntidade, a.EntidadeId, a.TipoAssinante }).IsUnique();
            e.HasOne(a => a.Usuario)
             .WithMany(u => u.Assinaturas)
             .HasForeignKey(a => a.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LogAuditoria>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id)
                 .HasColumnName("LogAuditoriaId");
            e.Property(l => l.Acao).HasMaxLength(100).IsRequired();
            e.Property(l => l.Entidade).HasMaxLength(100).IsRequired();
            e.Property(l => l.NomeUsuario).HasMaxLength(150);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var logs = GerarLogsAuditoria();
        var result = await base.SaveChangesAsync(cancellationToken);
        if (logs.Count > 0)
        {
            LogsAuditoria.AddRange(logs);
            await base.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    private List<LogAuditoria> GerarLogsAuditoria()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var usuarioId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var nomeUsuario = httpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();

        var logs = new List<LogAuditoria>();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.Entity is LogAuditoria) continue;

            var acao = entry.State switch
            {
                EntityState.Added => "Criação",
                EntityState.Modified => entry.Entity.Ativo ? "Atualização" : "Exclusão",
                EntityState.Deleted => "Exclusão",
                _ => null
            };

            if (acao is null) continue;

            logs.Add(new LogAuditoria
            {
                UsuarioId = usuarioId is not null ? Guid.Parse(usuarioId) : null,
                NomeUsuario = nomeUsuario,
                Acao = acao,
                Entidade = entry.Entity.GetType().Name,
                EntidadeId = entry.Entity.Id.ToString(),
                DadosAntigos = acao != "Criação" ? SerializarValoresOriginais(entry) : null,
                DadosNovos = acao != "Exclusão" ? SerializarValoresAtuais(entry) : null,
                EnderecoIp = ip,
                DataAcao = DateTime.Now
            });
        }

        return logs;
    }

    private static string? SerializarValoresOriginais(EntityEntry<BaseEntity> entry)
    {
        var valores = entry.Properties
            .Where(p => p.IsModified)
            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
        return valores.Count > 0 ? JsonSerializer.Serialize(valores, _jsonOptions) : null;
    }

    private static string? SerializarValoresAtuais(EntityEntry<BaseEntity> entry)
    {
        var valores = entry.Properties
            .Where(p => p.IsModified || entry.State == EntityState.Added)
            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
        return valores.Count > 0 ? JsonSerializer.Serialize(valores, _jsonOptions) : null;
    }
}

