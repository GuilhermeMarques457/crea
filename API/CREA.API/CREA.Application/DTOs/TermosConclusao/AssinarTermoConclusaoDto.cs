using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.TermosConclusao;

public class AssinarTermoConclusaoDto
{
    [Required(ErrorMessage = "A imagem da assinatura é obrigatória.")]
    public string ImagemAssinatura { get; set; } = string.Empty; // Base64 PNG (data:image/png;base64,...)
}
