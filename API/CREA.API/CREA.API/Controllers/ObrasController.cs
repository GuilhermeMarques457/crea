using System.Security.Claims;
using CREA.Application.DTOs.Obras;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ObrasController(
    IObraRepository obraRepository,
    IProfissionalRepository profissionalRepository,
    IProprietarioRepository proprietarioRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ObraDto>>> GetAll()
    {
        var obras = await obraRepository.ListWithDetailsAsync();
        return Ok(obras.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ObraDto>> GetById(Guid id)
    {
        var obra = await obraRepository.GetByIdWithDetailsAsync(id);
        if (obra is null) return NotFound();
        return Ok(ToDto(obra));
    }

    [HttpGet("por-status/{status}")]
    public async Task<ActionResult<IEnumerable<ObraDto>>> GetByStatus(StatusObra status)
    {
        var obras = await obraRepository.GetByStatusAsync(status);
        return Ok(obras.Select(ToDto));
    }

    [HttpGet("por-profissional/{profissionalId:guid}")]
    public async Task<ActionResult<IEnumerable<ObraDto>>> GetByProfissional(Guid profissionalId)
    {
        var obras = await obraRepository.GetByProfissionalAsync(profissionalId);
        return Ok(obras.Select(ToDto));
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<IEnumerable<ObraDto>>> GetMinhas()
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var obras = await obraRepository.GetByUsuarioCriadorAsync(usuarioId);
        return Ok(obras.Select(ToDto));
    }

    [HttpPost]
    public async Task<ActionResult<ObraDto>> Create([FromBody] CreateObraDto dto)
    {
        if (!await profissionalRepository.ExistsAsync(dto.ProfissionalId))
            return BadRequest(new { mensagem = "Profissional responsável não encontrado." });

        if (!await proprietarioRepository.ExistsAsync(dto.ProprietarioId))
            return BadRequest(new { mensagem = "Proprietário não encontrado." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var obra = new Obra
        {
            LocalObra = dto.LocalObra,
            ProprietarioId = dto.ProprietarioId,
            Empresa = dto.Empresa,
            NumeroCaderneta = dto.NumeroCaderneta,
            ProfissionalId = dto.ProfissionalId,
            NumeroArt = dto.NumeroArt,
            NumeroRT = dto.NumeroRT,
            TipoEdificacao = dto.TipoEdificacao,
            AtividadeTecnica = dto.AtividadeTecnica,
            DirecaoTecnica = dto.DirecaoTecnica,
            DataInicio = dto.DataInicio,
            AreaConstruir = dto.AreaConstruir,
            AreaRegularizar = dto.AreaRegularizar,
            AreaAmpliar = dto.AreaAmpliar,
            AreaReformar = dto.AreaReformar,
            AreaTotalEdificada = dto.AreaTotalEdificada,
            ValorRecibo = dto.ValorRecibo,
            UsuarioCriadorId = usuarioId
        };

        await obraRepository.AddAsync(obra);
        var criada = await obraRepository.GetByIdWithDetailsAsync(obra.Id);
        return CreatedAtAction(nameof(GetById), new { id = obra.Id }, ToDto(criada!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateObraDto dto)
    {
        var obra = await obraRepository.GetByIdAsync(id);
        if (obra is null) return NotFound();

        if (!await profissionalRepository.ExistsAsync(dto.ProfissionalId))
            return BadRequest(new { mensagem = "Profissional responsável não encontrado." });

        if (!await proprietarioRepository.ExistsAsync(dto.ProprietarioId))
            return BadRequest(new { mensagem = "Proprietário não encontrado." });

        obra.LocalObra = dto.LocalObra;
        obra.ProprietarioId = dto.ProprietarioId;
        obra.Empresa = dto.Empresa;
        obra.NumeroCaderneta = dto.NumeroCaderneta;
        obra.NumeroArt = dto.NumeroArt;
        obra.NumeroRT = dto.NumeroRT;
        obra.TipoEdificacao = dto.TipoEdificacao;
        obra.AtividadeTecnica = dto.AtividadeTecnica;
        obra.DirecaoTecnica = dto.DirecaoTecnica;
        obra.DataInicio = dto.DataInicio;
        obra.AreaConstruir = dto.AreaConstruir;
        obra.AreaRegularizar = dto.AreaRegularizar;
        obra.AreaAmpliar = dto.AreaAmpliar;
        obra.AreaReformar = dto.AreaReformar;
        obra.AreaTotalEdificada = dto.AreaTotalEdificada;
        obra.ValorRecibo = dto.ValorRecibo;
        obra.ProfissionalId = dto.ProfissionalId;

        await obraRepository.UpdateAsync(obra);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] StatusObra novoStatus)
    {
        var obra = await obraRepository.GetByIdAsync(id);
        if (obra is null) return NotFound();

        obra.Status = novoStatus;
        await obraRepository.UpdateAsync(obra);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await obraRepository.ExistsAsync(id)) return NotFound();
        await obraRepository.DeleteAsync(id);
        return NoContent();
    }

    private static ObraDto ToDto(Obra o) => new()
    {
        Id = o.Id,
        LocalObra = o.LocalObra,
        ProprietarioId = o.ProprietarioId,
        NomeProprietario = o.Proprietario?.Nome ?? string.Empty,
        TelefoneProprietario = string.IsNullOrWhiteSpace(o.Proprietario?.Telefone)
            ? null
            : o.Proprietario!.Telefone,
        Empresa = o.Empresa,
        NumeroCaderneta = o.NumeroCaderneta,
        NumeroArt = o.NumeroArt,
        NumeroRT = o.NumeroRT,
        TipoEdificacao = o.TipoEdificacao,
        AtividadeTecnica = o.AtividadeTecnica,
        DirecaoTecnica = o.DirecaoTecnica,
        Status = o.Status,
        DataInicio = o.DataInicio,
        AreaConstruir = o.AreaConstruir,
        AreaRegularizar = o.AreaRegularizar,
        AreaAmpliar = o.AreaAmpliar,
        AreaReformar = o.AreaReformar,
        AreaTotalEdificada = o.AreaTotalEdificada,
        ValorRecibo = o.ValorRecibo,
        ProfissionalId = o.ProfissionalId,
        NomeProfissional = o.Profissional?.Nome ?? string.Empty,
        UsuarioCriadorId = o.UsuarioCriadorId,
        Ativo = o.Ativo,
        CriadoEm = o.CriadoEm
    };
}
