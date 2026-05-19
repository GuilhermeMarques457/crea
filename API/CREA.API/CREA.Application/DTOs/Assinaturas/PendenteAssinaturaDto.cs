using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class PendenteAssinaturaDto
{
    public TipoEntidadeAssinatura TipoEntidade { get; set; }
    public Guid EntidadeId { get; set; }
    public TipoAssinante TipoAssinante { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public DateTime CriadoEm { get; set; }
}
