using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;

namespace CREA.Insfrastructure.Repositories;

public class ProprietarioRepository(ApplicationDbContext context) : Repository<Proprietario>(context), IProprietarioRepository
{
}
