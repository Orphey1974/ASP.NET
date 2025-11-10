using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.WebHost.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.GivingToCustomer.WebHost
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
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, EfDbInitializer>();

            // Настройка gRPC
            services.AddGrpc(options =>
            {
                // Разрешаем небезопасные соединения для локальной разработки
                options.EnableDetailedErrors = true;
            });

            // Включаем Server Reflection для работы с grpcurl и другими инструментами
            services.AddGrpcReflection();

            // Настройка MassTransit (всегда регистрируем для IPublishEndpoint)
            // Используем RabbitMQ если настроен, иначе in-memory bus
            var rabbitMqSettings = Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
            bool useRabbitMq = rabbitMqSettings != null && !string.IsNullOrWhiteSpace(rabbitMqSettings.Host);

            services.AddMassTransit(x =>
            {
                x.AddConsumer<Pcf.GivingToCustomer.Integration.Consumers.PartnerNotificationConsumer>();
                x.AddConsumer<Pcf.GivingToCustomer.Integration.Consumers.PromoCodeReceivedFromPartnerConsumer>();

                if (useRabbitMq)
                {
                    // Используем RabbitMQ если настроен
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var uri = $"rabbitmq://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}";
                        cfg.Host(uri, h =>
                        {
                            h.Username(rabbitMqSettings.Username);
                            h.Password(rabbitMqSettings.Password);

                            // Настройка таймаутов для предотвращения зависания при недоступности RabbitMQ
                            h.Heartbeat(TimeSpan.FromSeconds(10));
                            h.RequestedConnectionTimeout(TimeSpan.FromSeconds(10));
                        });

                        cfg.ConfigureEndpoints(context);

                        // Настройка retry политики для работы без RabbitMQ (не блокирует запуск)
                        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    });
                }
                else
                {
                    // Используем in-memory bus для разработки без RabbitMQ
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                }
            });

            // Настройка MassTransit Hosted Service
            services.Configure<MassTransitHostOptions>(options =>
            {
                if (useRabbitMq)
                {
                    // Не блокировать запуск при недоступности RabbitMQ
                    options.WaitUntilStarted = false;
                    options.StartTimeout = TimeSpan.FromSeconds(5);
                }
                else
                {
                    // In-memory bus запускается мгновенно
                    options.WaitUntilStarted = true;
                }
            });

            // HTTP клиент для микросервиса предпочтений
            services.AddHttpClient<IPreferencesGateway, PreferencesGateway>(client =>
            {
                client.BaseAddress = new Uri(Configuration["IntegrationSettings:PreferencesApiUrl"]);
            });
            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            // Настройка OpenAPI/Swagger
            // Примечание: RuntimeBinderException может возникать при генерации документации
            // Это не критично и не влияет на работу API
                   services.AddOpenApiDocument(options =>
                   {
                       options.Title = "PromoCode Factory Giving To Customer API Doc";
                       options.Version = "1.0";
                       // Примечание: RuntimeBinderException может возникать при генерации документации
                       // Это не критично и не влияет на работу API
                   });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            // Middleware для логирования ошибок (должен быть первым)
            app.UseMiddleware<Pcf.GivingToCustomer.WebHost.Middleware.ErrorLoggingMiddleware>();

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

            // Включаем HTTPS редирект для перенаправления HTTP на HTTPS
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Настройка gRPC endpoints
                endpoints.MapGrpcService<Pcf.GivingToCustomer.WebHost.Services.CustomersGrpcService>();
                // Включаем Server Reflection для работы с grpcurl (только для Development)
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
            });

            // Инициализация базы данных
            dbInitializer.InitializeDb();
        }
    }
}