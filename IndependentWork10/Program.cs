using System;
using System.Collections.Generic;

namespace lab9_self_vN
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            foreach (var item in collection)
            {
                yield return mapper(item);
            }
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            using var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Колекція порожня.");

            T maxItem = enumerator.Current;
            TKey maxKey = keySelector(maxItem);

            while (enumerator.MoveNext())
            {
                TKey currentKey = keySelector(enumerator.Current);
                if (currentKey.CompareTo(maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxItem = enumerator.Current;
                }
            }
            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Колекція 1: List<Product>
            var products = new List<Product>
            {
                new() { Name = "Ноутбук", Price = 32000 },
                new() { Name = "Мишка", Price = 650 },
                new() { Name = "Монітор", Price = 12500 },
                new() { Name = "Клавіатура", Price = 2100 }
            };

            Console.WriteLine("=== 1. Колекція Product ===");
            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви: " + string.Join(", ", names));

            var maxProduct = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий: {maxProduct.Name} ({maxProduct.Price} грн)");

            var expensive = CollectionUtils.Filter(products, p => p.Price > 2000);
            Console.WriteLine("Товари дорожчі за 2000 грн:");
            CollectionUtils.ForEach(expensive, p => Console.WriteLine($" - {p.Name}: {p.Price} грн"));

            // Колекція 2: List<int>
            var numbers = new List<int> { 3, 14, 7, 22, 9, 45, 18 };
            Console.WriteLine("\n=== 2. Колекція int ===");
            var squares = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати: " + string.Join(", ", squares));
            Console.WriteLine("Максимум: " + CollectionUtils.MaxBy(numbers, n => n));
            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.Write("Парні числа: ");
            CollectionUtils.ForEach(evenNumbers, n => Console.Write(n + " "));
            Console.WriteLine();

            // Колекція 3: List<string>
            var cities = new List<string> { "Рівне", "Київ", "Львів", "Івано-Франківськ", "Одеса" };
            Console.WriteLine("\n=== 3. Колекція string ===");
            var upperCities = CollectionUtils.Map(cities, c => c.ToUpper());
            Console.WriteLine("Верхній регістр: " + string.Join(", ", upperCities));
            Console.WriteLine("Найдовша назва: " + CollectionUtils.MaxBy(cities, c => c.Length));
            var longCities = CollectionUtils.Filter(cities, c => c.Length > 5);
            Console.WriteLine("Назви довші за 5 символів: " + string.Join(", ", longCities));
        }
    }
}