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
            Id =Guid.NewGuid(),
            Nome = "Administrador CREA",
            Email = "admin@crea.com",
            SenhaHash = HashSenha("Admin@123"),
            TipoUsuario = TipoUsuario.Administrador,
            CriadoEm = DateTime.UtcNow
        };

        var usuarioEngenheiro = new Usuario
        {
            Id =Guid.NewGuid(),
            Nome = "Carlos Engenheiro",
            Email = "carlos@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.ResponsavelTecnico,
            CriadoEm = DateTime.UtcNow
        };

        var usuarioOperacional = new Usuario
        {
            Id =Guid.NewGuid(),
            Nome = "João Operacional",
            Email = "joao@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.Operacional,
            CriadoEm = DateTime.UtcNow
        };

        var usuarioArquiteta = new Usuario
        {
            Id =Guid.NewGuid(),
            Nome = "Ana Arquiteta",
            Email = "ana@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.ResponsavelTecnico,
            CriadoEm = DateTime.UtcNow
        };

        var usuarioCrea = new Usuario
        {
            Id =Guid.NewGuid(),
            Nome = "Fiscal CREA",
            Email = "crea@crea.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.UsuarioCrea,
            CriadoEm = DateTime.UtcNow
        };

        var usuarioProprietarioTech = new Usuario
        {
            Id =Guid.NewGuid(),
            Nome = "Tech Solutions S.A.",
            Email = "proprietario.tech@empresa.com",
            SenhaHash = HashSenha("Crea@123"),
            TipoUsuario = TipoUsuario.Proprietario,
            CriadoEm = DateTime.UtcNow
        };

        await context.Usuarios.AddRangeAsync(
            usuarioAdmin, usuarioEngenheiro, usuarioOperacional, usuarioArquiteta, usuarioCrea, usuarioProprietarioTech);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // PROFISSIONAIS
        // ----------------------------------------------------------------
        var profissionalCarlos = new Profissional
        {
            Id =Guid.NewGuid(),
            Nome = "Carlos Engenheiro",
            Cpf = "123.456.789-00",
            NumeroRegistro = "CREA-SP 123456",
            TipoRegistro = "CREA",
            Empresa = "Engenharia Silva & Associados Ltda",
            Especialidade = "Engenharia Civil",
            Email = "carlos@crea.com",
            Telefone = "(11) 99999-0001",
            UsuarioId = usuarioEngenheiro.Id,
            CriadoEm = DateTime.UtcNow
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
            CriadoEm = DateTime.UtcNow
        };

        await context.Profissionais.AddRangeAsync(profissionalCarlos, profissionalAna);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // PROPRIETÁRIOS
        // ----------------------------------------------------------------
        var proprietarioRoberto = new Proprietario
        {
            Id =Guid.NewGuid(),
            Nome = "Roberto Silva",
            Cpf = string.Empty,
            Email = string.Empty,
            Telefone = "(11) 98765-4321",
            CriadoEm = DateTime.UtcNow
        };

        var proprietarioAlpha = new Proprietario
        {
            Id =Guid.NewGuid(),
            Nome = "Construtora Alpha Ltda",
            Cpf = string.Empty,
            Email = string.Empty,
            Telefone = "(11) 3333-4444",
            CriadoEm = DateTime.UtcNow
        };

        var proprietarioTech = new Proprietario
        {
            Id =Guid.NewGuid(),
            Nome = "Tech Solutions S.A.",
            Cpf = string.Empty,
            Email = "proprietario.tech@empresa.com",
            Telefone = "(11) 2222-5555",
            UsuarioId = usuarioProprietarioTech.Id,
            CriadoEm = DateTime.UtcNow
        };

        await context.Proprietarios.AddRangeAsync(proprietarioRoberto, proprietarioAlpha, proprietarioTech);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // OBRAS
        // ----------------------------------------------------------------
        var obraResidencial = new Obra
        {
            Id =Guid.NewGuid(),
            Nome = "Residência Família Silva",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01310-100",
            ProprietarioId = proprietarioRoberto.Id,
            Empresa = "Engenharia Silva & Associados Ltda",
            NumeroCaderneta = "12293",
            NumeroArt = "ART-2024-001234",
            NumeroRT = "RT-2024-001",
            TipoObra = TipoObra.Residencial,
            TipoEdificacao = TipoEdificacao.Residencial,
            AtividadeTecnica = AtividadeTecnica.Execucao,
            DirecaoTecnica = true,
            Status = StatusObra.EmAndamento,
            DataInicio = DateTime.UtcNow.AddMonths(-3),
            DataPrevisaoTermino = DateTime.UtcNow.AddMonths(6),
            Descricao = "Construção de residência unifamiliar com 180m², 3 quartos, 2 banheiros, sala, cozinha e garagem.",
            AreaConstruir = 180.00m,
            AreaTotalEdificada = 180.00m,
            ValorRecibo = 350000.00m,
            ProfissionalResponsavelId = profissionalCarlos.Id,
            UsuarioCriadorId = usuarioEngenheiro.Id,
            CriadoEm = DateTime.UtcNow
        };

        var obraComercial = new Obra
        {
            Id =Guid.NewGuid(),
            Nome = "Edifício Comercial Centro",
            Endereco = "Av. Paulista, 1000",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01310-200",
            ProprietarioId = proprietarioAlpha.Id,
            Empresa = "Engenharia Silva & Associados Ltda",
            NumeroCaderneta = "12294",
            NumeroArt = "ART-2024-005678",
            NumeroRT = "RT-2024-002",
            TipoObra = TipoObra.Comercial,
            TipoEdificacao = TipoEdificacao.Comercial,
            AtividadeTecnica = AtividadeTecnica.Execucao,
            DirecaoTecnica = true,
            Status = StatusObra.EmAndamento,
            DataInicio = DateTime.UtcNow.AddMonths(-6),
            DataPrevisaoTermino = DateTime.UtcNow.AddMonths(18),
            Descricao = "Construção de edifício comercial com 8 andares, 1.200m² por pavimento.",
            AreaConstruir = 9600.00m,
            AreaTotalEdificada = 9600.00m,
            ValorRecibo = 12000000.00m,
            ProfissionalResponsavelId = profissionalCarlos.Id,
            UsuarioCriadorId = usuarioEngenheiro.Id,
            CriadoEm = DateTime.UtcNow
        };

        var obraConcluida = new Obra
        {
            Id =Guid.NewGuid(),
            Nome = "Reforma Escritório Jardins",
            Endereco = "Rua Haddock Lobo, 500",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01414-001",
            ProprietarioId = proprietarioTech.Id,
            Empresa = "Arquitetura Moderna S.A.",
            NumeroCaderneta = "12295",
            NumeroArt = "ART-2023-009999",
            NumeroRT = "RT-2023-015",
            TipoObra = TipoObra.Comercial,
            TipoEdificacao = TipoEdificacao.Comercial,
            AtividadeTecnica = AtividadeTecnica.Fiscalizacao,
            DirecaoTecnica = false,
            Status = StatusObra.Concluida,
            DataInicio = DateTime.UtcNow.AddMonths(-12),
            DataPrevisaoTermino = DateTime.UtcNow.AddMonths(-2),
            Descricao = "Reforma completa de escritório com 350m², incluindo instalações elétricas e hidráulicas.",
            AreaReformar = 350.00m,
            AreaTotalEdificada = 350.00m,
            ValorRecibo = 180000.00m,
            ProfissionalResponsavelId = profissionalAna.Id,
            UsuarioCriadorId = usuarioArquiteta.Id,
            CriadoEm = DateTime.UtcNow.AddMonths(-12)
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
                Data = DateTime.UtcNow.AddDays(-10),
                Atividades = "Concretagem do radier da fundação. Foram utilizados 12m³ de concreto fck25. Nivelamento e acabamento da superfície.",
                EquipePresente = "3 pedreiros, 2 serventes, 1 mestre de obras",
                CondicaoClimatica = "Ensolarado, 28°C",
                Observacoes = "Serviço concluído conforme projeto. Aguardar cura de 28 dias antes de iniciar alvenaria.",
                ServicosPreliminar = true,
                Fundacao = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Aguardar cura do concreto por 28 dias antes de iniciar alvenaria.",
                UsuarioId = usuarioOperacional.Id,
                CriadoEm = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                ObraId = obraResidencial.Id,
                NumeroSequencial = 2,
                Data = DateTime.UtcNow.AddDays(-7),
                Atividades = "Início da alvenaria do pavimento térreo. Levantamento dos blocos da sala e cozinha. Assentamento de 320 blocos cerâmicos.",
                EquipePresente = "4 pedreiros, 3 serventes, 1 mestre de obras",
                CondicaoClimatica = "Parcialmente nublado, 25°C",
                Observacoes = "Verificar alinhamento das paredes antes de continuar amanhã.",
                Alvenarias = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Conferir prumo e nível das paredes no próximo dia.",
                UsuarioId = usuarioOperacional.Id,
                CriadoEm = DateTime.UtcNow.AddDays(-7)
            },
            new()
            {
                ObraId = obraResidencial.Id,
                NumeroSequencial = 3,
                Data = DateTime.UtcNow.AddDays(-3),
                Atividades = "Continuação da alvenaria. Levantamento das paredes dos quartos. Passagem de eletrodutos conforme projeto elétrico.",
                EquipePresente = "4 pedreiros, 3 serventes, 1 eletricista, 1 mestre de obras",
                CondicaoClimatica = "Ensolarado, 30°C",
                Observacoes = "Eletricista confirmou passagem conforme projeto aprovado.",
                Alvenarias = true,
                EsquadriasInstalacoesEletricasHidraulicas = true,
                PosicaoObra = PosicaoObra.EmAndamento,
                DecisoesTecnicas = "Prosseguir com instalações elétricas conforme projeto aprovado.",
                UsuarioId = usuarioOperacional.Id,
                CriadoEm = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                ObraId = obraComercial.Id,
                NumeroSequencial = 1,
                Data = DateTime.UtcNow.AddDays(-5),
                Atividades = "Concretagem das lajes do 3º pavimento. Utilização de 45m³ de concreto bombeado fck30. Instalação de forma e escoramento.",
                EquipePresente = "6 pedreiros, 4 serventes, 2 armadores, 1 encarregado",
                CondicaoClimatica = "Ensolarado, 27°C",
                Observacoes = "Concretagem realizada sem intercorrências. Cura iniciada às 16h.",
                Superestrutura = true,
                PosicaoObra = PosicaoObra.DeAcordoComProjeto,
                DecisoesTecnicas = "Manter escoramento por no mínimo 21 dias. Iniciar cura úmida.",
                UsuarioId = usuarioOperacional.Id,
                CriadoEm = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                ObraId = obraComercial.Id,
                NumeroSequencial = 2,
                Data = DateTime.UtcNow.AddDays(-2),
                Atividades = "Instalação de pilares metálicos do 4º andar. Soldagem e grauteamento. Inspeção de qualidade realizada.",
                EquipePresente = "3 soldadores, 2 ajudantes, 1 engenheiro fiscal",
                CondicaoClimatica = "Nublado, 22°C",
                Observacoes = "Ensaio de solda aprovado pelo engenheiro fiscal.",
                Superestrutura = true,
                ServicosComplementares = true,
                PosicaoObra = PosicaoObra.EmAndamento,
                DecisoesTecnicas = "Solicitar laudo de ensaio de solda ao laboratório.",
                UsuarioId = usuarioOperacional.Id,
                CriadoEm = DateTime.UtcNow.AddDays(-2)
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
            DataConclusao = DateTime.UtcNow.AddMonths(-2),
            Descricao = "Reforma concluída integralmente conforme projeto aprovado. Todas as etapas foram executadas dentro dos padrões técnicos exigidos.",
            Observacoes = "Cliente satisfeito com o resultado. Documentação entregue ao proprietário. Habite-se emitido pela prefeitura.",
            Empresa = "Arquitetura Moderna S.A.",
            Proprietario = "Tech Solutions S.A.",
            TelefoneProprietario = "(11) 2222-5555",
            LocalObra = "Rua Haddock Lobo, 500 - São Paulo/SP",
            DeclaracaoTexto = "Nós, abaixo assinados, proprietário e profissional responsável, pela execução da obra acima apontada declaramos que temos ainda conhecimento na íntegra das sanções prescritas nas Legislações Federal, Estadual e Municipal vigentes.",
            LocalDeclaracao = "São Paulo",
            DataDeclaracao = DateTime.UtcNow.AddMonths(-2),
            ProfissionalId = profissionalAna.Id,
            CriadoEm = DateTime.UtcNow.AddMonths(-2)
        };

        await context.TermosConclusao.AddAsync(termoConclusao);
        await context.SaveChangesAsync();

        var dataAssinaturaObra = DateTime.UtcNow.AddMonths(-1);
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
