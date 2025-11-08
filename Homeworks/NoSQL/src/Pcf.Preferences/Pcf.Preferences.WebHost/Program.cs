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

// Настройка Swagger/OpenAPI
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "PromoCode Factory Preferences API";
    options.Version = "1.0";
    options.Description = "API для управления справочником предпочтений клиентов";
});

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

// Настройка Swagger UI
app.UseOpenApi();
app.UseSwaggerUi(options =>
{
    options.DocExpansion = "list";
    options.Path = "/swagger";
});

// Корневой маршрут - перенаправление на Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

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