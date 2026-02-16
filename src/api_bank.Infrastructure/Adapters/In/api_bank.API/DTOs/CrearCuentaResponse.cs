namespace api_bank.API.DTOs;

public record CrearCuentaResponse
{
    public Guid CuentaId { get; init; }
    public string NumeroCuenta { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public decimal Saldo { get; init; }
    public DateTime FechaCreacion { get; init; }
}
