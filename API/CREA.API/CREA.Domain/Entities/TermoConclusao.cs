namespace CREA.Domain.Entities;

public class TermoConclusao : BaseEntity
{
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public int NumeroTermo { get; set; }
    public DateTime DataConclusao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }

    public string? DeclaracaoTexto { get; set; }
    public string? LocalDeclaracao { get; set; }
    public DateTime? DataDeclaracao { get; set; }
}
