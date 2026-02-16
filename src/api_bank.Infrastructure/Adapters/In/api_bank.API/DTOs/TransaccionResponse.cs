namespace api_bank.API.DTOs;

public record TransaccionResponse
{
    public Guid TransaccionId { get; init; }
    public decimal Monto { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
}
