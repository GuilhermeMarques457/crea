using System.Security.Claims;
using CREA.Application.DTOs.Usuarios;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController(IUsuarioRepository usuarioRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await usuarioRepository.GetAllAsync();
        return Ok(usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            TipoUsuario = u.TipoUsuario,
            Ativo = u.Ativo,
            CriadoEm = u.CriadoEm
        }));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id);
        if (usuario is null) return NotFound();

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            Ativo = usuario.Ativo,
            CriadoEm = usuario.CriadoEm
        });
    }

    [HttpGet("por-tipo/{tipo}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetByTipo(TipoUsuario tipo)
    {
        var usuarios = await usuarioRepository.GetByTipoAsync(tipo);
        return Ok(usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            TipoUsuario = u.TipoUsuario,
            Ativo = u.Ativo,
            CriadoEm = u.CriadoEm
        }));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioDto dto)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id);
        if (usuario is null) return NotFound();

        if (usuario.Email != dto.Email && await usuarioRepository.EmailExisteAsync(dto.Email))
            return Conflict(new { mensagem = "E-mail já em uso." });

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.TipoUsuario = dto.TipoUsuario;
        usuario.Ativo = dto.Ativo;

        await usuarioRepository.UpdateAsync(usuario);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await usuarioRepository.ExistsAsync(id)) return NotFound();

        var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == usuarioLogadoId)
            return BadRequest(new { mensagem = "Não é possível excluir o próprio usuário." });

        await usuarioRepository.DeleteAsync(id);
        return NoContent();
    }
}
