using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class RelatoVisitaRepository(ApplicationDbContext context) : Repository<RelatoVisita>(context), IRelatoVisitaRepository
{
    public async Task<IEnumerable<RelatoVisita>> GetByObraAsync(Guid obraId) =>
        await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Anexos)
            .Where(r => r.ObraId == obraId && r.Ativo)
            .OrderByDescending(r => r.Data)
            .ToListAsync();

    public async Task<IEnumerable<RelatoVisita>> GetByObraAndPeriodoAsync(Guid obraId, DateTime inicio, DateTime fim) =>
        await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Anexos)
            .Where(r => r.ObraId == obraId && r.Data >= inicio && r.Data <= fim && r.Ativo)
            .OrderByDescending(r => r.Data)
            .ToListAsync();

    public async Task<RelatoVisita?> GetByIdWithDetailsAsync(Guid id) =>
        await _dbSet
            .Include(r => r.Obra).ThenInclude(o => o.Proprietario)
            .Include(r => r.Usuario)
            .Include(r => r.Anexos)
            .FirstOrDefaultAsync(r => r.Id == id && r.Ativo);
}
