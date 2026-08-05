using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class TermoConclusaoRepository(ApplicationDbContext context) : Repository<TermoConclusao>(context), ITermoConclusaoRepository
{
    public async Task<TermoConclusao?> GetByObraAsync(Guid obraId) =>
        await _dbSet
            .AsSplitQuery()
            .Include(t => t.Obra)
                .ThenInclude(o => o.Profissional)
            .Include(t => t.Obra)
                .ThenInclude(o => o.Proprietario)
            .FirstOrDefaultAsync(t => t.ObraId == obraId && t.Ativo);

    public async Task<TermoConclusao?> GetByIdWithDetailsAsync(Guid termoId) =>
        await _dbSet
            .AsSplitQuery()
            .Include(t => t.Obra)
            .ThenInclude(o => o.Profissional)
            .Include(t => t.Obra)
            .ThenInclude(o => o.Proprietario)
            .FirstOrDefaultAsync(t => t.Id == termoId && t.Ativo);

    public async Task<bool> ObraPossuiTermoAsync(Guid obraId) =>
        await _dbSet.AnyAsync(t => t.ObraId == obraId && t.Ativo);
}
