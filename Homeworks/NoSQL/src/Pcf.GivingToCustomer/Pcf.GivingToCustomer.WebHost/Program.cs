using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pcf.GivingToCustomer.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Используем HTTPS для поддержки TLS (рекомендуется для gRPC)
                    webBuilder.UseUrls("https://localhost:8093", "http://localhost:8094");
                    webBuilder.UseKestrel(options =>
                    {
                        // Настройка для поддержки HTTP/2 с TLS для gRPC
                        // HTTP/2 с TLS (h2) работает лучше, чем без TLS (h2c)
                        options.ConfigureEndpointDefaults(listenOptions =>
                        {
                            // Используем Http1AndHttp2 - HTTP/2 будет использоваться с TLS
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });

                        // Настройка HTTPS endpoint
                        options.ListenLocalhost(8093, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            listenOptions.UseHttps(); // Использует development сертификат
                        });

                        // Настройка HTTP endpoint (для обратной совместимости)
                        options.ListenLocalhost(8094, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}