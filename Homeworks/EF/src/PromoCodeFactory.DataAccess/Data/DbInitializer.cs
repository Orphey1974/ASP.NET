using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PromoCodeFactoryDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(FakeDataFactory.Roles);
                context.SaveChanges();
            }

            if (!context.Employees.Any())
            {
                context.Employees.AddRange(FakeDataFactory.Employees);
                context.SaveChanges();
            }

            if (!context.Preferences.Any())
            {
                context.Preferences.AddRange(FakeDataFactory.Preferences);
                context.SaveChanges();
            }

            if (!context.Customers.Any())
            {
                context.Customers.AddRange(FakeDataFactory.Customers);
                context.SaveChanges();
            }

            if (!context.PromoCodes.Any())
            {
                context.PromoCodes.AddRange(FakeDataFactory.PromoCodes);
                context.SaveChanges();
            }
        }
    }
}