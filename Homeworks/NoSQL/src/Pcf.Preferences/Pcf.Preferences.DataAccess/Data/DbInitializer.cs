using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pcf.Preferences.Core.Domain;
using Pcf.Preferences.DataAccess.Data;

namespace Pcf.Preferences.DataAccess.Data
{
    /// <summary>
    /// Инициализатор базы данных
    /// </summary>
    public class DbInitializer
    {
        private readonly DataContext _context;

        public DbInitializer(DataContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            // Создаем базу данных, если она не существует
            await _context.Database.EnsureCreatedAsync();

            // Проверяем, есть ли данные
            if (await _context.Preferences.AnyAsync())
                return;

            // Добавляем тестовые данные
            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            var preferences = new List<Preference>
            {
                new Preference
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Электроника",
                    Description = "Предпочтения в области электронных устройств"
                },
                new Preference
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Одежда",
                    Description = "Предпочтения в области одежды и аксессуаров"
                },
                new Preference
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Книги",
                    Description = "Предпочтения в области литературы"
                },
                new Preference
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Спорт",
                    Description = "Предпочтения в области спорта и фитнеса"
                },
                new Preference
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Красота",
                    Description = "Предпочтения в области косметики и ухода"
                }
            };

            await _context.Preferences.AddRangeAsync(preferences);
            await _context.SaveChangesAsync();
        }
    }
}
