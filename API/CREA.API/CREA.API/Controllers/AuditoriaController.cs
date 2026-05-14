using CREA.Application.DTOs.Auditoria;
using CREA.Application.DTOs.Common;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AuditoriaController(ILogAuditoriaRepository logAuditoriaRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<LogAuditoriaDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? entidade = null,
        [FromQuery] string? acao = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await logAuditoriaRepository.GetPagedAsync(page, pageSize, entidade, acao, usuarioId, inicio, fim);

        return Ok(new PagedResult<LogAuditoriaDto>
        {
            Items = result.Items.Select(ToDto),
            TotalItems = result.TotalItems,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    [HttpGet("por-usuario/{usuarioId:guid}")]
    public async Task<IActionResult> GetByUsuario(Guid usuarioId)
    {
        var logs = await logAuditoriaRepository.GetByUsuarioAsync(usuarioId);
        return Ok(logs.Select(ToDto));
    }

    [HttpGet("por-entidade/{entidade}/{entidadeId}")]
    public async Task<IActionResult> GetByEntidade(string entidade, string entidadeId)
    {
        var logs = await logAuditoriaRepository.GetByEntidadeAsync(entidade, entidadeId);
        return Ok(logs.Select(ToDto));
    }

    [HttpGet("por-periodo")]
    public async Task<IActionResult> GetByPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        var logs = await logAuditoriaRepository.GetByPeriodoAsync(inicio, fim);
        return Ok(logs.Select(ToDto));
    }

    private static LogAuditoriaDto ToDto(LogAuditoria l) => new()
    {
        Id = l.Id,
        UsuarioId = l.UsuarioId,
        NomeUsuario = l.NomeUsuario,
        Acao = l.Acao,
        Entidade = l.Entidade,
        EntidadeId = l.EntidadeId,
        DadosAntigos = l.DadosAntigos,
        DadosNovos = l.DadosNovos,
        EnderecoIp = l.EnderecoIp,
        DataAcao = l.DataAcao
    };
}
