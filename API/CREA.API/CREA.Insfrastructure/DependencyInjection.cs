using CREA.Application.Interfaces.Repositories;
using CREA.Insfrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CREA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }
}
