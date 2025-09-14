using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentsModule.Persistance.Entities;

public class RefundEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime RefundDate { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    // Foreign Key to Payment (1-1 relationship)
    [Required]
    public Guid PaymentId { get; set; }
    
    [ForeignKey("PaymentId")]
    public virtual PaymentEntity Payment { get; set; } = null!;
}