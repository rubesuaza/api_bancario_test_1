using api_bank.Domain.Entities;
using api_bank.Domain.Ports.Out;

namespace api_bank.Application.UseCases;

public class ObtenerHistorialTransaccionesUseCase
{
    private readonly ITransaccionRepository _transaccionRepository;

    public ObtenerHistorialTransaccionesUseCase(ITransaccionRepository transaccionRepository)
    {
        _transaccionRepository = transaccionRepository;
    }

    public async Task<IEnumerable<Transaccion>> EjecutarAsync(Guid cuentaId, CancellationToken cancellationToken = default)
    {
        return await _transaccionRepository.ObtenerPorCuentaIdAsync(cuentaId, cancellationToken);
    }
}
