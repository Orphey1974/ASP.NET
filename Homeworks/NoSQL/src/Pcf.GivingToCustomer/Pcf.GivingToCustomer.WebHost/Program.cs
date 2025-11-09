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
                    webBuilder.UseUrls("http://localhost:8093");
                    webBuilder.UseKestrel(options =>
                    {
                        // Настройка для поддержки HTTP/2 через HTTP (без TLS) для gRPC
                        // Примечание: HTTP/2 без TLS (h2c) поддерживается ограниченно
                        // Но gRPC требует HTTP/2 для корректной работы
                        options.ConfigureEndpointDefaults(listenOptions =>
                        {
                            // Используем Http1AndHttp2 - предупреждение можно игнорировать
                            // gRPC будет пытаться использовать HTTP/2, при отсутствии TLS будет HTTP/1.1
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}