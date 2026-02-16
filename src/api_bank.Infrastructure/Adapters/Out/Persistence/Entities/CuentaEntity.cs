using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_bank.Infrastructure.Adapters.Out.Persistence;

[Table("accounts")]
public class CuentaEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [Column("owner_id")]
    public Guid OwnerId { get; set; }

    [Required]
    [Column("balance", TypeName = "decimal(19,4)")]
    public decimal Balance { get; set; }

    [Required]
    [MaxLength(3)]
    [Column("currency")]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
