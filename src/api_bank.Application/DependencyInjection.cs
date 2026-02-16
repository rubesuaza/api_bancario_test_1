using Microsoft.Extensions.DependencyInjection;

namespace api_bank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de aplicación aquí
        return services;
    }
}
