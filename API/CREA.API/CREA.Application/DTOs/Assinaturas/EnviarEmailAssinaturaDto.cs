using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class EnviarEmailAssinaturaDto
{
    public TipoEntidadeAssinatura TipoEntidade { get; set; }
    public Guid EntidadeId { get; set; }
    public string? LinkAssinatura { get; set; }
    public string EmailUsuarioLogado { get; set; } = string.Empty;
}
