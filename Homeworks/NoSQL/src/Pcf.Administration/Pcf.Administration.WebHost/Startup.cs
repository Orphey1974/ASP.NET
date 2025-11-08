using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.DataAccess.Data;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.Administration.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x=>
                x.SuppressAsyncSuffixInActionNames = false);

            // Настройка MongoDB
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<MongoDbInitializer>();

            // Регистрация MongoDB репозиториев
            services.AddScoped<IMongoRepository<Employee>>(provider =>
            {
                var mongoContext = provider.GetRequiredService<MongoDbContext>();
                return new MongoEmployeeRepository(mongoContext.Employees, mongoContext.Roles);
            });

            services.AddScoped<IMongoRepository<Role>>(provider =>
            {
                var mongoContext = provider.GetRequiredService<MongoDbContext>();
                return new MongoRoleRepository(mongoContext.Roles);
            });

            // Настройка RabbitMQ
            var rabbitMqSettings = Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
            if (rabbitMqSettings != null)
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<Pcf.Administration.DataAccess.Consumers.PromoCodeReceivedFromPartnerConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var uri = $"rabbitmq://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}";
                        cfg.Host(uri, h =>
                        {
                            h.Username(rabbitMqSettings.Username);
                            h.Password(rabbitMqSettings.Password);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });
            }

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Administration API Doc (MongoDB)";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MongoDbInitializer mongoDbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
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

            // Инициализация MongoDB
            mongoDbInitializer.InitializeAsync().Wait();
        }
    }
}