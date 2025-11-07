using Microsoft.EntityFrameworkCore;
using Pcf.Preferences.Core.Abstractions.Repositories;
using Pcf.Preferences.Core.Abstractions.Services;
using Pcf.Preferences.Core.Domain;
using Pcf.Preferences.DataAccess.Data;
using Pcf.Preferences.DataAccess.Repositories;
using Pcf.Preferences.DataAccess.Services;

var builder = WebApplication.CreateBuilder(args);

// Явно указываем порт для предотвращения конфликтов
builder.WebHost.UseUrls("http://localhost:8094");

// Add services to the container.
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRepository<Preference>, Repository<Preference>>();

// Cache - используем in-memory кэш для локальной разработки
if (builder.Configuration.GetValue<bool>("CacheSettings:UseRedis"))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
}
else
{
    builder.Services.AddMemoryCache();
}

// Services
if (builder.Configuration.GetValue<bool>("CacheSettings:UseRedis"))
{
    builder.Services.AddScoped<IPreferenceCacheService, PreferenceCacheService>();
}
else
{
    builder.Services.AddScoped<IPreferenceCacheService, MemoryPreferenceCacheService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
// Отключаем HTTPS редирект для локальной разработки
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Initialize database
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var initializer = new DbInitializer(context);
        await initializer.InitializeAsync();
        Console.WriteLine("Database initialized successfully");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing database: {ex.Message}");
    throw;
}

app.Run();