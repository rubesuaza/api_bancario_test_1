using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Exceptions;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;

namespace api_bank.Application.UseCases;

public class TransferirFondosUseCase
{
    private readonly ICuentaRepository _cuentaRepository;
    private readonly ITransaccionRepository _transaccionRepository;

    public TransferirFondosUseCase(
        ICuentaRepository cuentaRepository,
        ITransaccionRepository transaccionRepository)
    {
        _cuentaRepository = cuentaRepository;
        _transaccionRepository = transaccionRepository;
    }

    public async Task<Transaccion> EjecutarAsync(
        Guid cuentaOrigenId,
        Guid cuentaDestinoId,
        decimal monto,
        string concepto,
        CancellationToken cancellationToken = default)
    {
        if (cuentaOrigenId == cuentaDestinoId)
        {
            throw new SelfTransferException("No se puede transferir fondos a la misma cuenta.");
        }

        var cuentaOrigen = await _cuentaRepository.ObtenerPorIdAsync(cuentaOrigenId, cancellationToken);
        if (cuentaOrigen == null)
        {
            throw new EntityNotFoundException($"Cuenta origen con ID {cuentaOrigenId} no encontrada.");
        }

        var cuentaDestino = await _cuentaRepository.ObtenerPorIdAsync(cuentaDestinoId, cancellationToken);
        if (cuentaDestino == null)
        {
            throw new EntityNotFoundException($"Cuenta destino con ID {cuentaDestinoId} no encontrada.");
        }

        var montoDinero = new Dinero(monto, cuentaOrigen.Saldo.Divisa);

        // Realizar la transferencia
        cuentaOrigen.Debitar(montoDinero);
        cuentaDestino.Acreditar(montoDinero);

        // Guardar cambios
        await _cuentaRepository.GuardarAsync(cuentaOrigen, cancellationToken);
        await _cuentaRepository.GuardarAsync(cuentaDestino, cancellationToken);

        // Registrar transacción
        var transaccionId = Guid.NewGuid();
        var transaccion = new Transaccion(
            transaccionId,
            cuentaOrigenId,
            cuentaDestinoId,
            montoDinero,
            TipoMovimiento.TransferenciaEnviada,
            concepto);

        return await _transaccionRepository.GuardarAsync(transaccion, cancellationToken);
    }
}
