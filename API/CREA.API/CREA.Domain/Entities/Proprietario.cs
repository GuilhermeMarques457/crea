namespace CREA.Domain.Entities;

public class Proprietario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    public ICollection<Obra> Obras { get; set; } = [];
}
