namespace CREA.Domain.Entities;

public class TermoConclusao : BaseEntity
{
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public int NumeroTermo { get; set; }
    public DateTime DataConclusao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }

    // Dados replicados do recibo para o termo
    public string? Empresa { get; set; }
    public string? Proprietario { get; set; }
    public string? TelefoneProprietario { get; set; }
    public string? LocalObra { get; set; }

    // Texto da declaração formal
    public string? DeclaracaoTexto { get; set; }
    public string? LocalDeclaracao { get; set; }
    public DateTime? DataDeclaracao { get; set; }

    public Guid ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;

    public string HashAssinatura { get; set; } = string.Empty;
    public DateTime DataAssinatura { get; set; } = DateTime.UtcNow;

    // Assinatura do proprietário
    public string? AssinaturaProprietario { get; set; }
    public DateTime? DataAssinaturaProprietario { get; set; }

    public bool AssinadoPeloResponsavel { get; set; }
    public bool AssinadoPeloAdmin { get; set; }
    public bool Concluido => AssinadoPeloResponsavel && AssinadoPeloAdmin;

    public ICollection<AssinaturaTermoConclusao> Assinaturas { get; set; } = [];
}
