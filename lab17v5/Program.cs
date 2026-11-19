using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab17v5
{
    public class FileProcessor
    {
        private int _attempts = 0;

        public void UpdateConfig(string path, string key, string value)
        {
            _attempts++;
            Console.WriteLine($"[FileProcessor] Спроба #{_attempts}: оновлення '{key}={value}' у файлі '{path}'...");

            if (_attempts <= 2)
            {
                throw new IOException($"Файл '{path}' заблокований іншим процесом.");
            }

            Console.WriteLine($"[FileProcessor] Успіх! Параметр '{key}={value}' записано у '{path}'.");
        }
    }

    public class NetworkClient
    {
        private int _attempts = 0;

        public bool SendConfigUpdate(string url, string configJson)
        {
            _attempts++;
            Console.WriteLine($"[NetworkClient] Спроба #{_attempts}: відправка конфігурації на '{url}'...");

            if (_attempts <= 3)
            {
                throw new HttpRequestException($"Помилка з'єднання з сервером '{url}' (503 Service Unavailable).");
            }

            Console.WriteLine($"[NetworkClient] Успіх! Сервер '{url}' прийняв конфігурацію: {configJson}");
            return true;
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(300);
            }

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = shouldRetry == null || shouldRetry(ex);

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] Виняток {ex.GetType().Name} не підлягає повтору (shouldRetry = false).");
                        throw;
                    }

                    if (retryAttempt >= retryCount)
                    {
                        Console.WriteLine($"[RetryHelper] Вичерпано ліміт спроб ({retryCount}). Помилка: {ex.Message}");
                        throw;
                    }

                    retryAttempt++;
                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

                    Console.WriteLine($"[RetryHelper] Помилка ({ex.GetType().Name}): {ex.Message}");
                    Console.WriteLine($"[RetryHelper] Повторна спроба #{retryAttempt}/{retryCount} через {delay.TotalMilliseconds} мс...\n");

                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Func<Exception, bool> retryFilter = ex =>
                ex is IOException || ex is HttpRequestException;

            Console.WriteLine("=== Сценарій 1: Оновлення локальної конфігурації (FileProcessor) ===");
            try
            {
                RetryHelper.ExecuteWithRetry<bool>(
                    operation: () =>
                    {
                        fileProcessor.UpdateConfig("appsettings.json", "MaxConnections", "100");
                        return true;
                    },
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryFilter
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Не вдалося оновити файл: {ex.Message}");
            }

            Console.WriteLine();

            Console.WriteLine("=== Сценарій 2: Відправка конфігурації на сервер (NetworkClient) ===");
            try
            {
                bool isSent = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.SendConfigUpdate("https://api.example.com/config", "{\"MaxConnections\":100}"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryFilter
                );
                Console.WriteLine($"[Main] Результат відправки по мережі: {isSent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Мережевий запит завершився невдачею: {ex.Message}");
            }

            Console.WriteLine();

            Console.WriteLine("=== Сценарій 3: Перевірка shouldRetry для ArgumentException (без повторів) ===");
            try
            {
                RetryHelper.ExecuteWithRetry<string>(
                    operation: () =>
                    {
                        Console.WriteLine("[Validation] Перевірка формату конфігурації...");
                        throw new ArgumentException("Некоректний ключ конфігурації.");
                    },
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryFilter
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Перехоплено критичну помилку без повторів: {ex.Message}");
            }
        }
    }
}
