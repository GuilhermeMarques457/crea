using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class AnexoRepository(ApplicationDbContext context) : Repository<Anexo>(context), IAnexoRepository
{
    public async Task<IEnumerable<Anexo>> GetByObraAsync(Guid obraId) =>
        await _dbSet
            .Include(a => a.Usuario)
            .Where(a => a.ObraId == obraId && a.Ativo)
            .ToListAsync();

    public async Task<IEnumerable<Anexo>> GetByRegistroDiarioAsync(Guid registroDiarioId) =>
        await _dbSet
            .Include(a => a.Usuario)
            .Where(a => a.RegistroDiarioId == registroDiarioId && a.Ativo)
            .ToListAsync();

    public async Task<IEnumerable<Anexo>> GetByOcorrenciaAsync(Guid ocorrenciaId) =>
        await _dbSet
            .Include(a => a.Usuario)
            .Where(a => a.OcorrenciaId == ocorrenciaId && a.Ativo)
            .ToListAsync();
}
