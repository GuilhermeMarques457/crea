namespace CREA.Application.DTOs.Assinaturas;

public class PreviewEmailAssinaturaDto
{
    public string Titulo { get; set; } = string.Empty;
    public IReadOnlyList<DestinatarioAssinaturaEmailDto> Destinatarios { get; set; } = [];
}
