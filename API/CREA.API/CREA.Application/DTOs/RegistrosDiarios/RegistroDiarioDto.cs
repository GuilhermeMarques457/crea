using CREA.Domain.Enums;

namespace CREA.Application.DTOs.RegistrosDiarios;

public class RegistroDiarioDto
{
    public Guid Id { get; set; }
    public Guid ObraId { get; set; }
    public string NomeObra { get; set; } = string.Empty;
    public int NumeroSequencial { get; set; }
    public DateTime Data { get; set; }
    public string Atividades { get; set; } = string.Empty;
    public string EquipePresente { get; set; } = string.Empty;
    public string? CondicaoClimatica { get; set; }
    public string? Observacoes { get; set; }

    // Etapas da obra
    public bool ServicosPreliminar { get; set; }
    public bool Fundacao { get; set; }
    public bool Alvenarias { get; set; }
    public bool Superestrutura { get; set; }
    public bool Cobertura { get; set; }
    public bool EsquadriasInstalacoesEletricasHidraulicas { get; set; }
    public bool RevestimentoForroParePiso { get; set; }
    public bool Pintura { get; set; }
    public bool ServicosComplementares { get; set; }

    // Posição da obra
    public PosicaoObra? PosicaoObra { get; set; }
    public string? DecisoesTecnicas { get; set; }

    // Assinatura do proprietário
    public string? AssinaturaProprietario { get; set; }
    public DateTime? DataAssinaturaProprietario { get; set; }

    // Assinatura do responsável técnico
    public string? ImagemAssinaturaResponsavel { get; set; }
    public DateTime? DataAssinaturaResponsavel { get; set; }

    public Guid UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public int TotalAssinaturas { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
