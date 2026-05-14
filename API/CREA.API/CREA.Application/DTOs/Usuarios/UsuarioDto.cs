using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Usuarios;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
