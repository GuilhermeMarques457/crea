using System.Security.Claims;
using CREA.API.Helpers;
using CREA.Application.DTOs.Assinaturas;
using CREA.Application.Helpers;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssinaturasController(
    IAssinaturaRepository assinaturaRepository,
    IUsuarioRepository usuarioRepository,
    IObraRepository obraRepository,
    IRelatoVisitaRepository relatoVisitaRepository,
    ITermoConclusaoRepository termoConclusaoRepository,
    IProfissionalRepository profissionalRepository,
    IProprietarioRepository proprietarioRepository) : ControllerBase
{
    [HttpGet("por-entidade")]
    public async Task<ActionResult<IEnumerable<AssinaturaDto>>> GetPorEntidade(
        [FromQuery] TipoEntidadeAssinatura tipoEntidade,
        [FromQuery] Guid entidadeId)
    {
        if (!await EntidadeExisteAsync(tipoEntidade, entidadeId))
            return NotFound(new { mensagem = "Entidade não encontrada." });

        var assinaturas = await assinaturaRepository.GetByEntidadeAsync(tipoEntidade, entidadeId);
        return Ok(assinaturas.Select(ToDto));
    }

    [HttpGet("pendentes")]
    [Authorize(Roles = "ResponsavelTecnico,UsuarioCrea,Proprietario")]
    public async Task<ActionResult<IEnumerable<PendenteAssinaturaDto>>> GetPendentes()
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario is null) return Unauthorized();

        var pendentes = await assinaturaRepository.GetPendentesParaUsuarioAsync(usuarioId, usuario.TipoUsuario);
        return Ok(pendentes);
    }

    [HttpPost]
    [Authorize(Roles = "ResponsavelTecnico,UsuarioCrea,Proprietario")]
    public async Task<ActionResult<AssinaturaDto>> Assinar([FromBody] CreateAssinaturaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ImagemAssinatura))
            return BadRequest(new { mensagem = "A imagem da assinatura é obrigatória." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario is null) return Unauthorized();

        if (!TryObterTipoAssinante(usuario.TipoUsuario, out var tipoAssinante))
            return Forbid();

        if (!await EntidadeExisteAsync(dto.TipoEntidade, dto.EntidadeId))
            return NotFound(new { mensagem = "Entidade não encontrada." });

        var autorizado = await UsuarioPodeAssinarAsync(usuarioId, usuario.TipoUsuario, tipoAssinante, dto.TipoEntidade, dto.EntidadeId);
        if (!autorizado)
            return Forbid();

        if (await assinaturaRepository.ExisteAssinaturaAsync(dto.TipoEntidade, dto.EntidadeId, tipoAssinante))
            return Conflict(new { mensagem = "Esta assinatura já foi registrada." });

        var dataAssinatura = DateTime.UtcNow;
        var hash = AssinaturaHashGerador.Gerar(dto.TipoEntidade, dto.EntidadeId, usuarioId, tipoAssinante, dataAssinatura);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
        var userAgent = Request.Headers.UserAgent.ToString();
        var (navegadorUa, sistemaUa, dispositivoUa) = UserAgentInfo.Parse(userAgent);

        var assinatura = new Assinatura
        {
            TipoEntidade = dto.TipoEntidade,
            EntidadeId = dto.EntidadeId,
            TipoAssinante = tipoAssinante,
            UsuarioId = usuarioId,
            HashAssinatura = hash,
            DataAssinatura = dataAssinatura,
            ImagemAssinatura = dto.ImagemAssinatura,
            IpAssinante = ip,
            UserAgent = userAgent.Length > 512 ? userAgent[..512] : userAgent,
            Navegador =  navegadorUa,
            SistemaOperacional = dto.SistemaOperacional ?? sistemaUa,
            Dispositivo = dto.Dispositivo ?? dispositivoUa
        };

        await assinaturaRepository.AddAsync(assinatura);

        var criada = await assinaturaRepository.GetByEntidadeETipoAssinanteAsync(dto.TipoEntidade, dto.EntidadeId, tipoAssinante);
        return CreatedAtAction(nameof(GetPorEntidade), new { tipoEntidade = dto.TipoEntidade, entidadeId = dto.EntidadeId }, ToDto(criada!));
    }

    private static bool TryObterTipoAssinante(TipoUsuario tipoUsuario, out TipoAssinante tipoAssinante) =>
        tipoUsuario switch
        {
            TipoUsuario.ResponsavelTecnico => Assign(TipoAssinante.Profissional, out tipoAssinante),
            TipoUsuario.UsuarioCrea => Assign(TipoAssinante.UsuarioCrea, out tipoAssinante),
            TipoUsuario.Proprietario => Assign(TipoAssinante.Proprietario, out tipoAssinante),
            _ => Assign(default, out tipoAssinante) && false
        };

    private static bool Assign(TipoAssinante value, out TipoAssinante tipoAssinante)
    {
        tipoAssinante = value;
        return true;
    }

    private async Task<bool> EntidadeExisteAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId) =>
        tipoEntidade switch
        {
            TipoEntidadeAssinatura.Obra => await obraRepository.ExistsAsync(entidadeId),
            TipoEntidadeAssinatura.RelatoVisita => await relatoVisitaRepository.ExistsAsync(entidadeId),
            TipoEntidadeAssinatura.TermoConclusao => await termoConclusaoRepository.ExistsAsync(entidadeId),
            _ => false
        };

    private async Task<bool> UsuarioPodeAssinarAsync(
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        TipoAssinante tipoAssinante,
        TipoEntidadeAssinatura tipoEntidade,
        Guid entidadeId)
    {
        return (tipoEntidade, tipoAssinante, tipoUsuario) switch
        {
            (TipoEntidadeAssinatura.Obra, TipoAssinante.Profissional, TipoUsuario.ResponsavelTecnico) =>
                await ObraPertenceAoProfissionalAsync(entidadeId, usuarioId),

            (TipoEntidadeAssinatura.Obra, TipoAssinante.UsuarioCrea, TipoUsuario.UsuarioCrea) => true,

            (TipoEntidadeAssinatura.RelatoVisita, TipoAssinante.Profissional, TipoUsuario.ResponsavelTecnico) =>
                await RelatoPertenceAoProfissionalAsync(entidadeId, usuarioId),

            (TipoEntidadeAssinatura.RelatoVisita, TipoAssinante.Proprietario, TipoUsuario.Proprietario) =>
                await RelatoPertenceAoProprietarioAsync(entidadeId, usuarioId),

            (TipoEntidadeAssinatura.TermoConclusao, TipoAssinante.Profissional, TipoUsuario.ResponsavelTecnico) =>
                await TermoPertenceAoProfissionalAsync(entidadeId, usuarioId),

            (TipoEntidadeAssinatura.TermoConclusao, TipoAssinante.Proprietario, TipoUsuario.Proprietario) =>
                await TermoPertenceAoProprietarioAsync(entidadeId, usuarioId),

            _ => false
        };
    }

    private async Task<bool> ObraPertenceAoProfissionalAsync(Guid obraId, Guid usuarioId)
    {
        var obra = await obraRepository.GetByIdWithDetailsAsync(obraId);
        return obra?.ProfissionalResponsavel?.UsuarioId == usuarioId;
    }

    private async Task<bool> RelatoPertenceAoProfissionalAsync(Guid relatoId, Guid usuarioId)
    {
        var relato = await relatoVisitaRepository.GetByIdWithDetailsAsync(relatoId);
        return relato?.Obra.ProfissionalResponsavelId is Guid profId
            && (await profissionalRepository.GetByIdAsync(profId))?.UsuarioId == usuarioId;
    }

    private async Task<bool> RelatoPertenceAoProprietarioAsync(Guid relatoId, Guid usuarioId)
    {
        var relato = await relatoVisitaRepository.GetByIdWithDetailsAsync(relatoId);
        var proprietario = await proprietarioRepository.GetByUsuarioIdAsync(usuarioId);
        return relato is not null && proprietario is not null && relato.Obra.ProprietarioId == proprietario.Id;
    }

    private async Task<bool> TermoPertenceAoProfissionalAsync(Guid termoId, Guid usuarioId)
    {
        var termo = await termoConclusaoRepository.GetByIdAsync(termoId);
        if (termo is null) return false;
        var profissional = await profissionalRepository.GetByIdAsync(termo.ProfissionalId);
        return profissional?.UsuarioId == usuarioId;
    }

    private async Task<bool> TermoPertenceAoProprietarioAsync(Guid termoId, Guid usuarioId)
    {
        var termo = await termoConclusaoRepository.GetByObraAsync(
            (await termoConclusaoRepository.GetByIdAsync(termoId))?.ObraId ?? Guid.Empty);
        var proprietario = await proprietarioRepository.GetByUsuarioIdAsync(usuarioId);
        return termo is not null && proprietario is not null && termo.Obra.ProprietarioId == proprietario.Id;
    }

    private static AssinaturaDto ToDto(Assinatura a) => new()
    {
        Id = a.Id,
        TipoEntidade = a.TipoEntidade,
        EntidadeId = a.EntidadeId,
        TipoAssinante = a.TipoAssinante,
        UsuarioId = a.UsuarioId,
        NomeUsuario = a.Usuario?.Nome ?? string.Empty,
        HashAssinatura = a.HashAssinatura,
        DataAssinatura = a.DataAssinatura,
        ImagemAssinatura = a.ImagemAssinatura,
        IpAssinante = a.IpAssinante,
        UserAgent = a.UserAgent,
        Navegador = a.Navegador,
        SistemaOperacional = a.SistemaOperacional,
        Dispositivo = a.Dispositivo
    };
}
