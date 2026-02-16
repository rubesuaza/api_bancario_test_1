using api_bank.Domain.Entities;
using api_bank.Domain.Exceptions;
using api_bank.Domain.Ports.Out;

namespace api_bank.Application.UseCases;

public class ConsultarCuentaUseCase
{
    private readonly ICuentaRepository _cuentaRepository;

    public ConsultarCuentaUseCase(ICuentaRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    public async Task<Cuenta> EjecutarAsync(Guid cuentaId, CancellationToken cancellationToken = default)
    {
        var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId, cancellationToken);
        
        if (cuenta == null)
        {
            throw new EntityNotFoundException($"Cuenta con ID {cuentaId} no encontrada.");
        }

        return cuenta;
    }
}
