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

public class ConsultarCuentaUseCaseTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepositoryMock;
    private readonly ConsultarCuentaUseCase _useCase;

    public ConsultarCuentaUseCaseTests()
    {
        _cuentaRepositoryMock = new Mock<ICuentaRepository>();
        _useCase = new ConsultarCuentaUseCase(_cuentaRepositoryMock.Object);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaExistente_DeberiaRetornarCuenta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var cuentaEsperada = new Cuenta(
            cuentaId,
            "ACC-12345678",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaEsperada);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.CuentaId.Should().Be(cuentaId);
        resultado.NumeroCuenta.Should().Be("ACC-12345678");
        resultado.Saldo.Cantidad.Should().Be(1000m);
        resultado.Saldo.Divisa.Should().Be("USD");
        resultado.Estado.Should().Be(EstadoCuenta.Activa);

        _cuentaRepositoryMock.Verify(
            r => r.ObtenerPorIdAsync(cuentaId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaNoExistente_DeberiaLanzarEntityNotFoundException()
    {
        // Arrange
        var cuentaIdInexistente = Guid.NewGuid();

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaIdInexistente, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta?)null);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(cuentaIdInexistente);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"*{cuentaIdInexistente}*");
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaBloqueada_DeberiaRetornarCuenta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var cuentaBloqueada = new Cuenta(
            cuentaId,
            "ACC-BLOQUEADA",
            new Dinero(500m, "EUR"),
            Guid.NewGuid(),
            EstadoCuenta.Bloqueada
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaBloqueada);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoCuenta.Bloqueada);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaSuspendida_DeberiaRetornarCuenta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var cuentaSuspendida = new Cuenta(
            cuentaId,
            "ACC-SUSPENDIDA",
            new Dinero(300m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Suspendida
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaSuspendida);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoCuenta.Suspendida);
    }

    [Fact]
    public async Task EjecutarAsync_ConDiferentesMonedas_DeberiaRetornarCuentaCorrecta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var cuentaEUR = new Cuenta(
            cuentaId,
            "ACC-EUR",
            new Dinero(2000m, "EUR"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaEUR);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Saldo.Divisa.Should().Be("EUR");
        resultado.Saldo.Cantidad.Should().Be(2000m);
    }
}
