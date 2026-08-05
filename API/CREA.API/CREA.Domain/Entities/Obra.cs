using CREA.Domain.Enums;

namespace CREA.Domain.Entities;

public class Obra : BaseEntity
{
    public string LocalObra { get; set; } = string.Empty;

    public Guid ProprietarioId { get; set; }
    public Proprietario Proprietario { get; set; } = null!;

    public string? Empresa { get; set; }
    public string? NumeroCaderneta { get; set; }
    public string NumeroArt { get; set; } = string.Empty;
    public string? NumeroRT { get; set; }
    public TipoEdificacao? TipoEdificacao { get; set; }
    public AtividadeTecnica? AtividadeTecnica { get; set; }
    public bool DirecaoTecnica { get; set; }
    public StatusObra Status { get; set; } = StatusObra.EmAndamento;
    public DateTime DataInicio { get; set; }

    // Áreas (m²)
    public decimal? AreaConstruir { get; set; }
    public decimal? AreaRegularizar { get; set; }
    public decimal? AreaAmpliar { get; set; }
    public decimal? AreaReformar { get; set; }
    public decimal? AreaTotalEdificada { get; set; }

    // Valor do recibo
    public decimal? ValorRecibo { get; set; }

    public Guid ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;

    public Guid UsuarioCriadorId { get; set; }
    public Usuario UsuarioCriador { get; set; } = null!;

    public ICollection<RelatoVisita> RelatoVisita { get; set; } = [];
    public ICollection<Anexo> Anexos { get; set; } = [];
    public TermoConclusao? TermoConclusao { get; set; }
}
