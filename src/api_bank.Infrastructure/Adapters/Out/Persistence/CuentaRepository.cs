using api_bank.Domain.Entities;
using api_bank.Domain.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

public class CuentaRepository : ICuentaRepository
{
    private readonly BankDbContext _context;

    public CuentaRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Cuenta?> ObtenerPorIdAsync(Guid cuentaId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<CuentaEntity>()
            .FirstOrDefaultAsync(e => e.Id == cuentaId, cancellationToken);

        return entity == null ? null : CuentaMapper.ToDomain(entity);
    }

    public async Task<Cuenta?> ObtenerPorNumeroCuentaAsync(string numeroCuenta, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<CuentaEntity>()
            .FirstOrDefaultAsync(e => e.AccountNumber == numeroCuenta, cancellationToken);

        return entity == null ? null : CuentaMapper.ToDomain(entity);
    }

    public async Task<Cuenta> GuardarAsync(Cuenta cuenta, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<CuentaEntity>()
            .FirstOrDefaultAsync(e => e.Id == cuenta.CuentaId, cancellationToken);

        if (entity == null)
        {
            entity = CuentaMapper.ToEntity(cuenta);
            _context.Set<CuentaEntity>().Add(entity);
        }
        else
        {
            CuentaMapper.UpdateEntity(entity, cuenta);
            _context.Set<CuentaEntity>().Update(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return CuentaMapper.ToDomain(entity);
    }

    public async Task<bool> ExisteAsync(Guid cuentaId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CuentaEntity>()
            .AnyAsync(e => e.Id == cuentaId, cancellationToken);
    }
}
