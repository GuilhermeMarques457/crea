using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CREA.Insfrastructure.Repositories;

public class ProfissionalRepository(ApplicationDbContext context) : Repository<Profissional>(context), IProfissionalRepository
{
    public async Task<Profissional?> GetByNumeroRegistroAsync(string numeroRegistro) =>
        await _dbSet.FirstOrDefaultAsync(p => p.NumeroRegistro == numeroRegistro && p.Ativo);

    public async Task<Profissional?> GetByCpfAsync(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        var formatted = digits.Length == 11
            ? $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}"
            : cpf;
        return await _dbSet.FirstOrDefaultAsync(p =>
            (p.Cpf == cpf || p.Cpf == digits || p.Cpf == formatted) && p.Ativo);
    }

    public async Task<IEnumerable<Profissional>> GetByTipoRegistroAsync(string tipoRegistro) =>
        await _dbSet.Where(p => p.TipoRegistro == tipoRegistro && p.Ativo).ToListAsync();

    public async Task<bool> NumeroRegistroExisteAsync(string numeroRegistro) =>
        await _dbSet.AnyAsync(p => p.NumeroRegistro == numeroRegistro);

    public async Task<Profissional?> GetByUsuarioIdAsync(Guid usuarioId) =>
        await _dbSet.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.Ativo);
}
