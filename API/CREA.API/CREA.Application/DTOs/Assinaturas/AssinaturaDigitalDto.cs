namespace CREA.Application.DTOs.Assinaturas;

public class AssinaturaDigitalDto
{
    public Guid Id { get; set; }
    public Guid RegistroDiarioId { get; set; }
    public DateTime DataRegistroDiario { get; set; }
    public Guid ProfissionalId { get; set; }
    public string NomeProfissional { get; set; } = string.Empty;
    public string NumeroRegistro { get; set; } = string.Empty;
    public DateTime DataAssinatura { get; set; }
    public string HashAssinatura { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
}
