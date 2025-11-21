using ContractMontlyClaims.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractMontlyClaims.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; } = null!;

        public DbSet<ContractMonthlyClaim> ContractMonthlyClaims { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                      .HasMaxLength(64);

                entity.Property(u => u.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(u => u.Email)
                      .HasMaxLength(256);

                entity.Property(u => u.Password)
                      .IsRequired();
            });

            modelBuilder.Entity<ContractMonthlyClaim>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.HoursWorked)
                      .HasColumnType("decimal(18,2)");
                entity.Property(c => c.HourlyRate)
                      .HasColumnType("decimal(18,2)");
                entity.Property(c => c.Notes)
                      .HasMaxLength(500);
            });
        }
    }
}
