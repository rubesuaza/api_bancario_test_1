using api_bank.Domain.Entities;

namespace api_bank.Domain.Ports.Out;

public interface ITransaccionRepository
{
    Task<Transaccion> GuardarAsync(Transaccion transaccion, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaccion>> ObtenerPorCuentaIdAsync(Guid cuentaId, CancellationToken cancellationToken = default);
}
