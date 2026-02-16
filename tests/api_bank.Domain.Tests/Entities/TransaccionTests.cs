using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace api_bank.Domain.Tests.Entities;

public class TransaccionTests
{
    [Fact]
    public void Constructor_ConParametrosValidos_DeberiaCrearInstancia()
    {
        // Arrange
        var transaccionId = Guid.NewGuid();
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = new Dinero(500m, "USD");
        var tipo = TipoMovimiento.TransferenciaEnviada;

        // Act
        var transaccion = new Transaccion(
            transaccionId,
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            tipo,
            "Referencia de prueba"
        );

        // Assert
        transaccion.TransaccionId.Should().Be(transaccionId);
        transaccion.CuentaOrigenId.Should().Be(cuentaOrigenId);
        transaccion.CuentaDestinoId.Should().Be(cuentaDestinoId);
        transaccion.Monto.Should().Be(monto);
        transaccion.Tipo.Should().Be(tipo);
        transaccion.Referencia.Should().Be("Referencia de prueba");
        transaccion.Fecha.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ConReferenciaNula_DeberiaPermitirNull()
    {
        // Arrange
        var transaccionId = Guid.NewGuid();
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = new Dinero(500m, "USD");
        var tipo = TipoMovimiento.TransferenciaEnviada;

        // Act
        var transaccion = new Transaccion(
            transaccionId,
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            tipo,
            null
        );

        // Assert
        transaccion.Referencia.Should().BeNull();
    }

    [Fact]
    public void Constructor_ConCuentasDiferentes_DeberiaCrearInstancia()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = new Dinero(500m, "USD");

        // Act
        var transaccion = new Transaccion(
            Guid.NewGuid(),
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            TipoMovimiento.TransferenciaEnviada,
            "Test"
        );

        // Assert
        transaccion.CuentaOrigenId.Should().NotBe(transaccion.CuentaDestinoId);
    }

    [Theory]
    [InlineData(TipoMovimiento.TransferenciaEnviada)]
    [InlineData(TipoMovimiento.TransferenciaRecibida)]
    [InlineData(TipoMovimiento.Ajuste)]
    public void Constructor_ConDiferentesTiposMovimiento_DeberiaCrearInstancia(TipoMovimiento tipo)
    {
        // Arrange
        var monto = new Dinero(500m, "USD");

        // Act
        var transaccion = new Transaccion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            monto,
            tipo,
            "Test"
        );

        // Assert
        transaccion.Tipo.Should().Be(tipo);
    }
}
