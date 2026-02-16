using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

[Table("transactions")]
public class TransaccionEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("origin_account_id")]
    public Guid OriginAccountId { get; set; }

    [Required]
    [Column("destination_account_id")]
    public Guid DestinationAccountId { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(19,4)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(3)]
    [Column("currency")]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("transaction_type")]
    public string TransactionType { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
