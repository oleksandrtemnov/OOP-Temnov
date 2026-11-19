using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        private static int _dbAttempts = 0;
        private static int _brokerAttempts = 0;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            RunDatabaseRetryScenario();
            RunMessageBrokerCircuitBreakerScenario();
            RunReportGenerationTimeoutScenario();
        }

        // Сценарій 1: Тимчасова втрата з'єднання з БД (Retry з Exponential Backoff)
        static void RunDatabaseRetryScenario()
        {
            Console.WriteLine("=== Сценарій 1: Підключення до БД (Retry) ===");

            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[LOG Retry] Спроба #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                        return default;
                    }
                })
                .Build();

            try
            {
                string data = pipeline.Execute(() => QueryDatabase());
                Console.WriteLine($"[УСПІХ] Отримано: {data}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА] Усі спроби вичерпано: {ex.Message}\n");
            }
        }

        static string QueryDatabase()
        {
            _dbAttempts++;
            Console.WriteLine($"Виконання SQL-запиту (спроба #{_dbAttempts})...");
            if (_dbAttempts <= 2)
                throw new TimeoutException("Connection pool timeout (БД перевантажена).");

            return "Список замовлень (42 записи)";
        }

        // Сценарій 2: Перевантажений брокер повідомлень (Circuit Breaker)
        static void RunMessageBrokerCircuitBreakerScenario()
        {
            Console.WriteLine("=== Сценарій 2: Черга повідомлень (Circuit Breaker) ===");

            var pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(2),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[LOG CircuitBreaker] Ланцюг РОЗІМКНУТО на {args.BreakDuration.TotalSeconds} с через збої!");
                        return default;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine("[LOG CircuitBreaker] Ланцюг ЗАМКНУТО. Роботу відновлено.");
                        return default;
                    }
                })
                .Build();

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    pipeline.Execute(() => PublishMessage($"Повідомлення #{i}"));
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"[ШВИДКА ВІДМОВА] Повідомлення #{i} відхилено без звернення до брокера (ланцюг відкритий).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ЗБІЙ] Повідомлення #{i}: {ex.Message}");
                }

                if (i == 3)
                {
                    Console.WriteLine("Очікуємо 2.2 секунди для закриття Circuit Breaker...");
                    Thread.Sleep(2200);
                }
            }
            Console.WriteLine();
        }

        static void PublishMessage(string msg)
        {
            _brokerAttempts++;
            if (_brokerAttempts <= 2)
                throw new IOException("RabbitMQ broker unreachable.");

            Console.WriteLine($"[УСПІХ] Відправлено: {msg}");
        }

        // Сценарій 3: Занадто довга генерація звіту (Timeout + Fallback)
        static void RunReportGenerationTimeoutScenario()
        {
            Console.WriteLine("=== Сценарій 3: Генерація звіту (Timeout + Fallback) ===");

            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddFallback(new Polly.Fallback.FallbackStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                    FallbackAction = args =>
                    {
                        Console.WriteLine("[LOG Fallback] Перевищено ліміт очікування. Повертаємо кешований звіт.");
                        return Outcome.FromResultAsValueTask("Кешований звіт за вчора");
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(400),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"[LOG Timeout] Операція перервана після {args.Timeout.TotalMilliseconds} мс!");
                        return default;
                    }
                })
                .Build();

            string report = pipeline.Execute(ct =>
            {
                Console.WriteLine("Запуск важкої генерації звіту (імітація 1000 мс)...");
                for (int i = 0; i < 10; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
                return "Свіжий звіт";
            });

            Console.WriteLine($"[РЕЗУЛЬТАТ] Отримано: {report}\n");
        }
    }
}