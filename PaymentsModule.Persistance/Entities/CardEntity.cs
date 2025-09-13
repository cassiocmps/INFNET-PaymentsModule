using System.ComponentModel.DataAnnotations;

namespace PaymentsModule.Persistance.Entities;

public class CardEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(19)]
    public string Number { get; set; } = string.Empty;
    
    [Required]
    [StringLength(4)]
    public string Cvv { get; set; } = string.Empty;
    
    [Required]
    [StringLength(7)] // MM/YYYY format
    public string ExpirationDate { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string HolderName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(14)] // CPF or CNPJ
    public string HolderDocument { get; set; } = string.Empty;
    
    // Navigation property
    public virtual ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
}