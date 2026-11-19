using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork12
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };
            Console.WriteLine("=== 1. ПОРІВНЯННЯ ПРОДУКТИВНОСТІ: LINQ vs PLINQ ===");
            Console.WriteLine($"{"Розмір",-12} | {"LINQ (мс)",-12} | {"PLINQ (мс)",-12} | {"Прискорення",-12}");
            Console.WriteLine(new string('-', 56));

            foreach (int size in sizes)
            {
                var data = GenerateData(size);

                var sw = Stopwatch.StartNew();
                var linqRes = data
                    .Where(x => x % 2 != 0)
                    .Select(HeavyMathOperation)
                    .Count();
                sw.Stop();
                long linqMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var plinqRes = data
                    .AsParallel()
                    .Where(x => x % 2 != 0)
                    .Select(HeavyMathOperation)
                    .Count();
                sw.Stop();
                long plinqMs = sw.ElapsedMilliseconds;

                double speedup = plinqMs > 0 ? (double)linqMs / plinqMs : 0;
                Console.WriteLine($"{size,-12:N0} | {linqMs,-12} | {plinqMs,-12} | {speedup:F2}x");
            }

            Console.WriteLine("\n=== 2. ПРОБЛЕМА ПОБІЧНИХ ЕФЕКТІВ У PLINQ ===");
            DemonstrateSideEffects();
        }

        static List<int> GenerateData(int count)
        {
            var rnd = new Random(123);
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
                list.Add(rnd.Next(1, 100_000));
            return list;
        }

        static double HeavyMathOperation(int number)
        {
            double result = number;
            for (int i = 1; i <= 25; i++)
            {
                result = Math.Sqrt(result * i) + Math.Sin(result) * Math.Cos(i);
            }
            return result;
        }

        static void DemonstrateSideEffects()
        {
            var numbers = Enumerable.Range(1, 100_000).ToList();

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                int unsafeSum = 0;
                numbers.AsParallel().ForAll(n =>
                {
                    
                    unsafeSum += 1;
                });
                Console.WriteLine($"Небезпечний PLINQ (спроба {attempt}): очікувано 100000, отримано {unsafeSum}");
            }

            int safeSumLock = 0;
            object locker = new object();
            numbers.AsParallel().ForAll(n =>
            {
                lock (locker)
                {
                    safeSumLock += 1;
                }
            });
            Console.WriteLine($"Виправлено через lock: отримано {safeSumLock}");

            
            var safeBag = new ConcurrentBag<int>();
            numbers.AsParallel().Where(n => n % 2 == 0).ForAll(n => safeBag.Add(n));
            Console.WriteLine($"Виправлено через ConcurrentBag<T>: зібрано {safeBag.Count} парних елементів (очікувано 50000)");
        }
    }
}