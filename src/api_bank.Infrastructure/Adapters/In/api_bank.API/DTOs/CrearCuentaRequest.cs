namespace api_bank.API.DTOs;

public record CrearCuentaRequest
{
    public Guid TitularId { get; init; }
    public string Moneda { get; init; } = string.Empty;
    public decimal SaldoInicial { get; init; }
}
