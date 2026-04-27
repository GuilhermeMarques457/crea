using CREA.Domain.Enums;

namespace CREA.Domain.Entities;

public class Ocorrencia : BaseEntity
{
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public DateTime DataOcorrencia { get; set; }
    public TipoOcorrencia Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Providencias { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public ICollection<Anexo> Anexos { get; set; } = [];
}
