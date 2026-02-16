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

public class TransferirFondosUseCaseTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepositoryMock;
    private readonly Mock<ITransaccionRepository> _transaccionRepositoryMock;
    private readonly TransferirFondosUseCase _useCase;

    public TransferirFondosUseCaseTests()
    {
        _cuentaRepositoryMock = new Mock<ICuentaRepository>();
        _transaccionRepositoryMock = new Mock<ITransaccionRepository>();
        _useCase = new TransferirFondosUseCase(
            _cuentaRepositoryMock.Object,
            _transaccionRepositoryMock.Object);
    }

    [Fact]
    public async Task EjecutarAsync_ConParametrosValidos_DeberiaTransferirFondos()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 500m;
        var concepto = "Transferencia de prueba";

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        var cuentaDestino = new Cuenta(
            cuentaDestinoId,
            "ACC-DESTINO",
            new Dinero(200m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaDestino);

        _cuentaRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Cuenta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta c, CancellationToken ct) => c);

        var transaccionEsperada = new Transaccion(
            Guid.NewGuid(),
            cuentaOrigenId,
            cuentaDestinoId,
            new Dinero(monto, "USD"),
            TipoMovimiento.TransferenciaEnviada,
            concepto
        );

        _transaccionRepositoryMock
            .Setup(r => r.GuardarAsync(It.IsAny<Transaccion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaccion t, CancellationToken ct) => t);

        // Act
        var resultado = await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            concepto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.CuentaOrigenId.Should().Be(cuentaOrigenId);
        resultado.CuentaDestinoId.Should().Be(cuentaDestinoId);
        resultado.Monto.Cantidad.Should().Be(monto);
        resultado.Referencia.Should().Be(concepto);
        resultado.Tipo.Should().Be(TipoMovimiento.TransferenciaEnviada);

        cuentaOrigen.Saldo.Cantidad.Should().Be(500m); // 1000 - 500
        cuentaDestino.Saldo.Cantidad.Should().Be(700m); // 200 + 500

        _cuentaRepositoryMock.Verify(
            r => r.GuardarAsync(cuentaOrigen, It.IsAny<CancellationToken>()),
            Times.Once);
        _cuentaRepositoryMock.Verify(
            r => r.GuardarAsync(cuentaDestino, It.IsAny<CancellationToken>()),
            Times.Once);
        _transaccionRepositoryMock.Verify(
            r => r.GuardarAsync(It.IsAny<Transaccion>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ConMismaCuentaOrigenYDestino_DeberiaLanzarSelfTransferException()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var monto = 500m;
        var concepto = "Transferencia inválida";

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaId,
            cuentaId,
            monto,
            concepto);

        await act.Should().ThrowAsync<SelfTransferException>()
            .WithMessage("*misma cuenta*");

        _cuentaRepositoryMock.Verify(
            r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaOrigenNoExistente_DeberiaLanzarEntityNotFoundException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 500m;

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta?)null);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"*{cuentaOrigenId}*");
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaDestinoNoExistente_DeberiaLanzarEntityNotFoundException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 500m;

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cuenta?)null);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"*{cuentaDestinoId}*");
    }

    [Fact]
    public async Task EjecutarAsync_ConSaldoInsuficiente_DeberiaLanzarInsufficientFundsException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 1500m; // Mayor que el saldo disponible

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        var cuentaDestino = new Cuenta(
            cuentaDestinoId,
            "ACC-DESTINO",
            new Dinero(200m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaDestino);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<InsufficientFundsException>();

        _transaccionRepositoryMock.Verify(
            r => r.GuardarAsync(It.IsAny<Transaccion>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaOrigenBloqueada_DeberiaLanzarInvalidOperationException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 500m;

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Bloqueada
        );

        var cuentaDestino = new Cuenta(
            cuentaDestinoId,
            "ACC-DESTINO",
            new Dinero(200m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaDestino);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*bloqueada*");
    }

    [Fact]
    public async Task EjecutarAsync_ConDivisasDiferentes_DeberiaLanzarArgumentException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 500m;

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        var cuentaDestino = new Cuenta(
            cuentaDestinoId,
            "ACC-DESTINO",
            new Dinero(200m, "EUR"), // Diferente divisa
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaDestino);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*misma divisa*");
    }

    [Fact]
    public async Task EjecutarAsync_ConMontoCero_DeberiaLanzarInvalidAmountException()
    {
        // Arrange
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = 0m;

        var cuentaOrigen = new Cuenta(
            cuentaOrigenId,
            "ACC-ORIGEN",
            new Dinero(1000m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        var cuentaDestino = new Cuenta(
            cuentaDestinoId,
            "ACC-DESTINO",
            new Dinero(200m, "USD"),
            Guid.NewGuid(),
            EstadoCuenta.Activa
        );

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaOrigenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaOrigen);

        _cuentaRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(cuentaDestinoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentaDestino);

        // Act & Assert
        var act = async () => await _useCase.EjecutarAsync(
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            "Test");

        await act.Should().ThrowAsync<InvalidAmountException>();
    }
}
