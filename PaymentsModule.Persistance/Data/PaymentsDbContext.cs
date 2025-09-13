using Microsoft.EntityFrameworkCore;
using PaymentsModule.Persistance.Entities;

namespace PaymentsModule.Persistance.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<CardEntity> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Payment entity
        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.PaymentType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.QrCode)
                .HasMaxLength(500);

            entity.Property(e => e.Barcode)
                .HasMaxLength(200);

            entity.Property(e => e.DigitableLine)
                .HasMaxLength(200);

            // Configure relationships - One-to-One: Order can only have one Payment
            entity.HasOne(e => e.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<PaymentEntity>(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Card)
                .WithMany(c => c.Payments)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Order entity
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();
        });

        // Configure Card entity
        modelBuilder.Entity<CardEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Number)
                .HasMaxLength(19)
                .IsRequired();

            entity.Property(e => e.Cvv)
                .HasMaxLength(4)
                .IsRequired();

            entity.Property(e => e.ExpirationDate)
                .HasMaxLength(7)
                .IsRequired();

            entity.Property(e => e.HolderName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.HolderDocument)
                .HasMaxLength(14)
                .IsRequired();
        });
    }
}