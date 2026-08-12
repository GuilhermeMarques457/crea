using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CREA.Application.DTOs.Auth;
using CREA.Application.DTOs.Usuarios;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CREA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUsuarioRepository usuarioRepository, IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(dto.Email);
        if (usuario is null || !VerificarSenha(dto.Senha, usuario.SenhaHash))
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var token = GerarToken(usuario);
        return Ok(new LoginResponseDto
        {
            Token = token,
            Expiracao = DateTime.Now.AddHours(GetExpiracaoHoras()),
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario
        });
    }

    [HttpPost("esqueci-senha")]
    [AllowAnonymous]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaDto dto)
    {
        _ = await usuarioRepository.GetByEmailAsync(dto.Email);
        return Ok(new
        {
            mensagem = "Se o e-mail estiver cadastrado, você receberá instruções para redefinir sua senha."
        });
    }

    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaDto dto)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario is null) return NotFound();

        if (!VerificarSenha(dto.SenhaAtual, usuario.SenhaHash))
            return BadRequest(new { mensagem = "Senha atual incorreta." });

        usuario.SenhaHash = HashSenha(dto.NovaSenha);
        await usuarioRepository.UpdateAsync(usuario);

        return Ok(new { mensagem = "Senha alterada com sucesso." });
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioDto>> Registrar([FromBody] CreateUsuarioDto dto)
    {
        if (await usuarioRepository.EmailExisteAsync(dto.Email))
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = HashSenha(dto.Senha),
            TipoUsuario = dto.TipoUsuario
        };

        await usuarioRepository.AddAsync(usuario);

        return CreatedAtAction(nameof(Login), new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            Ativo = usuario.Ativo,
            CriadoEm = usuario.CriadoEm
        });
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> Perfil()
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
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

    private string GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(GetExpiracaoHoras()),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetExpiracaoHoras() =>
        int.TryParse(configuration["Jwt:ExpiracaoHoras"], out var h) ? h : 8;

    public static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }

    private static bool VerificarSenha(string senha, string hash) =>
        HashSenha(senha) == hash;
}
