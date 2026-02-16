using Microsoft.Extensions.DependencyInjection;

namespace api_bank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Registrar servicios de infraestructura aquí
        return services;
    }
}
