using api_bank.Domain.Entities;
using api_bank.Domain.Enums;
using api_bank.Domain.Ports.Out;
using api_bank.Domain.ValueObjects;
using api_bank.Infrastructure.Adapters.Out.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace api_bank.Infrastructure.Tests.Adapters.Out.Persistence;

public class CuentaRepositoryTests : IDisposable
{
    private readonly BankDbContext _context;
    private readonly ICuentaRepository _repository;

    public CuentaRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BankDbContext(options);
        _repository = new CuentaRepository(_context);
    }

    [Fact]
    public async Task GuardarAsync_CuentaNueva_DeberiaPersistirCorrectamente()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldo = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(cuentaId, "ACC-001", saldo, titularId, EstadoCuenta.Activa);

        // Act
        var resultado = await _repository.GuardarAsync(cuenta);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(cuentaId, resultado.CuentaId);
        Assert.Equal("ACC-001", resultado.NumeroCuenta);
        
        var cuentaPersistida = await _context.Set<CuentaEntity>().FirstOrDefaultAsync(c => c.Id == cuentaId);
        Assert.NotNull(cuentaPersistida);
        Assert.Equal(1000m, cuentaPersistida.Balance);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuentaExistente_DeberiaRetornarCuenta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldo = new Dinero(500m, "USD");
        var cuenta = new Cuenta(cuentaId, "ACC-002", saldo, titularId, EstadoCuenta.Activa);
        await _repository.GuardarAsync(cuenta);

        // Act
        var resultado = await _repository.ObtenerPorIdAsync(cuentaId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(cuentaId, resultado.CuentaId);
        Assert.Equal("ACC-002", resultado.NumeroCuenta);
        Assert.Equal(500m, resultado.Saldo.Cantidad);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuentaNoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var cuentaIdInexistente = Guid.NewGuid();

        // Act
        var resultado = await _repository.ObtenerPorIdAsync(cuentaIdInexistente);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerPorNumeroCuentaAsync_CuentaExistente_DeberiaRetornarCuenta()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldo = new Dinero(750m, "EUR");
        var cuenta = new Cuenta(cuentaId, "ACC-003", saldo, titularId, EstadoCuenta.Activa);
        await _repository.GuardarAsync(cuenta);

        // Act
        var resultado = await _repository.ObtenerPorNumeroCuentaAsync("ACC-003");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(cuentaId, resultado.CuentaId);
        Assert.Equal("ACC-003", resultado.NumeroCuenta);
        Assert.Equal(750m, resultado.Saldo.Cantidad);
    }

    [Fact]
    public async Task ExisteAsync_CuentaExistente_DeberiaRetornarTrue()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldo = new Dinero(300m, "USD");
        var cuenta = new Cuenta(cuentaId, "ACC-004", saldo, titularId, EstadoCuenta.Activa);
        await _repository.GuardarAsync(cuenta);

        // Act
        var resultado = await _repository.ExisteAsync(cuentaId);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task ExisteAsync_CuentaNoExistente_DeberiaRetornarFalse()
    {
        // Arrange
        var cuentaIdInexistente = Guid.NewGuid();

        // Act
        var resultado = await _repository.ExisteAsync(cuentaIdInexistente);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task GuardarAsync_CuentaModificada_DeberiaActualizarSaldo()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var titularId = Guid.NewGuid();
        var saldoInicial = new Dinero(1000m, "USD");
        var cuenta = new Cuenta(cuentaId, "ACC-005", saldoInicial, titularId, EstadoCuenta.Activa);
        await _repository.GuardarAsync(cuenta);

        // Act - Modificar saldo
        var montoDebito = new Dinero(200m, "USD");
        cuenta.Debitar(montoDebito);
        await _repository.GuardarAsync(cuenta);

        // Assert
        var cuentaActualizada = await _repository.ObtenerPorIdAsync(cuentaId);
        Assert.NotNull(cuentaActualizada);
        Assert.Equal(800m, cuentaActualizada.Saldo.Cantidad);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
