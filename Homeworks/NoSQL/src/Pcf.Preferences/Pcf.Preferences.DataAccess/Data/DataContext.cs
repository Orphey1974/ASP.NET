using Microsoft.EntityFrameworkCore;
using Pcf.Preferences.Core.Domain;

namespace Pcf.Preferences.DataAccess.Data
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Предпочтения
        /// </summary>
        public DbSet<Preference> Preferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка сущности Preference
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
            });
        }
    }
}
