using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class AssinaturaDto
{
    public Guid Id { get; set; }
    public TipoEntidadeAssinatura TipoEntidade { get; set; }
    public Guid EntidadeId { get; set; }
    public TipoAssinante TipoAssinante { get; set; }
    public Guid UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string HashAssinatura { get; set; } = string.Empty;
    public DateTime DataAssinatura { get; set; }
    public string ImagemAssinatura { get; set; } = string.Empty;
    public string IpAssinante { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Navegador { get; set; }
    public string? SistemaOperacional { get; set; }
    public string? Dispositivo { get; set; }
}
