using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PromoCodeFactory.DataAccess.Data
{
    public class PromoCodeFactoryDbContextFactory : IDesignTimeDbContextFactory<PromoCodeFactoryDbContext>
    {
        public PromoCodeFactoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PromoCodeFactoryDbContext>();
            optionsBuilder.UseSqlite("Data Source=PromoCodeFactory.db");
            return new PromoCodeFactoryDbContext(optionsBuilder.Options);
        }
    }
}