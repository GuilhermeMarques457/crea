using CREA.Application.DTOs.Assinaturas;
using CREA.Domain.Enums;

namespace CREA.Application.DTOs.RelatoVisita;

public class RelatoVisitaDto
{
    public Guid Id { get; set; }
    public Guid ObraId { get; set; }
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
    public string NomeUsuario { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public int TotalAssinaturas { get; set; }
    public bool AssinadoPeloProfissional { get; set; }
    public bool AssinadoPeloProprietario { get; set; }
    public int QuantidadeAnexos { get; set; }
    public List<AssinaturaDto> Assinaturas { get; set; } = [];
}
