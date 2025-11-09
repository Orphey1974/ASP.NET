using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pcf.GivingToCustomer.WebHost.Middleware
{
    /// <summary>
    /// Middleware для логирования ошибок и исключений в файл
    /// </summary>
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;
        private static readonly object _lockObject = new object();
        private static string _errorLogFilePath;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            // Инициализация пути к файлу логов при первом использовании
            if (_errorLogFilePath == null)
            {
                InitializeErrorLogFile();
            }
        }

        private void InitializeErrorLogFile()
        {
            try
            {
                // Определяем корень решения (на 5 уровней выше от bin/Debug/net8.0)
                // bin/Debug/net8.0 -> WebHost -> Pcf.GivingToCustomer -> src -> корень решения
                var currentDir = AppContext.BaseDirectory;
                var solutionRoot = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "..", "..", ".."));

                // Альтернативный способ: ищем папку src или .sln файл
                var directory = new DirectoryInfo(currentDir);
                while (directory != null && !File.Exists(Path.Combine(directory.FullName, "src", "Pcf.sln")))
                {
                    directory = directory.Parent;
                }

                if (directory != null)
                {
                    solutionRoot = directory.FullName;
                }

                var errorsDirectory = Path.Combine(solutionRoot, "errors");

                // Создаем папку errors, если её нет
                if (!Directory.Exists(errorsDirectory))
                {
                    Directory.CreateDirectory(errorsDirectory);
                }

                // Формируем имя файла с текущей датой и временем
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd__HH_mm_ss");
                _errorLogFilePath = Path.Combine(errorsDirectory, $"errors_{timestamp}.txt");

                // Записываем заголовок в файл
                var header = $"{new string('=', 80)}\r\n" +
                            $"ERROR LOG SESSION STARTED\r\n" +
                            $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n" +
                            $"Application: Pcf.GivingToCustomer.WebHost\r\n" +
                            $"{new string('=', 80)}\r\n\r\n";

                File.AppendAllText(_errorLogFilePath, header, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации файла логов ошибок");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex, context);
                throw; // Пробрасываем исключение дальше для обработки стандартными механизмами
            }
        }

        private Task LogErrorAsync(Exception exception, HttpContext context)
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"{new string('-', 80)}");
                logEntry.AppendLine($"ERROR OCCURRED: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                logEntry.AppendLine($"{new string('-', 80)}");
                logEntry.AppendLine($"Exception Type: {exception.GetType().FullName}");
                logEntry.AppendLine($"Exception Message: {exception.Message}");
                logEntry.AppendLine($"Stack Trace:");
                logEntry.AppendLine(exception.StackTrace);

                // Добавляем информацию о внутренних исключениях
                var innerException = exception.InnerException;
                int innerLevel = 1;
                while (innerException != null)
                {
                    logEntry.AppendLine();
                    logEntry.AppendLine($"Inner Exception #{innerLevel}:");
                    logEntry.AppendLine($"  Type: {innerException.GetType().FullName}");
                    logEntry.AppendLine($"  Message: {innerException.Message}");
                    logEntry.AppendLine($"  Stack Trace: {innerException.StackTrace}");
                    innerException = innerException.InnerException;
                    innerLevel++;
                }

                // Добавляем информацию о HTTP запросе
                if (context != null)
                {
                    logEntry.AppendLine();
                    logEntry.AppendLine("HTTP Request Information:");
                    logEntry.AppendLine($"  Method: {context.Request.Method}");
                    logEntry.AppendLine($"  Path: {context.Request.Path}");
                    logEntry.AppendLine($"  QueryString: {context.Request.QueryString}");
                    logEntry.AppendLine($"  Remote IP: {context.Connection.RemoteIpAddress}");
                    if (context.Request.Headers != null)
                    {
                        logEntry.AppendLine("  Headers:");
                        foreach (var header in context.Request.Headers)
                        {
                            logEntry.AppendLine($"    {header.Key}: {string.Join(", ", header.Value)}");
                        }
                    }
                }

                logEntry.AppendLine();
                logEntry.AppendLine($"{new string('=', 80)}");
                logEntry.AppendLine();

                // Записываем в файл (с блокировкой для потокобезопасности)
                lock (_lockObject)
                {
                    if (!string.IsNullOrEmpty(_errorLogFilePath))
                    {
                        File.AppendAllText(_errorLogFilePath, logEntry.ToString(), Encoding.UTF8);
                    }
                }

                // Также логируем через стандартный механизм логирования
                _logger.LogError(exception, "Произошла ошибка при обработке запроса {Path}", context?.Request.Path);
            }
            catch (Exception logEx)
            {
                // Если не удалось записать в файл, логируем через стандартный механизм
                _logger.LogError(logEx, "Ошибка при записи ошибки в файл логов");
                _logger.LogError(exception, "Исходная ошибка: {Message}", exception.Message);
            }

            return Task.CompletedTask;
        }
    }
}

