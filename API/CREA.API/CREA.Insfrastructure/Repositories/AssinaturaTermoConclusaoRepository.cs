using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Infrastructure;

namespace CREA.Insfrastructure.Repositories;

public class AssinaturaTermoConclusaoRepository(ApplicationDbContext context)
    : Repository<AssinaturaTermoConclusao>(context), IAssinaturaTermoConclusaoRepository
{
}
