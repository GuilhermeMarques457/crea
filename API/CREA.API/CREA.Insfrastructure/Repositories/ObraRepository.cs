using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class ObraRepository(ApplicationDbContext context) : Repository<Obra>(context), IObraRepository
{
    public async Task<Obra?> GetByIdWithDetailsAsync(Guid id) =>
        await _dbSet
            .Include(o => o.Proprietario)
            .Include(o => o.ProfissionalResponsavel)
            .Include(o => o.UsuarioCriador)
            .FirstOrDefaultAsync(o => o.Id == id && o.Ativo);

    public async Task<IEnumerable<Obra>> GetByProfissionalAsync(Guid profissionalId) =>
        await _dbSet
            .Include(o => o.Proprietario)
            .Include(o => o.ProfissionalResponsavel)
            .Where(o => o.ProfissionalResponsavelId == profissionalId && o.Ativo)
            .ToListAsync();

    public async Task<IEnumerable<Obra>> GetByStatusAsync(StatusObra status) =>
        await _dbSet
            .Include(o => o.Proprietario)
            .Include(o => o.ProfissionalResponsavel)
            .Where(o => o.Status == status && o.Ativo)
            .ToListAsync();

    public async Task<IEnumerable<Obra>> GetByUsuarioCriadorAsync(Guid usuarioId) =>
        await _dbSet
            .Include(o => o.Proprietario)
            .Include(o => o.ProfissionalResponsavel)
            .Where(o => o.UsuarioCriadorId == usuarioId && o.Ativo)
            .ToListAsync();

    public async Task<IEnumerable<Obra>> ListWithDetailsAsync() =>
        await _dbSet
            .Include(o => o.Proprietario)
            .Include(o => o.ProfissionalResponsavel)
            .ToListAsync();

    public async Task<bool> ExisteObraAtivaComProprietarioAsync(Guid proprietarioId) =>
        await _dbSet.AnyAsync(o => o.ProprietarioId == proprietarioId && o.Ativo);
}
