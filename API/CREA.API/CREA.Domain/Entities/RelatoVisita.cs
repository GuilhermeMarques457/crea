using CREA.Domain.Enums;

namespace CREA.Domain.Entities;

public class RelatoVisita : BaseEntity
{
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public int NumeroSequencial { get; set; }
    public DateTime Data { get; set; }
    public string Atividades { get; set; } = string.Empty;
    public string EquipePresente { get; set; } = string.Empty;
    public string? CondicaoClimatica { get; set; }
    public string? Observacoes { get; set; }

    public bool ServicosPreliminar { get; set; }
    public bool Fundacao { get; set; }
    public bool Alvenarias { get; set; }
    public bool Superestrutura { get; set; }
    public bool Cobertura { get; set; }
    public bool EsquadriasInstalacoesEletricasHidraulicas { get; set; }
    public bool RevestimentoForroParePiso { get; set; }
    public bool Pintura { get; set; }
    public bool ServicosComplementares { get; set; }

    public PosicaoObra? PosicaoObra { get; set; }
    public string? DecisoesTecnicas { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public ICollection<Anexo> Anexos { get; set; } = [];
}
