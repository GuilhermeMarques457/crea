using CREA.Application.DTOs.Assinaturas;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class AssinaturaRepository(ApplicationDbContext context)
    : Repository<Assinatura>(context), IAssinaturaRepository
{
    public async Task<IEnumerable<Assinatura>> GetByEntidadeAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId) =>
        await _dbSet
            .Include(a => a.Usuario)
            .Where(a => a.Ativo && a.TipoEntidade == tipoEntidade && a.EntidadeId == entidadeId)
            .OrderBy(a => a.DataAssinatura)
            .ToListAsync();

    public async Task<Assinatura?> GetByEntidadeETipoAssinanteAsync(
        TipoEntidadeAssinatura tipoEntidade, Guid entidadeId, TipoAssinante tipoAssinante) =>
        await _dbSet
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a =>
                a.Ativo
                && a.TipoEntidade == tipoEntidade
                && a.EntidadeId == entidadeId
                && a.TipoAssinante == tipoAssinante);

    public async Task<bool> ExisteAssinaturaAsync(
        TipoEntidadeAssinatura tipoEntidade, Guid entidadeId, TipoAssinante tipoAssinante) =>
        await _dbSet.AnyAsync(a =>
            a.Ativo
            && a.TipoEntidade == tipoEntidade
            && a.EntidadeId == entidadeId
            && a.TipoAssinante == tipoAssinante);

    public async Task<IEnumerable<PendenteAssinaturaDto>> GetPendentesParaUsuarioAsync(Guid usuarioId, TipoUsuario tipoUsuario)
    {
        var assinaturas = context.Assinaturas.Where(a => a.Ativo);
        var pendentes = new List<PendenteAssinaturaDto>();

        if (tipoUsuario == TipoUsuario.ResponsavelTecnico)
        {
            var obrasProfissional = await context.Obras
                .Include(o => o.ProfissionalResponsavel)
                .Where(o => o.Ativo && o.ProfissionalResponsavel.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var obra in obrasProfissional)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.Obra
                        && a.EntidadeId == obra.Id
                        && a.TipoAssinante == TipoAssinante.Profissional))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.Obra,
                        EntidadeId = obra.Id,
                        ObraId = obra.Id,
                        TipoAssinante = TipoAssinante.Profissional,
                        Titulo = obra.Nome,
                        Subtitulo = "Assinatura do profissional responsável",
                        CriadoEm = obra.CriadoEm
                    });
                }
            }

            var relatos = await context.RelatosVisita
                .Include(r => r.Obra).ThenInclude(o => o.ProfissionalResponsavel)
                .Where(r => r.Ativo && r.Obra.ProfissionalResponsavel.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var relato in relatos)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.RelatoVisita
                        && a.EntidadeId == relato.Id
                        && a.TipoAssinante == TipoAssinante.Profissional))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.RelatoVisita,
                        EntidadeId = relato.Id,
                        ObraId = relato.ObraId,
                        TipoAssinante = TipoAssinante.Profissional,
                        Titulo = relato.Obra.Nome,
                        Subtitulo = $"Relato de visita #{relato.NumeroSequencial}",
                        CriadoEm = relato.CriadoEm
                    });
                }
            }

            var termos = await context.TermosConclusao
                .Include(t => t.Obra)
                .Include(t => t.Profissional)
                .Where(t => t.Ativo && t.Profissional.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var termo in termos)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.TermoConclusao
                        && a.EntidadeId == termo.Id
                        && a.TipoAssinante == TipoAssinante.Profissional))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.TermoConclusao,
                        EntidadeId = termo.Id,
                        ObraId = termo.ObraId,
                        TipoAssinante = TipoAssinante.Profissional,
                        Titulo = termo.Obra.Nome,
                        Subtitulo = $"Termo de conclusão nº {termo.NumeroTermo}",
                        CriadoEm = termo.CriadoEm
                    });
                }
            }
        }

        if (tipoUsuario == TipoUsuario.UsuarioCrea)
        {
            var obras = await context.Obras.Where(o => o.Ativo).ToListAsync();
            foreach (var obra in obras)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.Obra
                        && a.EntidadeId == obra.Id
                        && a.TipoAssinante == TipoAssinante.UsuarioCrea))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.Obra,
                        EntidadeId = obra.Id,
                        ObraId = obra.Id,
                        TipoAssinante = TipoAssinante.UsuarioCrea,
                        Titulo = obra.Nome,
                        Subtitulo = "Assinatura CREA",
                        CriadoEm = obra.CriadoEm
                    });
                }
            }
        }

        if (tipoUsuario == TipoUsuario.Proprietario)
        {
            var proprietario = await context.Proprietarios
                .FirstOrDefaultAsync(p => p.Ativo && p.UsuarioId == usuarioId);
            if (proprietario is null) return pendentes;

            var relatosProprietario = await context.RelatosVisita
                .Include(r => r.Obra)
                .Where(r => r.Ativo && r.Obra.ProprietarioId == proprietario.Id)
                .ToListAsync();

            foreach (var relato in relatosProprietario)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.RelatoVisita
                        && a.EntidadeId == relato.Id
                        && a.TipoAssinante == TipoAssinante.Proprietario))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.RelatoVisita,
                        EntidadeId = relato.Id,
                        ObraId = relato.ObraId,
                        TipoAssinante = TipoAssinante.Proprietario,
                        Titulo = relato.Obra.Nome,
                        Subtitulo = $"Relato de visita #{relato.NumeroSequencial}",
                        CriadoEm = relato.CriadoEm
                    });
                }
            }

            var termosProprietario = await context.TermosConclusao
                .Include(t => t.Obra)
                .Where(t => t.Ativo && t.Obra.ProprietarioId == proprietario.Id)
                .ToListAsync();

            foreach (var termo in termosProprietario)
            {
                if (!await assinaturas.AnyAsync(a =>
                        a.TipoEntidade == TipoEntidadeAssinatura.TermoConclusao
                        && a.EntidadeId == termo.Id
                        && a.TipoAssinante == TipoAssinante.Proprietario))
                {
                    pendentes.Add(new PendenteAssinaturaDto
                    {
                        TipoEntidade = TipoEntidadeAssinatura.TermoConclusao,
                        EntidadeId = termo.Id,
                        ObraId = termo.ObraId,
                        TipoAssinante = TipoAssinante.Proprietario,
                        Titulo = termo.Obra.Nome,
                        Subtitulo = $"Termo de conclusão nº {termo.NumeroTermo}",
                        CriadoEm = termo.CriadoEm
                    });
                }
            }
        }

        return pendentes.OrderByDescending(p => p.CriadoEm);
    }

    private static TipoAssinante[] AssinantesRequeridos(TipoEntidadeAssinatura tipo) => tipo switch
    {
        TipoEntidadeAssinatura.Obra => [TipoAssinante.Profissional, TipoAssinante.UsuarioCrea],
        _ => [TipoAssinante.Profissional, TipoAssinante.Proprietario],
    };

    public async Task<IEnumerable<MinhaAssinaturaDto>> GetMinhasAsync(Guid usuarioId)
    {
        var assinaturas = await _dbSet
            .Where(a => a.Ativo && a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.DataAssinatura)
            .ToListAsync();

        // Batch-load all assinaturas for those same entities (by any user) to detect full completion
        var entityIds = assinaturas.Select(a => a.EntidadeId).Distinct().ToList();
        var todasAssinaturas = entityIds.Count > 0
            ? await _dbSet
                .Where(a => a.Ativo && entityIds.Contains(a.EntidadeId))
                .Select(a => new { a.TipoEntidade, a.EntidadeId, a.TipoAssinante })
                .ToListAsync()
            : [];
        var sigsByEntity = todasAssinaturas
            .GroupBy(a => (a.TipoEntidade, a.EntidadeId))
            .ToDictionary(g => g.Key, g => g.Select(x => x.TipoAssinante).ToHashSet());

        var obraIds = assinaturas.Where(a => a.TipoEntidade == TipoEntidadeAssinatura.Obra)
            .Select(a => a.EntidadeId).ToList();
        var relatoIds = assinaturas.Where(a => a.TipoEntidade == TipoEntidadeAssinatura.RelatoVisita)
            .Select(a => a.EntidadeId).ToList();
        var termoIds = assinaturas.Where(a => a.TipoEntidade == TipoEntidadeAssinatura.TermoConclusao)
            .Select(a => a.EntidadeId).ToList();

        var obras = obraIds.Count > 0
            ? await context.Obras.Where(o => obraIds.Contains(o.Id)).ToDictionaryAsync(o => o.Id)
            : new Dictionary<Guid, CREA.Domain.Entities.Obra>();
        var relatos = relatoIds.Count > 0
            ? await context.RelatosVisita.Include(r => r.Obra)
                .Where(r => relatoIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id)
            : new Dictionary<Guid, CREA.Domain.Entities.RelatoVisita>();
        var termos = termoIds.Count > 0
            ? await context.TermosConclusao.Include(t => t.Obra)
                .Where(t => termoIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id)
            : new Dictionary<Guid, CREA.Domain.Entities.TermoConclusao>();

        var result = new List<MinhaAssinaturaDto>();
        foreach (var a in assinaturas)
        {
            string titulo = string.Empty;
            string? subtitulo = null;
            Guid obraId = Guid.Empty;

            if (a.TipoEntidade == TipoEntidadeAssinatura.Obra && obras.TryGetValue(a.EntidadeId, out var obra))
            {
                titulo = obra.Nome;
                obraId = obra.Id;
            }
            else if (a.TipoEntidade == TipoEntidadeAssinatura.RelatoVisita && relatos.TryGetValue(a.EntidadeId, out var relato))
            {
                titulo = relato.Obra.Nome;
                subtitulo = $"Relato de visita #{relato.NumeroSequencial}";
                obraId = relato.ObraId;
            }
            else if (a.TipoEntidade == TipoEntidadeAssinatura.TermoConclusao && termos.TryGetValue(a.EntidadeId, out var termo))
            {
                titulo = termo.Obra.Nome;
                subtitulo = $"Termo de conclusão nº {termo.NumeroTermo}";
                obraId = termo.ObraId;
            }

            if (obraId == Guid.Empty) continue;

            var presentes = sigsByEntity.GetValueOrDefault((a.TipoEntidade, a.EntidadeId)) ?? [];
            var totalmenteAssinado = AssinantesRequeridos(a.TipoEntidade).All(s => presentes.Contains(s));

            result.Add(new MinhaAssinaturaDto
            {
                TipoEntidade = a.TipoEntidade,
                EntidadeId = a.EntidadeId,
                ObraId = obraId,
                TipoAssinante = a.TipoAssinante,
                Titulo = titulo,
                Subtitulo = subtitulo,
                DataAssinatura = a.DataAssinatura,
                TotalmenteAssinado = totalmenteAssinado,
            });
        }
        return result;
    }
}
