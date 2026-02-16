using api_bank.Domain.ValueObjects;
using FluentAssertions;

namespace api_bank.Domain.Tests.ValueObjects;

public class NotificacionTests
{
    [Fact]
    public void Constructor_ConParametrosValidos_DeberiaCrearInstancia()
    {
        // Arrange & Act
        var notificacion = new Notificacion("usuario@example.com", "Transferencia realizada", true);

        // Assert
        notificacion.Destinatario.Should().Be("usuario@example.com");
        notificacion.Contenido.Should().Be("Transferencia realizada");
        notificacion.EstadoEnvio.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ConEstadoEnvioFalse_DeberiaCrearInstancia()
    {
        // Arrange & Act
        var notificacion = new Notificacion("usuario@example.com", "Transferencia fallida", false);

        // Assert
        notificacion.EstadoEnvio.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ConDestinatarioNulo_DeberiaLanzarArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new Notificacion(null!, "Contenido", true);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ConContenidoNulo_DeberiaLanzarArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new Notificacion("destinatario", null!, true);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Igualdad_ConMismosValores_DeberiaSerIgual()
    {
        // Arrange
        var notificacion1 = new Notificacion("usuario@example.com", "Mensaje", true);
        var notificacion2 = new Notificacion("usuario@example.com", "Mensaje", true);

        // Act & Assert
        notificacion1.Should().Be(notificacion2);
    }

    [Fact]
    public void Igualdad_ConValoresDiferentes_DeberiaSerDiferente()
    {
        // Arrange
        var notificacion1 = new Notificacion("usuario1@example.com", "Mensaje", true);
        var notificacion2 = new Notificacion("usuario2@example.com", "Mensaje", true);

        // Act & Assert
        notificacion1.Should().NotBe(notificacion2);
    }
}
