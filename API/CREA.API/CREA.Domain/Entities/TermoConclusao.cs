namespace CREA.Domain.Entities;

public class TermoConclusao : BaseEntity
{
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public int NumeroTermo { get; set; }
    public DateTime DataConclusao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }

    public string? Empresa { get; set; }
    public string? Proprietario { get; set; }
    public string? TelefoneProprietario { get; set; }
    public string? LocalObra { get; set; }

    public string? DeclaracaoTexto { get; set; }
    public string? LocalDeclaracao { get; set; }
    public DateTime? DataDeclaracao { get; set; }

    public Guid ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;
}
