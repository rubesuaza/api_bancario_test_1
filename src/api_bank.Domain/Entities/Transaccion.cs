using api_bank.Domain.Enums;
using api_bank.Domain.ValueObjects;

namespace api_bank.Domain.Entities;

public class Transaccion
{
    public Guid TransaccionId { get; private set; }
    public Guid CuentaOrigenId { get; private set; }
    public Guid CuentaDestinoId { get; private set; }
    public Dinero Monto { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Referencia { get; private set; }

    private Transaccion() { } // Para EF Core

    public Transaccion(
        Guid transaccionId,
        Guid cuentaOrigenId,
        Guid cuentaDestinoId,
        Dinero monto,
        TipoMovimiento tipo,
        string? referencia = null)
    {
        TransaccionId = transaccionId;
        CuentaOrigenId = cuentaOrigenId;
        CuentaDestinoId = cuentaDestinoId;
        Monto = monto ?? throw new ArgumentNullException(nameof(monto));
        Tipo = tipo;
        Referencia = referencia;
        Fecha = DateTime.UtcNow;
    }
}
