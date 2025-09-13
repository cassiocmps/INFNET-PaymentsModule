using System.ComponentModel.DataAnnotations;
using PaymentsModule.Domain.Enums;

namespace PaymentsModule.Persistance.Entities;

public class OrderEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    // Navigation property - One-to-One relationship
    public virtual PaymentEntity? Payment { get; set; }
}