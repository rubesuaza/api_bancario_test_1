using api_bank.Domain.Enums;
using api_bank.Domain.Exceptions;
using api_bank.Domain.ValueObjects;

namespace api_bank.Domain.Entities;

public class Cuenta
{
    public Guid CuentaId { get; private set; }
    public string NumeroCuenta { get; private set; }
    public Dinero Saldo { get; private set; }
    public Guid TitularId { get; private set; }
    public EstadoCuenta Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Cuenta() { } // Para EF Core

    public Cuenta(Guid cuentaId, string numeroCuenta, Dinero saldo, Guid titularId, EstadoCuenta estado)
    {
        CuentaId = cuentaId;
        NumeroCuenta = numeroCuenta ?? throw new ArgumentNullException(nameof(numeroCuenta));
        Saldo = saldo ?? throw new ArgumentNullException(nameof(saldo));
        TitularId = titularId;
        Estado = estado;
        FechaCreacion = DateTime.UtcNow;
    }

    public void Debitar(Dinero monto)
    {
        ValidarEstadoActivo();
        ValidarMismaDivisa(monto);

        if (monto.Cantidad <= 0)
        {
            throw new InvalidAmountException(monto.Cantidad);
        }

        if (!Saldo.EsSuficiente(monto))
        {
            throw new InsufficientFundsException(Saldo.Cantidad, monto.Cantidad);
        }

        Saldo = Saldo.Restar(monto);
    }

    public void Acreditar(Dinero monto)
    {
        ValidarMismaDivisa(monto);

        if (monto.Cantidad <= 0)
        {
            throw new InvalidAmountException(monto.Cantidad);
        }

        Saldo = Saldo.Sumar(monto);
    }

    private void ValidarEstadoActivo()
    {
        if (Estado == EstadoCuenta.Bloqueada)
        {
            throw new InvalidOperationException("No se pueden realizar operaciones en una cuenta bloqueada.");
        }

        if (Estado == EstadoCuenta.Suspendida)
        {
            throw new InvalidOperationException("No se pueden realizar operaciones en una cuenta suspendida.");
        }
    }

    private void ValidarMismaDivisa(Dinero monto)
    {
        if (Saldo.Divisa != monto.Divisa)
        {
            throw new ArgumentException($"Las operaciones solo pueden realizarse con la misma divisa. Divisa de la cuenta: {Saldo.Divisa}, Divisa proporcionada: {monto.Divisa}");
        }
    }
}
