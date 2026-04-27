using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface IRegistroDiarioRepository : IRepository<RegistroDiario>
{
    Task<IEnumerable<RegistroDiario>> GetByObraAsync(Guid obraId);
    Task<IEnumerable<RegistroDiario>> GetByObraAndPeriodoAsync(Guid obraId, DateTime inicio, DateTime fim);
    Task<RegistroDiario?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<RegistroDiario>> GetPendentesAssinaturaAsync(Guid usuarioId);
}
