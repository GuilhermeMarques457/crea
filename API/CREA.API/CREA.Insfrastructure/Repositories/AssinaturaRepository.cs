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
}
