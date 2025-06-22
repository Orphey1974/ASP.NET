using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Domain.PartnerManagement;
using System;
using System.Linq;

namespace PromoCodeFactory.DataAccess.Data
{
    /// <summary>
    /// Сервис для инициализации базы данных
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Инициализирует базу данных тестовыми данными
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public static async Task InitializeAsync(PromoCodeFactoryDbContext context)
        {
            // Удаляем существующую базу данных
            await context.Database.EnsureDeletedAsync();

            // Создаем новую базу данных
            await context.Database.EnsureCreatedAsync();

            // Проверяем, есть ли уже данные
            if (await context.Roles.AnyAsync())
            {
                return; // Данные уже есть
            }

            // Добавляем роли
            var roles = FakeDataFactory.Roles.ToList();
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            // Добавляем сотрудников
            var employees = FakeDataFactory.Employees.ToList();
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // Добавляем предпочтения
            var preferences = FakeDataFactory.Preferences.ToList();
            await context.Preferences.AddRangeAsync(preferences);
            await context.SaveChangesAsync();

            // Добавляем клиентов
            var customers = FakeDataFactory.Customers.ToList();
            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            // Добавляем связи клиентов с предпочтениями
            var customerPreferences = FakeDataFactory.CustomerPreferences.ToList();
            await context.CustomerPreferences.AddRangeAsync(customerPreferences);
            await context.SaveChangesAsync();

            // Добавляем партнеров
            var partners = FakeDataFactory.Partners.ToList();
            await context.Partners.AddRangeAsync(partners);
            await context.SaveChangesAsync();

            // Добавляем лимиты партнеров
            var partnerLimits = FakeDataFactory.PartnerLimits.ToList();
            await context.PartnerLimits.AddRangeAsync(partnerLimits);
            await context.SaveChangesAsync();

            // Добавляем промокоды
            var promoCodes = FakeDataFactory.PromoCodes.ToList();
            await context.PromoCodes.AddRangeAsync(promoCodes);
            await context.SaveChangesAsync();
        }
    }
}