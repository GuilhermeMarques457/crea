using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Assinaturas;

public class MinhaAssinaturaDto
{
    public TipoEntidadeAssinatura TipoEntidade { get; set; }
    public Guid EntidadeId { get; set; }
    /// <summary>ID da obra raiz — usado para gerar links de assinatura compartilháveis.</summary>
    public Guid ObraId { get; set; }
    public TipoAssinante TipoAssinante { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public DateTime DataAssinatura { get; set; }
    /// <summary>True quando todos os assinantes requeridos já assinaram a entidade.</summary>
    public bool TotalmenteAssinado { get; set; }
}
