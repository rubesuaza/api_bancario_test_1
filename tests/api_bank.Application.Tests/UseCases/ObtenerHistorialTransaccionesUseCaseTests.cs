using api_bank.Application.UseCases;
using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace api_bank.Application.Tests.UseCases;

public class ObtenerHistorialTransaccionesUseCaseTests
{
    private readonly Mock<ITransaccionRepository> _transaccionRepositoryMock;
    private readonly ObtenerHistorialTransaccionesUseCase _useCase;

    public ObtenerHistorialTransaccionesUseCaseTests()
    {
        _transaccionRepositoryMock = new Mock<ITransaccionRepository>();
        _useCase = new ObtenerHistorialTransaccionesUseCase(_transaccionRepositoryMock.Object);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaConTransacciones_DeberiaRetornarTransacciones()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();

        var transaccionesEsperadas = new List<Transaccion>
        {
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(100m, "USD"),
                TipoMovimiento.TransferenciaEnviada,
                "Transferencia 1"
            ),
            new Transaccion(
                Guid.NewGuid(),
                otraCuentaId,
                cuentaId,
                new Dinero(50m, "USD"),
                TipoMovimiento.TransferenciaRecibida,
                "Transferencia 2"
            ),
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(200m, "USD"),
                TipoMovimiento.TransferenciaEnviada,
                "Transferencia 3"
            )
        };

        _transaccionRepositoryMock
            .Setup(r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaccionesEsperadas);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Should().NotBeNull();
        var transacciones = resultado.ToList();
        transacciones.Should().HaveCount(3);
        transacciones.Should().BeEquivalentTo(transaccionesEsperadas);

        _transaccionRepositoryMock.Verify(
            r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ConCuentaSinTransacciones_DeberiaRetornarListaVacia()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();

        _transaccionRepositoryMock
            .Setup(r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Transaccion>());

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task EjecutarAsync_ConDiferentesTiposMovimiento_DeberiaRetornarTodos()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();

        var transaccionesEsperadas = new List<Transaccion>
        {
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(100m, "USD"),
                TipoMovimiento.TransferenciaEnviada,
                "Enviada"
            ),
            new Transaccion(
                Guid.NewGuid(),
                otraCuentaId,
                cuentaId,
                new Dinero(50m, "USD"),
                TipoMovimiento.TransferenciaRecibida,
                "Recibida"
            ),
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(25m, "USD"),
                TipoMovimiento.Ajuste,
                "Ajuste"
            )
        };

        _transaccionRepositoryMock
            .Setup(r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaccionesEsperadas);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        var transacciones = resultado.ToList();
        transacciones.Should().HaveCount(3);
        transacciones.Should().Contain(t => t.Tipo == TipoMovimiento.TransferenciaEnviada);
        transacciones.Should().Contain(t => t.Tipo == TipoMovimiento.TransferenciaRecibida);
        transacciones.Should().Contain(t => t.Tipo == TipoMovimiento.Ajuste);
    }

    [Fact]
    public async Task EjecutarAsync_ConDiferentesMonedas_DeberiaRetornarTransacciones()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();

        var transaccionesEsperadas = new List<Transaccion>
        {
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(100m, "USD"),
                TipoMovimiento.TransferenciaEnviada,
                "USD"
            ),
            new Transaccion(
                Guid.NewGuid(),
                otraCuentaId,
                cuentaId,
                new Dinero(50m, "EUR"),
                TipoMovimiento.TransferenciaRecibida,
                "EUR"
            )
        };

        _transaccionRepositoryMock
            .Setup(r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaccionesEsperadas);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        var transacciones = resultado.ToList();
        transacciones.Should().HaveCount(2);
        transacciones.Should().Contain(t => t.Monto.Divisa == "USD");
        transacciones.Should().Contain(t => t.Monto.Divisa == "EUR");
    }

    [Fact]
    public async Task EjecutarAsync_ConTransaccionesConReferenciaNula_DeberiaRetornarTransacciones()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();

        var transaccionesEsperadas = new List<Transaccion>
        {
            new Transaccion(
                Guid.NewGuid(),
                cuentaId,
                otraCuentaId,
                new Dinero(100m, "USD"),
                TipoMovimiento.TransferenciaEnviada,
                null
            )
        };

        _transaccionRepositoryMock
            .Setup(r => r.ObtenerPorCuentaIdAsync(cuentaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaccionesEsperadas);

        // Act
        var resultado = await _useCase.EjecutarAsync(cuentaId);

        // Assert
        var transacciones = resultado.ToList();
        transacciones.Should().HaveCount(1);
        transacciones.First().Referencia.Should().BeNull();
    }
}
