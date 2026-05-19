using CREA.Domain.Entities;

namespace CREA.Application.Interfaces.Repositories;

public interface IRelatoVisitaRepository : IRepository<RelatoVisita>
{
    Task<IEnumerable<RelatoVisita>> GetByObraAsync(Guid obraId);
    Task<IEnumerable<RelatoVisita>> GetByObraAndPeriodoAsync(Guid obraId, DateTime inicio, DateTime fim);
    Task<RelatoVisita?> GetByIdWithDetailsAsync(Guid id);
}
