using api_bank.Application.UseCases;
using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Exceptions;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace api_bank.Application.Tests.UseCases;

public class CrearCuentaUseCaseTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepositoryMock;
    private readonly CrearCuentaUseCase _useCase;

    public CrearCuentaUseCaseTests()
    {
        _cuentaRepositoryMock = new Mock<ICuentaRepository>();
        _useCase = new CrearCuentaUseCase(_cuentaRepositoryMock.Object);
    }

    [Fact]
    public async Task EjecutarAsync_ConParametrosValidos_DeberiaCrearCuenta()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "USD";
        var saldoInicial = 1000m;
        var cuentaEsperada = new Cuenta(
            Guid.NewGuid(),
            "ACC-12345678",
            new Dinero(saldoInicial, moneda),
            titularId,
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta c, CancellationToken ct) => c);

        // Act
        var resultado = await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TitularId.Should().Be(titularId);
        resultado.Saldo.Cantidad.Should().Be(saldoInicial);
        resultado.Saldo.Divisa.Should().Be(moneda);
        resultado.Estado.Should().Be(EstadoCuenta.Activa);
        resultado.NumeroCuenta.Should().StartWith("ACC-");
        resultado.NumeroCuenta.Length.Should().BeGreaterThanOrEqualTo(12); // "ACC-" + al menos 8 caracteres

        _cuentaRepositoryMock.Verify(
            r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ConMonedaEUR_DeberiaCrearCuentaConEUR()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "EUR";
        var saldoInicial = 500m;

        _cuentaRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta c, CancellationToken ct) => c);

        // Act
        var resultado = await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);

        // Assert
        resultado.Saldo.Divisa.Should().Be("EUR");
        resultado.Saldo.Cantidad.Should().Be(500m);
    }

    [Fact]
    public async Task EjecutarAsync_ConSaldoCero_DeberiaLanzarExcepcion()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "USD";
        var saldoInicial = 0m;

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);
        await act.Should().ThrowAsync<InvalidAmountException>();

        _cuentaRepositoryMock.Verify(
            r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_ConSaldoNegativo_DeberiaLanzarExcepcion()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "USD";
        var saldoInicial = -100m;

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);
        await act.Should().ThrowAsync<InvalidAmountException>();

        _cuentaRepositoryMock.Verify(
            r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_DeberiaGenerarNumeroCuentaUnico()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "USD";
        var saldoInicial = 1000m;

        _cuentaRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta c, CancellationToken ct) => c);

        // Act
        var cuenta1 = await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);
        var cuenta2 = await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);

        // Assert
        cuenta1.NumeroCuenta.Should().NotBe(cuenta2.NumeroCuenta);
        cuenta1.CuentaId.Should().NotBe(cuenta2.CuentaId);
    }

    [Fact]
    public async Task EjecutarAsync_DeberiaEstablecerFechaCreacion()
    {
        // Arrange
        var titularId = Guid.NewGuid();
        var moneda = "USD";
        var saldoInicial = 1000m;

        _cuentaRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta c, CancellationToken ct) => c);

        // Act
        var resultado = await _useCase.EjecutarAsync(titularId, moneda, saldoInicial);

        // Assert
        resultado.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
