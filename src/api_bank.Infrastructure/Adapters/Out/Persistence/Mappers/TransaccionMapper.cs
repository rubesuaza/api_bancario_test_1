using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.ValueObjects;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

internal static class TransaccionMapper
{
    public static Transaccion ToDomain(TransaccionEntity entity)
    {
        var monto = new Dinero(entity.Amount, entity.Currency);
        var tipo = Enum.Parse<TipoMovimiento>(entity.TransactionType, ignoreCase: true);
        
        return new Transaccion(
            entity.Id,
            entity.OriginAccountId,
            entity.DestinationAccountId,
            monto,
            tipo,
            entity.Description);
    }

    public static TransaccionEntity ToEntity(Transaccion domain)
    {
        return new TransaccionEntity
        {
            Id = domain.TransaccionId,
            OriginAccountId = domain.CuentaOrigenId,
            DestinationAccountId = domain.CuentaDestinoId,
            Amount = domain.Monto.Cantidad,
            Currency = domain.Monto.Divisa,
            TransactionType = domain.Tipo.ToString(),
            Description = domain.Referencia,
            CreatedAt = domain.Fecha
        };
    }
}
