namespace CREA.Domain.Entities;

public class Anexo : BaseEntity
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }

    public Guid? ObraId { get; set; }
    public Obra? Obra { get; set; }

    public Guid? RelatoVisitaId { get; set; }
    public RelatoVisita? RelatoVisita { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
