using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class DestinatarioAssinaturaEmailDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAssinante TipoAssinante { get; set; }
}
