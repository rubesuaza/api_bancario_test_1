namespace api_bank.API.DTOs;

public record TransferenciaResponse
{
    public Guid TransaccionId { get; init; }
    public string Estado { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
}
