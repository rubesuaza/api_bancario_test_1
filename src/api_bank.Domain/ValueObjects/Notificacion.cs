namespace api_bank.Domain.ValueObjects;

public record Notificacion
{
    public string Destinatario { get; init; }
    public string Contenido { get; init; }
    public bool EstadoEnvio { get; init; }

    public Notificacion(string destinatario, string contenido, bool estadoEnvio)
    {
        Destinatario = destinatario ?? throw new ArgumentNullException(nameof(destinatario));
        Contenido = contenido ?? throw new ArgumentNullException(nameof(contenido));
        EstadoEnvio = estadoEnvio;
    }
}
