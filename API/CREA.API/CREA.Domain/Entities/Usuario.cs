using CREA.Domain.Enums;

namespace CREA.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; }

    public ICollection<RegistroDiario> RegistrosDiarios { get; set; } = [];
    public ICollection<Ocorrencia> Ocorrencias { get; set; } = [];
    public ICollection<Anexo> Anexos { get; set; } = [];
}
