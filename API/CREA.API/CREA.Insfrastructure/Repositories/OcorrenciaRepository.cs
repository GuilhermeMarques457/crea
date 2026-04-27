using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class OcorrenciaRepository(ApplicationDbContext context) : Repository<Ocorrencia>(context), IOcorrenciaRepository
{
    public async Task<IEnumerable<Ocorrencia>> GetByObraAsync(Guid obraId) =>
        await _dbSet
            .Include(o => o.Usuario)
            .Where(o => o.ObraId == obraId && o.Ativo)
            .OrderByDescending(o => o.DataOcorrencia)
            .ToListAsync();

    public async Task<IEnumerable<Ocorrencia>> GetByObraAndTipoAsync(Guid obraId, TipoOcorrencia tipo) =>
        await _dbSet
            .Include(o => o.Usuario)
            .Where(o => o.ObraId == obraId && o.Tipo == tipo && o.Ativo)
            .OrderByDescending(o => o.DataOcorrencia)
            .ToListAsync();

    public async Task<Ocorrencia?> GetByIdWithDetailsAsync(Guid id) =>
        await _dbSet
            .Include(o => o.Obra)
            .Include(o => o.Usuario)
            .Include(o => o.Anexos)
            .FirstOrDefaultAsync(o => o.Id == id && o.Ativo);
}
