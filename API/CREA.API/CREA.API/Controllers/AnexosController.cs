using System.Security.Claims;
using CREA.Application.DTOs.Anexos;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnexosController(IAnexoRepository anexoRepository) : ControllerBase
{
    private const long LimiteTamanhoBytes = 10 * 1024 * 1024; // 10MB
    private static readonly string[] TiposPermitidos = ["image/jpeg", "image/png", "image/gif", "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"];

    [HttpGet("por-obra/{obraId:guid}")]
    public async Task<ActionResult<IEnumerable<AnexoDto>>> GetByObra(Guid obraId)
    {
        var anexos = await anexoRepository.GetByObraAsync(obraId);
        return Ok(anexos.Select(a => ToDto(a, Request, null)));
    }

    [HttpGet("por-registro/{registroDiarioId:guid}")]
    public async Task<ActionResult<IEnumerable<AnexoDto>>> GetByRegistro(Guid registroDiarioId)
    {
        var anexos = await anexoRepository.GetByRelatoVisitaAsync(registroDiarioId);
        return Ok(anexos.Select(a => ToDto(a, Request, null)));
    }

    [HttpGet("download/{id:guid}")]
    public async Task<IActionResult> Download(Guid id)
    {
        var anexo = await anexoRepository.GetByIdAsync(id);
        if (anexo is null) return NotFound();

        var caminho = Path.Combine(Directory.GetCurrentDirectory(), "uploads", anexo.CaminhoArquivo);
        if (!System.IO.File.Exists(caminho))
            return NotFound(new { mensagem = "Arquivo não encontrado no servidor." });

        var bytes = await System.IO.File.ReadAllBytesAsync(caminho);
        return File(bytes, anexo.TipoArquivo, anexo.NomeArquivoOriginal);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<AnexoDto>> Upload(
        IFormFile arquivo,
        [FromQuery] Guid? obraId,
        [FromQuery] Guid? registroDiarioId)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { mensagem = "Nenhum arquivo enviado." });

        if (arquivo.Length > LimiteTamanhoBytes)
            return BadRequest(new { mensagem = "Arquivo excede o limite de 10MB." });

        if (!TiposPermitidos.Contains(arquivo.ContentType))
            return BadRequest(new { mensagem = "Tipo de arquivo não permitido." });

        if (obraId is null && registroDiarioId is null)
            return BadRequest(new { mensagem = "É necessário informar obraId ou registroDiarioId." });

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";
        var pasta = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(pasta);

        var caminho = Path.Combine(pasta, nomeArquivo);
        await using (var stream = new FileStream(caminho, FileMode.Create))
            await arquivo.CopyToAsync(stream);

        var anexo = new Anexo
        {
            NomeArquivo = nomeArquivo,
            NomeArquivoOriginal = arquivo.FileName,
            CaminhoArquivo = nomeArquivo,
            TipoArquivo = arquivo.ContentType,
            TamanhoBytes = arquivo.Length,
            ObraId = obraId,
            RelatoVisitaId = registroDiarioId,
            UsuarioId = usuarioId
        };

        await anexoRepository.AddAsync(anexo);
        var nomeUsuario = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        return CreatedAtAction(nameof(Download), new { id = anexo.Id }, ToDto(anexo, Request, nomeUsuario));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var anexo = await anexoRepository.GetByIdAsync(id);
        if (anexo is null) return NotFound();

        var caminho = Path.Combine(Directory.GetCurrentDirectory(), "uploads", anexo.CaminhoArquivo);
        if (System.IO.File.Exists(caminho))
            System.IO.File.Delete(caminho);

        await anexoRepository.DeleteAsync(id);
        return NoContent();
    }

    private static AnexoDto ToDto(Anexo a, HttpRequest request, string? nomeUsuarioFallback) => new()
    {
        Id = a.Id,
        NomeArquivo = a.NomeArquivo,
        NomeArquivoOriginal = a.NomeArquivoOriginal,
        TipoArquivo = a.TipoArquivo,
        TamanhoBytes = a.TamanhoBytes,
        ObraId = a.ObraId,
        RelatoVisitaId = a.RelatoVisitaId,
        UsuarioId = a.UsuarioId,
        NomeUsuario = a.Usuario?.Nome ?? nomeUsuarioFallback ?? string.Empty,
        UrlDownload = $"{request.Scheme}://{request.Host}/uploads/{a.NomeArquivo}",
        CriadoEm = a.CriadoEm
    };
}
