using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.Application.Interfaces.Repositories;

public interface IOcorrenciaRepository : IRepository<Ocorrencia>
{
    Task<IEnumerable<Ocorrencia>> GetByObraAsync(Guid obraId);
    Task<IEnumerable<Ocorrencia>> GetByObraAndTipoAsync(Guid obraId, TipoOcorrencia tipo);
    Task<Ocorrencia?> GetByIdWithDetailsAsync(Guid id);
}
