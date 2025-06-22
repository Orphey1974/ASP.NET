using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Настройка Entity Framework с SQLite
            services.AddDbContext<PromoCodeFactoryContext>(options =>
                options.UseSqlite("Data Source=PromoCodeFactory.db"));

            // Регистрация репозиториев с Entity Framework
            services.AddScoped(typeof(IRepository<Employee>), typeof(EfRepository<Employee>));
            services.AddScoped(typeof(IRepository<Role>), typeof(EfRepository<Role>));
            services.AddScoped(typeof(IRepository<Preference>), typeof(EfRepository<Preference>));
            services.AddScoped(typeof(IRepository<Customer>), typeof(EfRepository<Customer>));
            services.AddScoped(typeof(IRepository<CustomerPreference>), typeof(EfRepository<CustomerPreference>));
            services.AddScoped(typeof(IRepository<PromoCode>), typeof(EfRepository<PromoCode>));

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Инициализация базы данных
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<PromoCodeFactoryContext>();
                DatabaseInitializer.InitializeAsync(context).Wait();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}