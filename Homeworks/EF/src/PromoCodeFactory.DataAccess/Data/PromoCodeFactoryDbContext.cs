using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class PromoCodeFactoryDbContext : DbContext
    {
        public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerLimit> PartnerLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.HasOne(e => e.Role)
                      .WithMany()
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Name).HasMaxLength(100).IsRequired();
                entity.Property(r => r.Description).HasMaxLength(255);
            });

            // Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(c => c.LastName).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Email).HasMaxLength(255).IsRequired();
            });

            // Preference
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            });

            // Partner
            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.ContactEmail).HasMaxLength(255);
                entity.Property(p => p.ContactPhone).HasMaxLength(20);
                entity.HasOne(p => p.PartnerManager)
                      .WithMany(e => e.ManagedPartners)
                      .HasForeignKey(p => p.PartnerManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PartnerLimit
            modelBuilder.Entity<PartnerLimit>(entity =>
            {
                entity.Property(pl => pl.Limit).IsRequired();
                entity.Property(pl => pl.CurrentCount).IsRequired();
                entity.Property(pl => pl.StartDate).IsRequired();
                entity.Property(pl => pl.EndDate).IsRequired();
                entity.HasOne(pl => pl.Partner)
                      .WithMany(p => p.PartnerLimits)
                      .HasForeignKey(pl => pl.PartnerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PromoCode
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.Property(p => p.Code).HasMaxLength(50).IsRequired();
                entity.Property(p => p.ServiceInfo).HasMaxLength(255);
                entity.Property(p => p.PartnerName).HasMaxLength(100);
                entity.HasOne(p => p.PartnerManager)
                      .WithMany()
                      .HasForeignKey(p => p.PartnerManagerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(p => p.Partner)
                      .WithMany(p => p.PromoCodes)
                      .HasForeignKey(p => p.PartnerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(p => p.Preference)
                      .WithMany()
                      .HasForeignKey(p => p.PreferenceId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Customer)
                      .WithMany(c => c.PromoCodes)
                      .HasForeignKey(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CustomerPreference (Many-to-Many)
            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasOne(cp => cp.Customer)
                      .WithMany(c => c.CustomerPreferences)
                      .HasForeignKey(cp => cp.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Preference)
                      .WithMany(p => p.CustomerPreferences)
                      .HasForeignKey(cp => cp.PreferenceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}