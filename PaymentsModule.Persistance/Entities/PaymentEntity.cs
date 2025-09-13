using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaymentsModule.Domain.Enums;

namespace PaymentsModule.Persistance.Entities;

public class PaymentEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime LastUpdatedAt { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; }
    
    [Required]
    public PaymentType PaymentType { get; set; }
    
    // Foreign Key to Order
    [Required]
    public Guid OrderId { get; set; }
    
    [ForeignKey("OrderId")]
    public virtual OrderEntity Order { get; set; } = null!;
    
    // Card Payment fields - nullable foreign key
    public Guid? CardId { get; set; }
    
    [ForeignKey("CardId")]
    public virtual CardEntity? Card { get; set; }
    
    // PIX Payment fields
    [StringLength(500)]
    public string? QrCode { get; set; }
    
    public DateTime? PixExpirationDate { get; set; }
    
    // Boleto Payment fields
    [StringLength(200)]
    public string? Barcode { get; set; }
    
    public DateOnly? DueDate { get; set; }
    
    [StringLength(200)]
    public string? DigitableLine { get; set; }
}