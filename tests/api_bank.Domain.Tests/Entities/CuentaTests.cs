using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Exceptions;
using api_bank.Domain.ValueObjects;
using FluentAssertions;

namespace api_bank.Domain.Tests.Entities;

public class CuentaTests
{
    [Fact]
    public void Constructor_ConParametrosValidos_DeberiaCrearInstancia()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldo = new Dinero(1000m, "USD");

        // Act
        var cuenta = new Cuenta(cuentaId, "123456789", saldo, titularId, EstadoCuenta.Activa);

        // Assert
        cuenta.CuentaId.Should().Be(cuentaId);
        cuenta.NumeroCuenta.Should().Be("123456789");
        cuenta.Saldo.Should().Be(saldo);
        cuenta.TitularId.Should().Be(titularId);
        cuenta.Estado.Should().Be(EstadoCuenta.Activa);
        cuenta.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Debitar_ConSaldoSuficiente_DeberiaReducirSaldo()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(300m, "USD");

        // Act
        cuenta.Debitar(monto);

        // Assert
        cuenta.Saldo.Cantidad.Should().Be(700m);
    }

    [Fact]
    public void Debitar_ConSaldoInsuficiente_DeberiaLanzarInsufficientFundsException()
    {
        // Arrange
        var saldoInicial = new Dinero(100m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(200m, "USD");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<InsufficientFundsException>();
    }

    [Fact]
    public void Debitar_ConMontoCero_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(0m, "USD");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Debitar_ConMontoNegativo_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(-100m, "USD");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Debitar_ConSaldoExacto_DeberiaReducirSaldoACero()
    {
        // Arrange
        var saldoInicial = new Dinero(500m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(500m, "USD");

        // Act
        cuenta.Debitar(monto);

        // Assert
        cuenta.Saldo.Cantidad.Should().Be(0m);
    }

    [Fact]
    public void Debitar_ConCuentaBloqueada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Bloqueada);
        var monto = new Dinero(300m, "USD");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*bloqueada*");
    }

    [Fact]
    public void Debitar_ConCuentaSuspendida_DeberiaLanzarExcepcion()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Suspendida);
        var monto = new Dinero(300m, "USD");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*suspendida*");
    }

    [Fact]
    public void Acreditar_ConMontoValido_DeberiaIncrementarSaldo()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(500m, "USD");

        // Act
        cuenta.Acreditar(monto);

        // Assert
        cuenta.Saldo.Cantidad.Should().Be(1500m);
    }

    [Fact]
    public void Acreditar_ConMontoCero_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(0m, "USD");

        // Act & Assert
        var act = () => cuenta.Acreditar(monto);
        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Acreditar_ConMontoNegativo_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(-100m, "USD");

        // Act & Assert
        var act = () => cuenta.Acreditar(monto);
        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Acreditar_ConDivisaDiferente_DeberiaLanzarExcepcion()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(500m, "EUR");

        // Act & Assert
        var act = () => cuenta.Acreditar(monto);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*misma divisa*");
    }

    [Fact]
    public void Debitar_ConDivisaDiferente_DeberiaLanzarExcepcion()
    {
        // Arrange
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(Guid.NewGuid(), "123456789", saldoInicial, Guid.NewGuid(), EstadoCuenta.Activa);
        var monto = new Dinero(300m, "EUR");

        // Act & Assert
        var act = () => cuenta.Debitar(monto);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*misma divisa*");
    }
}
