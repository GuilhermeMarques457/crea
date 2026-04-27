using System.ComponentModel.DataAnnotations;

namespace CREA.Application.DTOs.RegistrosDiarios;

public class AssinarRegistroDiarioDto
{
    [Required(ErrorMessage = "A imagem da assinatura é obrigatória.")]
    public string ImagemAssinatura { get; set; } = string.Empty;
}
