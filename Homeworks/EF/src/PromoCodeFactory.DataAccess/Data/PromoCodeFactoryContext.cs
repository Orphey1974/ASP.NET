using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    /// <summary>
    /// Контекст базы данных для Entity Framework
    /// </summary>
    public class PromoCodeFactoryContext : DbContext
    {
        public PromoCodeFactoryContext(DbContextOptions<PromoCodeFactoryContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Настройка Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();

                // Связь с Role
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Employees)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            });

            // Настройка Preference
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });

            // Настройка CustomerPreference (Many-to-Many)
            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Связь с Customer
                entity.HasOne(cp => cp.Customer)
                    .WithMany(c => c.CustomerPreferences)
                    .HasForeignKey(cp => cp.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь с Preference
                entity.HasOne(cp => cp.Preference)
                    .WithMany(p => p.CustomerPreferences)
                    .HasForeignKey(cp => cp.PreferenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка PromoCode
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ServiceInfo).HasMaxLength(500);
                entity.Property(e => e.PartnerName).HasMaxLength(200);

                // Связь с Employee (PartnerManager)
                entity.HasOne(pc => pc.PartnerManager)
                    .WithMany(e => e.PromoCodes)
                    .HasForeignKey(pc => pc.PartnerManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с Preference
                entity.HasOne(pc => pc.Preference)
                    .WithMany(p => p.PromoCodes)
                    .HasForeignKey(pc => pc.PreferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с Customer
                entity.HasOne(pc => pc.Customer)
                    .WithMany(c => c.PromoCodes)
                    .HasForeignKey(pc => pc.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}