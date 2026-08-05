using System.Security.Cryptography;
using System.Text;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CREA.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Usuarios.AnyAsync())
        {
            logger.LogInformation("Banco de dados já possui dados. Seed ignorado.");
            return;
        }

        logger.LogInformation("Iniciando seed do banco de dados...");

        // ----------------------------------------------------------------
        // USUÁRIOS
        // ----------------------------------------------------------------
        var usuarioAdmin = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Administrador CREA",
            Email = "admin@crea.com",
            SenhaHash = HashSenha("Admin@123"),
            TipoUsuario = TipoUsuario.Administrador,
            CriadoEm = DateTime.Now
        };

        var usuarioEngenheiro = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Carlos Engenheiro",
            Email = "carlos@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.ResponsavelTecnico,
            CriadoEm = DateTime.Now
        };

        var usuarioOperacional = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "João Operacional",
            Email = "joao@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.Operacional,
            CriadoEm = DateTime.Now
        };

        var usuarioArquiteta = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Ana Arquiteta",
            Email = "ana@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.ResponsavelTecnico,
            CriadoEm = DateTime.Now
        };

        var usuarioCrea = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Fiscal CREA",
            Email = "crea@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.UsuarioCrea,
            CriadoEm = DateTime.Now
        };

        var usuarioProprietarioTech = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Tech Solutions S.A.",
            Email = "proprietario.tech@empresa.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.Proprietario,
            CriadoEm = DateTime.Now
        };

        await context.Usuarios.AddRangeAsync(
            usuarioAdmin, usuarioEngenheiro, usuarioOperacional, usuarioArquiteta, usuarioCrea, usuarioProprietarioTech);
        await context.SaveChangesAsync();
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }
}
