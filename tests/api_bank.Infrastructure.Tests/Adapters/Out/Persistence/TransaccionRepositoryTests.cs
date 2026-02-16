using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;
using api_bank.Infrastructure.Adapters.Out.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace api_bank.Infrastructure.Tests.Adapters.Out.Persistence;

public class TransaccionRepositoryTests : IDisposable
{
    private readonly BankDbContext _context;
    private readonly ITransaccionRepository _repository;

    public TransaccionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BankDbContext(options);
        _repository = new TransaccionRepository(_context);
    }

    [Fact]
    public async Task GuardarAsync_TransaccionNueva_DeberiaPersistirCorrectamente()
    {
        // Arrange
        var transaccionId = Guid.NewGuid();
        var cuentaOrigenId = Guid.NewGuid();
        var cuentaDestinoId = Guid.NewGuid();
        var monto = new Dinero(100m, "USD");
        var transaccion = new Transaccion(
            transaccionId,
            cuentaOrigenId,
            cuentaDestinoId,
            monto,
            TipoMovimiento.TransferenciaEnviada,
            "Test transfer");

        // Act
        var resultado = await _repository.GuardarAsync(transaccion);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(transaccionId, resultado.TransaccionId);
        Assert.Equal(cuentaOrigenId, resultado.CuentaOrigenId);
        Assert.Equal(cuentaDestinoId, resultado.CuentaDestinoId);
        Assert.Equal(100m, resultado.Monto.Cantidad);
        
        var transaccionPersistida = await _context.Set<TransaccionEntity>().FirstOrDefaultAsync(t => t.Id == transaccionId);
        Assert.NotNull(transaccionPersistida);
        Assert.Equal(100m, transaccionPersistida.Amount);
    }

    [Fact]
    public async Task ObtenerPorCuentaIdAsync_CuentaConTransacciones_DeberiaRetornarTodasLasTransacciones()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();
        var monto = new Dinero(50m, "USD");

        var transaccion1 = new Transaccion(
            Guid.NewGuid(),
            cuentaId,
            otraCuentaId,
            monto,
            TipoMovimiento.TransferenciaEnviada,
            "Transfer 1");
        
        var transaccion2 = new Transaccion(
            Guid.NewGuid(),
            otraCuentaId,
            cuentaId,
            monto,
            TipoMovimiento.TransferenciaRecibida,
            "Transfer 2");

        await _repository.GuardarAsync(transaccion1);
        await _repository.GuardarAsync(transaccion2);

        // Act
        var resultado = await _repository.ObtenerPorCuentaIdAsync(cuentaId);

        // Assert
        Assert.NotNull(resultado);
        var transacciones = resultado.ToList();
        Assert.Equal(2, transacciones.Count);
        Assert.Contains(transacciones, t => t.TransaccionId == transaccion1.TransaccionId);
        Assert.Contains(transacciones, t => t.TransaccionId == transaccion2.TransaccionId);
    }

    [Fact]
    public async Task ObtenerPorCuentaIdAsync_CuentaSinTransacciones_DeberiaRetornarListaVacia()
    {
        // Arrange
        var cuentaIdSinTransacciones = Guid.NewGuid();

        // Act
        var resultado = await _repository.ObtenerPorCuentaIdAsync(cuentaIdSinTransacciones);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObtenerPorCuentaIdAsync_DeberiaIncluirTransaccionesComoOrigenYDestino()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var otraCuentaId = Guid.NewGuid();
        var monto = new Dinero(75m, "EUR");

        // Transacción donde cuentaId es origen
        var transaccionOrigen = new Transaccion(
            Guid.NewGuid(),
            cuentaId,
            otraCuentaId,
            monto,
            TipoMovimiento.TransferenciaEnviada,
            "As origin");

        // Transacción donde cuentaId es destino
        var transaccionDestino = new Transaccion(
            Guid.NewGuid(),
            otraCuentaId,
            cuentaId,
            monto,
            TipoMovimiento.TransferenciaRecibida,
            "As destination");

        await _repository.GuardarAsync(transaccionOrigen);
        await _repository.GuardarAsync(transaccionDestino);

        // Act
        var resultado = await _repository.ObtenerPorCuentaIdAsync(cuentaId);

        // Assert
        var transacciones = resultado.ToList();
        Assert.Equal(2, transacciones.Count);
        Assert.Contains(transacciones, t => t.CuentaOrigenId == cuentaId);
        Assert.Contains(transacciones, t => t.CuentaDestinoId == cuentaId);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
