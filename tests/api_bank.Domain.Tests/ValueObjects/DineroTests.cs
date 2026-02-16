using api_bank.Domain.Exceptions;
using api_bank.Domain.ValueObjects;
using FluentAssertions;

namespace api_bank.Domain.Tests.ValueObjects;

public class DineroTests
{
    [Fact]
    public void Constructor_ConMontoPositivo_DeberiaCrearInstancia()
    {
        // Arrange & Act
        var dinero = new Dinero(100.50m, "USD");

        // Assert
        dinero.Cantidad.Should().Be(100.50m);
        dinero.Divisa.Should().Be("USD");
    }

    [Fact]
    public void Constructor_ConMontoCero_DeberiaLanzarInvalidAmountException()
    {
        // Arrange & Act & Assert
        var act = () => new Dinero(0m, "USD");
        act.Should().Throw<InvalidAmountException>()
            .WithMessage("*mayor a cero*");
    }

    [Fact]
    public void Constructor_ConMontoNegativo_DeberiaLanzarInvalidAmountException()
    {
        // Arrange & Act & Assert
        var act = () => new Dinero(-10m, "USD");
        act.Should().Throw<InvalidAmountException>()
            .WithMessage("*mayor a cero*");
    }

    [Fact]
    public void EsSuficiente_ConSaldoMayorAlMonto_DeberiaRetornarTrue()
    {
        // Arrange
        var saldo = new Dinero(100m, "USD");
        var monto = new Dinero(50m, "USD");

        // Act
        var resultado = saldo.EsSuficiente(monto);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void EsSuficiente_ConSaldoIgualAlMonto_DeberiaRetornarTrue()
    {
        // Arrange
        var saldo = new Dinero(100m, "USD");
        var monto = new Dinero(100m, "USD");

        // Act
        var resultado = saldo.EsSuficiente(monto);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void EsSuficiente_ConSaldoMenorAlMonto_DeberiaRetornarFalse()
    {
        // Arrange
        var saldo = new Dinero(50m, "USD");
        var monto = new Dinero(100m, "USD");

        // Act
        var resultado = saldo.EsSuficiente(monto);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EsSuficiente_ConDivisasDiferentes_DeberiaLanzarExcepcion()
    {
        // Arrange
        var saldo = new Dinero(100m, "USD");
        var monto = new Dinero(50m, "EUR");

        // Act & Assert
        var act = () => saldo.EsSuficiente(monto);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*misma divisa*");
    }

    [Fact]
    public void Sumar_ConDivisasIguales_DeberiaRetornarNuevoDinero()
    {
        // Arrange
        var dinero1 = new Dinero(50m, "USD");
        var dinero2 = new Dinero(30m, "USD");

        // Act
        var resultado = dinero1.Sumar(dinero2);

        // Assert
        resultado.Cantidad.Should().Be(80m);
        resultado.Divisa.Should().Be("USD");
    }

    [Fact]
    public void Restar_ConDivisasIguales_DeberiaRetornarNuevoDinero()
    {
        // Arrange
        var dinero1 = new Dinero(100m, "USD");
        var dinero2 = new Dinero(30m, "USD");

        // Act
        var resultado = dinero1.Restar(dinero2);

        // Assert
        resultado.Cantidad.Should().Be(70m);
        resultado.Divisa.Should().Be("USD");
    }

    [Fact]
    public void Restar_ConResultadoNegativo_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var dinero1 = new Dinero(50m, "USD");
        var dinero2 = new Dinero(100m, "USD");

        // Act & Assert
        var act = () => dinero1.Restar(dinero2);
        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Restar_ConResultadoCero_DeberiaPermitirCero()
    {
        // Arrange
        var dinero1 = new Dinero(100m, "USD");
        var dinero2 = new Dinero(100m, "USD");

        // Act
        var resultado = dinero1.Restar(dinero2);

        // Assert
        resultado.Cantidad.Should().Be(0m);
        resultado.Divisa.Should().Be("USD");
    }

    [Fact]
    public void Restar_ConDivisasDiferentes_DeberiaLanzarExcepcion()
    {
        // Arrange
        var dinero1 = new Dinero(100m, "USD");
        var dinero2 = new Dinero(50m, "EUR");

        // Act & Assert
        var act = () => dinero1.Restar(dinero2);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*misma divisa*");
    }

    [Fact]
    public void Igualdad_ConMismosValores_DeberiaSerIgual()
    {
        // Arrange
        var dinero1 = new Dinero(100m, "USD");
        var dinero2 = new Dinero(100m, "USD");

        // Act & Assert
        dinero1.Should().Be(dinero2);
        (dinero1 == dinero2).Should().BeTrue();
    }

    [Fact]
    public void Igualdad_ConValoresDiferentes_DeberiaSerDiferente()
    {
        // Arrange
        var dinero1 = new Dinero(100m, "USD");
        var dinero2 = new Dinero(50m, "USD");

        // Act & Assert
        dinero1.Should().NotBe(dinero2);
        (dinero1 != dinero2).Should().BeTrue();
    }
}
