using api_bank.Domain.Entities;
using api_bank.Domain.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

public class TransaccionRepository : ITransaccionRepository
{
    private readonly BankDbContext _context;

    public TransaccionRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Transaccion> GuardarAsync(Transaccion transaccion, CancellationToken cancellationToken = default)
    {
        var entity = TransaccionMapper.ToEntity(transaccion);
        _context.Set<TransaccionEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return TransaccionMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaIdAsync(Guid cuentaId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Set<TransaccionEntity>()
            .Where(t => t.OriginAccountId == cuentaId || t.DestinationAccountId == cuentaId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(TransaccionMapper.ToDomain);
    }
}
