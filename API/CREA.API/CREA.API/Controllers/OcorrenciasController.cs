using System.Security.Claims;
using CREA.Application.DTOs.Ocorrencias;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OcorrenciasController(
    IOcorrenciaRepository ocorrenciaRepository,
    IObraRepository obraRepository) : ControllerBase
{
    [HttpGet("por-obra/{obraId:guid}")]
    public async Task<ActionResult<IEnumerable<OcorrenciaDto>>> GetByObra(Guid obraId)
    {
        if (!await obraRepository.ExistsAsync(obraId)) return NotFound(new { mensagem = "Obra não encontrada." });

        var ocorrencias = await ocorrenciaRepository.GetByObraAsync(obraId);
        return Ok(ocorrencias.Select(ToDto));
    }

    [HttpGet("por-obra/{obraId:guid}/tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<OcorrenciaDto>>> GetByObraAndTipo(Guid obraId, TipoOcorrencia tipo)
    {
        var ocorrencias = await ocorrenciaRepository.GetByObraAndTipoAsync(obraId, tipo);
        return Ok(ocorrencias.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OcorrenciaDto>> GetById(Guid id)
    {
        var ocorrencia = await ocorrenciaRepository.GetByIdWithDetailsAsync(id);
        if (ocorrencia is null) return NotFound();
        return Ok(ToDto(ocorrencia));
    }

    [HttpPost]
    public async Task<ActionResult<OcorrenciaDto>> Create([FromBody] CreateOcorrenciaDto dto)
    {
        if (!await obraRepository.ExistsAsync(dto.ObraId))
            return BadRequest(new { mensagem = "Obra não encontrada." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var ocorrencia = new Ocorrencia
        {
            ObraId = dto.ObraId,
            DataOcorrencia = dto.DataOcorrencia,
            Tipo = dto.Tipo,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Providencias = dto.Providencias,
            UsuarioId = usuarioId
        };

        await ocorrenciaRepository.AddAsync(ocorrencia);
        var criada = await ocorrenciaRepository.GetByIdWithDetailsAsync(ocorrencia.Id);
        return CreatedAtAction(nameof(GetById), new { id = ocorrencia.Id }, ToDto(criada!));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateOcorrenciaDto dto)
    {
        var ocorrencia = await ocorrenciaRepository.GetByIdAsync(id);
        if (ocorrencia is null) return NotFound();

        ocorrencia.DataOcorrencia = dto.DataOcorrencia;
        ocorrencia.Tipo = dto.Tipo;
        ocorrencia.Titulo = dto.Titulo;
        ocorrencia.Descricao = dto.Descricao;
        ocorrencia.Providencias = dto.Providencias;

        await ocorrenciaRepository.UpdateAsync(ocorrencia);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador,ResponsavelTecnico")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await ocorrenciaRepository.ExistsAsync(id)) return NotFound();
        await ocorrenciaRepository.DeleteAsync(id);
        return NoContent();
    }

    private static OcorrenciaDto ToDto(Ocorrencia o) => new()
    {
        Id = o.Id,
        ObraId = o.ObraId,
        NomeObra = o.Obra?.Nome ?? string.Empty,
        DataOcorrencia = o.DataOcorrencia,
        Tipo = o.Tipo,
        Titulo = o.Titulo,
        Descricao = o.Descricao,
        Providencias = o.Providencias,
        UsuarioId = o.UsuarioId,
        NomeUsuario = o.Usuario?.Nome ?? string.Empty,
        Ativo = o.Ativo,
        CriadoEm = o.CriadoEm,
        QuantidadeAnexos = o.Anexos?.Count(a => a.Ativo) ?? 0
    };
}
