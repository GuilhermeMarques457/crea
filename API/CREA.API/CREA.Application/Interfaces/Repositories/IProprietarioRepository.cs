using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface IProprietarioRepository : IRepository<Proprietario>
{
    Task<Proprietario?> GetByUsuarioIdAsync(Guid usuarioId);
}
