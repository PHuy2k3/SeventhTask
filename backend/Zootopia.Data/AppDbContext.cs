using Microsoft.EntityFrameworkCore;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<SocialInsuranceRegistration> SocialInsuranceRegistrations => Set<SocialInsuranceRegistration>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Citizen>(e =>
        {
            // ✅ báo EF biết table có trigger để tránh OUTPUT clause gây lỗi
            e.ToTable("Citizens", "dbo", tb => tb.HasTrigger("TR_Citizens"));

            e.HasKey(x => x.Id);

            e.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            e.Property(x => x.NationalId).IsRequired().HasMaxLength(20);
            e.Property(x => x.AddressText).HasMaxLength(500);
            e.Property(x => x.PasswordHash).HasMaxLength(200);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.DateOfBirth).HasColumnType("date");
            e.Property(x => x.CreatedAt).HasColumnType("datetime2(0)");
            e.Property(x => x.UpdatedAt).HasColumnType("datetime2(0)");

            e.HasIndex(x => x.NationalId);
        });
        modelBuilder.Entity<SocialInsuranceRegistration>(e =>
        {
            e.ToTable("SocialInsuranceRegistrations", "dbo");
            e.HasKey(x => x.Id);

            e.Property(x => x.SocialInsuranceNumber).HasMaxLength(50);
            e.Property(x => x.SocialInsuranceProvider).HasMaxLength(120);
            e.Property(x => x.Status).HasMaxLength(30);
            e.Property(x => x.Note).HasMaxLength(500);
            e.Property(x => x.ReviewedBy).HasMaxLength(100);
            e.Property(x => x.CreatedAt).HasColumnType("datetime2(0)");
            e.Property(x => x.ReviewedAt).HasColumnType("datetime2(0)");

            e.HasOne(x => x.Citizen)
                .WithMany()
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.Status);
        });
    }
}
