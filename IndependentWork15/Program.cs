using System;
using System.Net.Http;
using System.Threading;

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random;

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay));
            if (jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor));

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
            _random = new Random();
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = retryAttempt < _maxRetries && (shouldRetry == null || shouldRetry(ex));
                    if (!canRetry)
                    {
                        Console.WriteLine($"[ПОМИЛКА] Повторні спроби припинено (спроб виконано: {retryAttempt}). Виняток: {ex.Message}");
                        throw;
                    }

                    // Формула затримки згідно із завданням:
                    // delay = initialDelay * Math.Pow(2, retryAttempt) * (1 + random.NextDouble() * jitterFactor * 2 - jitterFactor)
                    double jitterMultiplier = 1.0 + (_random.NextDouble() * _jitterFactor * 2.0 - _jitterFactor);
                    double delayMs = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitterMultiplier;
                    TimeSpan delay = TimeSpan.FromMilliseconds(Math.Max(0, delayMs));

                    retryAttempt++;
                    Console.WriteLine($"[RETRY #{retryAttempt}/{_maxRetries}] Причина: {ex.Message} | Очікування: {delay.TotalMilliseconds:F0} мс...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        private static int _flakyCallCount = 0;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var retryPolicy = new RetryPolicy(
                maxRetries: 3,
                initialDelay: TimeSpan.FromMilliseconds(300),
                jitterFactor: 0.2
            );

            // Сценарій 1: Операція спочатку падає двічі, а на 3-й раз виконується успішно
            Console.WriteLine("=== Сценарій 1: Тимчасовий збій -> Успіх ===");
            try
            {
                string response = retryPolicy.Execute(
                    operation: () => UnstableNetworkCall(),
                    shouldRetry: ex => ex is HttpRequestException
                );
                Console.WriteLine($"[РЕЗУЛЬТАТ] Успішно отримано дані: {response}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧНО] Операція провалилась: {ex.Message}\n");
            }

            // Сценарій 2: Операція завжди падає -> вичерпання спроб і викидання винятку
            Console.WriteLine("=== Сценарій 2: Постійний збій -> Виняток після всіх спроб ===");
            try
            {
                retryPolicy.Execute<int>(
                    operation: () => AlwaysFailingOperation(),
                    shouldRetry: ex => ex is TimeoutException
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПЕРЕХОПЛЕНО В MAIN] Остаточний виняток ({ex.GetType().Name}): {ex.Message}\n");
            }
        }

        static string UnstableNetworkCall()
        {
            _flakyCallCount++;
            Console.WriteLine($"Виклик сервісу (спроба #{_flakyCallCount})...");
            if (_flakyCallCount <= 2)
                throw new HttpRequestException("503 Service Unavailable");

            return "{ \"status\": \"OK\", \"items\": 15 }";
        }

        static int AlwaysFailingOperation()
        {
            Console.WriteLine("Спроба підключення до недоступного вузла...");
            throw new TimeoutException("Перевищено час очікування відповіді від сервера.");
        }
    }
}