using CREA.Domain.Enums;

namespace CREA.Application.DTOs.Obras;

public class ObraDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public Guid ProprietarioId { get; set; }
    public string NomeProprietario { get; set; } = string.Empty;
    public string? TelefoneProprietario { get; set; }
    public string? Empresa { get; set; }
    public string? NumeroCaderneta { get; set; }
    public string NumeroArt { get; set; } = string.Empty;
    public string? NumeroRT { get; set; }
    public TipoObra TipoObra { get; set; }
    public TipoEdificacao? TipoEdificacao { get; set; }
    public AtividadeTecnica? AtividadeTecnica { get; set; }
    public bool DirecaoTecnica { get; set; }
    public StatusObra Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataPrevisaoTermino { get; set; }
    public string? Descricao { get; set; }
    public decimal? AreaConstruir { get; set; }
    public decimal? AreaRegularizar { get; set; }
    public decimal? AreaAmpliar { get; set; }
    public decimal? AreaReformar { get; set; }
    public decimal? AreaTotalEdificada { get; set; }
    public decimal? ValorRecibo { get; set; }
    public Guid ProfissionalResponsavelId { get; set; }
    public string NomeProfissionalResponsavel { get; set; } = string.Empty;
    public Guid UsuarioCriadorId { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
