using api_bank.Domain.Entities;

namespace api_bank.Domain.Ports.Out;

public interface ICuentaRepository
{
    Task<Cuenta?> ObtenerPorIdAsync(Guid cuentaId, CancellationToken cancellationToken = default);
    Task<Cuenta?> ObtenerPorNumeroCuentaAsync(string numeroCuenta, CancellationToken cancellationToken = default);
    Task<Cuenta> GuardarAsync(Cuenta cuenta, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(Guid cuentaId, CancellationToken cancellationToken = default);
}
