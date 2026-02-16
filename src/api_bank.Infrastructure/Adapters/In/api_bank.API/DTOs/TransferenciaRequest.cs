namespace api_bank.API.DTOs;

public record TransferenciaRequest
{
    public Guid CuentaOrigenId { get; init; }
    public Guid CuentaDestinoId { get; init; }
    public decimal Monto { get; init; }
    public string Concepto { get; init; } = string.Empty;
}
