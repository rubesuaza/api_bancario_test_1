namespace api_bank.Domain.Entities;

public record CuentaId(Guid Value)
{
    public static CuentaId New() => new(Guid.NewGuid());
}
