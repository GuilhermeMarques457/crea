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
            usuarioAdmin, usuarioEngenheiro, usuarioArquiteta, usuarioCrea, usuarioProprietarioTech);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // PROFISSIONAIS
        // ----------------------------------------------------------------
        var profissionalCarlos = new Profissional
        {
            Id = Guid.NewGuid(),
            Nome = "Carlos Engenheiro",
            Cpf = "123.456.789-00",
            NumeroRegistro = "CREA-SP 123456",
            TipoRegistro = "CREA",
            Empresa = "Engenharia Silva & Associados Ltda",
            Especialidade = "Engenharia Civil",
            Email = "carlos@crea.com",
            Telefone = "(11) 99999-0001",
            UsuarioId = usuarioEngenheiro.Id,
            CriadoEm = DateTime.Now
        };

        var profissionalAna = new Profissional
        {
            Id = Guid.NewGuid(),
            Nome = "Ana Arquiteta",
            Cpf = "987.654.321-00",
            NumeroRegistro = "CAU-SP 654321",
            TipoRegistro = "CAU",
            Empresa = "Arquitetura Moderna S.A.",
            Especialidade = "Arquitetura e Urbanismo",
            Email = "ana@crea.com",
            Telefone = "(11) 99999-0002",
            UsuarioId = usuarioArquiteta.Id,
            CriadoEm = DateTime.Now
        };

        await context.Profissionais.AddRangeAsync(profissionalCarlos, profissionalAna);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // PROPRIETÁRIOS
        // ----------------------------------------------------------------
        var proprietarioRoberto = new Proprietario
        {
            Id = Guid.NewGuid(),
            Nome = "Roberto Silva",
            Cpf = string.Empty,
            Email = string.Empty,
            Telefone = "(11) 98765-4321",
            CriadoEm = DateTime.Now
        };

        var proprietarioAlpha = new Proprietario
        {
            Id = Guid.NewGuid(),
            Nome = "Construtora Alpha Ltda",
            Cpf = string.Empty,
            Email = string.Empty,
            Telefone = "(11) 3333-4444",
            CriadoEm = DateTime.Now
        };

        var proprietarioTech = new Proprietario
        {
            Id = Guid.NewGuid(),
            Nome = "Tech Solutions S.A.",
            Cpf = string.Empty,
            Email = "proprietario.tech@empresa.com",
            Telefone = "(11) 2222-5555",
            UsuarioId = usuarioProprietarioTech.Id,
            CriadoEm = DateTime.Now
        };

        await context.Proprietarios.AddRangeAsync(proprietarioRoberto, proprietarioAlpha, proprietarioTech);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // OBRAS
        // ----------------------------------------------------------------
        var obraResidencial = new Obra
        {
            Id = Guid.NewGuid(),
            LocalObra = "Rua das Flores, 123",
            ProprietarioId = proprietarioRoberto.Id,
            Empresa = "Engenharia Silva & Associados Ltda",
            ProfissionalId = profissionalAna.Id,
            NumeroCaderneta = "12293",
            NumeroRT = "RT-2024-001",
            TipoEdificacao = TipoEdificacao.Residencial,
            AtividadeTecnica = AtividadeTecnica.Execucao,
            Status = StatusObra.EmAndamento,
            DataInicio = DateTime.Now.AddMonths(-3),
            AreaConstruir = 180.00m,
            AreaTotalEdificada = 180.00m,
            ValorRecibo = 350000.00m,
            UsuarioCriadorId = usuarioAdmin.Id,
            CriadoEm = DateTime.Now
        };

        var obraComercial = new Obra
        {
            Id = Guid.NewGuid(),
            LocalObra = "Av. Paulista, 1000",
            ProprietarioId = proprietarioAlpha.Id,
            Empresa = "Engenharia Silva & Associados Ltda",
            NumeroCaderneta = "12294",
            NumeroRT = "RT-2024-002",
            TipoEdificacao = TipoEdificacao.Comercial,
            AtividadeTecnica = AtividadeTecnica.Execucao,
            Status = StatusObra.EmAndamento,
            DataInicio = DateTime.Now.AddMonths(-6),
            AreaConstruir = 9600.00m,
            AreaTotalEdificada = 9600.00m,
            ValorRecibo = 12000000.00m,
            ProfissionalId = profissionalCarlos.Id,
            UsuarioCriadorId = usuarioAdmin.Id,
            CriadoEm = DateTime.Now
        };

        var obraConcluida = new Obra
        {
            Id = Guid.NewGuid(),
            LocalObra = "Rua Haddock Lobo, 500",
            ProprietarioId = proprietarioTech.Id,
            Empresa = "Arquitetura Moderna S.A.",
            NumeroCaderneta = "12295",
            NumeroRT = "RT-2023-015",
            TipoEdificacao = TipoEdificacao.Comercial,
            AtividadeTecnica = AtividadeTecnica.Fiscalizacao,
            Status = StatusObra.Concluida,
            DataInicio = DateTime.Now.AddMonths(-12),
            AreaReformar = 350.00m,
            AreaTotalEdificada = 350.00m,
            ValorRecibo = 180000.00m,
            ProfissionalId = profissionalAna.Id,
            UsuarioCriadorId = usuarioAdmin.Id,
            CriadoEm = DateTime.Now.AddMonths(-12)
        };

        await context.Obras.AddRangeAsync(obraResidencial, obraComercial, obraConcluida);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // REGISTROS DIÁRIOS
        // ----------------------------------------------------------------
        var registros = new List<RelatoVisita>
        {
            new()
            {
                ObraId = obraResidencial.Id,
                NumeroSequencial = 1,
                Data = DateTime.Now.AddDays(-10),
                ServicosPreliminar = true,
                Fundacao = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Aguardar cura do concreto por 28 dias antes de iniciar alvenaria.",
                UsuarioId = usuarioAdmin.Id,
                CriadoEm = DateTime.Now.AddDays(-10)
            },
            new()
            {
                ObraId = obraResidencial.Id,
                NumeroSequencial = 2,
                Data = DateTime.Now.AddDays(-7),
                Alvenarias = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Conferir prumo e nível das paredes no próximo dia.",
                UsuarioId = usuarioAdmin.Id,
                CriadoEm = DateTime.Now.AddDays(-7)
            },
            new()
            {
                ObraId = obraResidencial.Id,
                NumeroSequencial = 3,
                Data = DateTime.Now.AddDays(-3),
                Alvenarias = true,
                EsquadriasInstalacoesEletricasHidraulicas = true,
                PosicaoObra = PosicaoObra.EmAndamento,
                DecisoesTecnicas = "Prosseguir com instalações elétricas conforme projeto aprovado.",
                UsuarioId = usuarioAdmin.Id,
                CriadoEm = DateTime.Now.AddDays(-3)
            },
            new()
            {
                ObraId = obraComercial.Id,
                NumeroSequencial = 1,
                Data = DateTime.Now.AddDays(-5),
                Superestrutura = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Manter escoramento por no mínimo 21 dias. Iniciar cura úmida.",
                UsuarioId = usuarioAdmin.Id,
                CriadoEm = DateTime.Now.AddDays(-5)
            },
            new()
            {
                ObraId = obraComercial.Id,
                NumeroSequencial = 2,
                Data = DateTime.Now.AddDays(-2),
                Superestrutura = true,
                ServicosComplementares = true,
                PosicaoObra = PosicaoObra.EmAndamento,
                DecisoesTecnicas = "Solicitar laudo de ensaio de solda ao laboratório.",
                UsuarioId = usuarioAdmin.Id,
                CriadoEm = DateTime.Now.AddDays(-2)
            }
        };

        await context.RelatosVisita.AddRangeAsync(registros);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // TERMO DE CONCLUSÃO (obra concluída)
        // ----------------------------------------------------------------
        var termoConclusao = new TermoConclusao
        {
            ObraId = obraConcluida.Id,
            NumeroTermo = 20,
            DataConclusao = DateTime.Now.AddMonths(-2),
            DeclaracaoTexto = "Nós, abaixo assinados, proprietário e profissional responsável, pela execução da obra acima apontada declaramos que temos ainda conhecimento na íntegra das sanções prescritas nas Legislações Federal, Estadual e Municipal vigentes.",
            LocalDeclaracao = "São Paulo",
            DataDeclaracao = DateTime.Now.AddMonths(-2),
            CriadoEm = DateTime.Now.AddMonths(-2)
        };

        await context.TermosConclusao.AddAsync(termoConclusao);
        await context.SaveChangesAsync();

        var dataAssinaturaObra = DateTime.Now.AddMonths(-1);
        await context.Assinaturas.AddRangeAsync(
            new Assinatura
            {
                TipoEntidade = TipoEntidadeAssinatura.Obra,
                EntidadeId = obraConcluida.Id,
                TipoAssinante = TipoAssinante.Profissional,
                UsuarioId = usuarioArquiteta.Id,
                HashAssinatura = GerarHashAssinatura(TipoEntidadeAssinatura.Obra, obraConcluida.Id, usuarioArquiteta.Id, TipoAssinante.Profissional, dataAssinaturaObra),
                DataAssinatura = dataAssinaturaObra,
                ImagemAssinatura = "iVBORw0KGgo=",
                IpAssinante = "127.0.0.1",
                UserAgent = "Seed",
                Navegador = "Seed",
                CriadoEm = dataAssinaturaObra
            },
            new Assinatura
            {
                TipoEntidade = TipoEntidadeAssinatura.Obra,
                EntidadeId = obraConcluida.Id,
                TipoAssinante = TipoAssinante.UsuarioCrea,
                UsuarioId = usuarioCrea.Id,
                HashAssinatura = GerarHashAssinatura(TipoEntidadeAssinatura.Obra, obraConcluida.Id, usuarioCrea.Id, TipoAssinante.UsuarioCrea, dataAssinaturaObra),
                DataAssinatura = dataAssinaturaObra,
                ImagemAssinatura = "iVBORw0KGgo=",
                IpAssinante = "127.0.0.1",
                UserAgent = "Seed",
                Navegador = "Seed",
                CriadoEm = dataAssinaturaObra
            },
            new Assinatura
            {
                TipoEntidade = TipoEntidadeAssinatura.TermoConclusao,
                EntidadeId = termoConclusao.Id,
                TipoAssinante = TipoAssinante.Profissional,
                UsuarioId = usuarioArquiteta.Id,
                HashAssinatura = GerarHashAssinatura(TipoEntidadeAssinatura.TermoConclusao, termoConclusao.Id, usuarioArquiteta.Id, TipoAssinante.Profissional, dataAssinaturaObra),
                DataAssinatura = dataAssinaturaObra,
                ImagemAssinatura = "iVBORw0KGgo=",
                IpAssinante = "127.0.0.1",
                UserAgent = "Seed",
                CriadoEm = dataAssinaturaObra
            },
            new Assinatura
            {
                TipoEntidade = TipoEntidadeAssinatura.TermoConclusao,
                EntidadeId = termoConclusao.Id,
                TipoAssinante = TipoAssinante.Proprietario,
                UsuarioId = usuarioProprietarioTech.Id,
                HashAssinatura = GerarHashAssinatura(TipoEntidadeAssinatura.TermoConclusao, termoConclusao.Id, usuarioProprietarioTech.Id, TipoAssinante.Proprietario, dataAssinaturaObra),
                DataAssinatura = dataAssinaturaObra,
                ImagemAssinatura = "iVBORw0KGgo=",
                IpAssinante = "127.0.0.1",
                UserAgent = "Seed",
                CriadoEm = dataAssinaturaObra
            });
        await context.SaveChangesAsync();

        logger.LogInformation("Seed concluído com sucesso!");
        logger.LogInformation("=== CREDENCIAIS DE TESTE ===");
        logger.LogInformation("Admin:          admin@crea.com  / Admin@123");
        logger.LogInformation("Resp. Técnico:  carlos@crea.com / Crea@123");
        logger.LogInformation("Resp. Técnico:  ana@crea.com    / Crea@123");
        logger.LogInformation("Operacional:    joao@crea.com   / Crea@123");
        logger.LogInformation("Usuário CREA:   crea@crea.com   / Crea@123");
        logger.LogInformation("Proprietário:   proprietario.tech@empresa.com / Crea@123");
        logger.LogInformation("============================");
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }

    private static string GerarHashAssinatura(
        TipoEntidadeAssinatura tipoEntidade,
        Guid entidadeId,
        Guid usuarioId,
        TipoAssinante tipoAssinante,
        DateTime data)
    {
        var conteudo = $"ASSINATURA:{tipoEntidade}:{entidadeId}:{usuarioId}:{tipoAssinante}:{data:O}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(conteudo));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
