using Microsoft.EntityFrameworkCore;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }

    public DbSet<CuentaEntity> Cuentas { get; set; }
    public DbSet<TransaccionEntity> Transacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de CuentaEntity
        modelBuilder.Entity<CuentaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.Balance).HasPrecision(19, 4);
        });

        // Configuración de TransaccionEntity
        modelBuilder.Entity<TransaccionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(19, 4);
            
            // Índices para optimizar búsquedas
            entity.HasIndex(e => e.OriginAccountId);
            entity.HasIndex(e => e.DestinationAccountId);
            entity.HasIndex(e => new { e.OriginAccountId, e.CreatedAt });
        });
    }
}
