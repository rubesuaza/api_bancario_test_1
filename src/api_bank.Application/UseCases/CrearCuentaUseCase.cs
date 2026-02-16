using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;

namespace api_bank.Application.UseCases;

public class CrearCuentaUseCase
{
    private readonly ICuentaRepository _cuentaRepository;

    public CrearCuentaUseCase(ICuentaRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    public async Task<Cuenta> EjecutarAsync(Guid titularId, string moneda, decimal saldoInicial, CancellationToken cancellationToken = default)
    {
        var cuentaId = Guid.NewGuid();
        var numeroCuenta = $"ACC-{cuentaId.ToString().Substring(0, 8).ToUpper()}";
        var saldo = new Dinero(saldoInicial, moneda);
        var cuenta = new Cuenta(cuentaId, numeroCuenta, saldo, titularId, EstadoCuenta.Activa);

        return await _cuentaRepository.GuardarAsync(cuenta, cancellationToken);
    }
}
