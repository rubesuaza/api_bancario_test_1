using System.Reflection;
using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.ValueObjects;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

internal static class CuentaMapper
{
    public static Cuenta ToDomain(CuentaEntity entity)
    {
        Dinero saldo;
        
        // Manejar el caso especial cuando el balance es cero
        if (entity.Balance == 0m)
        {
            // Usar reflexión para acceder al constructor privado que permite cero
            var constructor = typeof(Dinero).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(c => c.GetParameters().Length == 3);
            
            if (constructor != null)
            {
                saldo = (Dinero)constructor.Invoke(new object[] { 0m, entity.Currency, true });
            }
            else
            {
                // Fallback: usar un valor mínimo si no podemos acceder al constructor
                saldo = new Dinero(0.01m, entity.Currency);
            }
        }
        else
        {
            saldo = new Dinero(entity.Balance, entity.Currency);
        }
        
        var estado = Enum.Parse<EstadoCuenta>(entity.Status, ignoreCase: true);
        
        return new Cuenta(
            entity.Id,
            entity.AccountNumber,
            saldo,
            entity.OwnerId,
            estado);
    }

    public static CuentaEntity ToEntity(Cuenta domain)
    {
        return new CuentaEntity
        {
            Id = domain.CuentaId,
            AccountNumber = domain.NumeroCuenta,
            OwnerId = domain.TitularId,
            Balance = domain.Saldo.Cantidad,
            Currency = domain.Saldo.Divisa,
            Status = domain.Estado.ToString(),
            CreatedAt = domain.FechaCreacion
        };
    }

    public static void UpdateEntity(CuentaEntity entity, Cuenta domain)
    {
        entity.Balance = domain.Saldo.Cantidad;
        entity.Status = domain.Estado.ToString();
    }
}
