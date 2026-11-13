using System;
using System.Collections.Generic;

namespace lab13v5
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString() => $"{Name} ({Age} р.)";
    }

    public static class FunctionalUtils
    {
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate initialValue, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            TAccumulate result = initialValue;
            foreach (var item in list)
            {
                result = accumulator(result, item);
            }
            return result;
        }

        public static void ForEach<T>(List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static bool All<T>(List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static T? FirstOrDefault<T>(List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №13 (ВАРІАНТ 5) ===\n");

            Console.WriteLine("--- 1. Тестування з int ---");
            List<int> numbers = new List<int> { 2, 4, 6, 8, 10 };

            Console.Write("Початковий список (ForEach): ");
            FunctionalUtils.ForEach(numbers, n => Console.Write($"{n} "));
            Console.WriteLine();

            var multiplied = FunctionalUtils.Map(numbers, n => n * 10);
            Console.Write("Помножені на 10 (Map): ");
            FunctionalUtils.ForEach(multiplied, n => Console.Write($"{n} "));
            Console.WriteLine();

            int product = FunctionalUtils.Reduce(numbers, 1, (acc, n) => acc * n);
            Console.WriteLine($"Добуток усіх чисел (Reduce): {product}");

            bool allEven = FunctionalUtils.All(numbers, n => n % 2 == 0);
            Console.WriteLine($"Усі числа парні (All): {allEven}");

            int firstOverFive = FunctionalUtils.FirstOrDefault(numbers, n => n > 5);
            Console.WriteLine($"Перше число > 5 (FirstOrDefault): {firstOverFive}\n");

            Console.WriteLine("--- 2. Тестування зі string ---");
            List<string> words = new List<string> { "Apple", "Banana", "Avocado", "Apricot" };

            var lengths = FunctionalUtils.Map(words, w => w.Length);
            Console.Write("Довжини слів (Map): ");
            FunctionalUtils.ForEach(lengths, l => Console.Write($"{l} "));
            Console.WriteLine();

            string sentence = FunctionalUtils.Reduce(words, "", (acc, w) => acc == "" ? w : $"{acc} -> {w}");
            Console.WriteLine($"З'єднані слова (Reduce): {sentence}");

            bool allStartWithA = FunctionalUtils.All(words, w => w.StartsWith("A"));
            Console.WriteLine($"Усі слова починаються на 'A' (All): {allStartWithA}");

            string? longWord = FunctionalUtils.FirstOrDefault(words, w => w.Length > 6);
            Console.WriteLine($"Перше слово довжиною > 6 (FirstOrDefault): {longWord ?? "Не знайдено"}\n");

            Console.WriteLine("--- 3. Тестування з власним класом Person ---");
            List<Person> people = new List<Person>
            {
                new Person("Андрій", 20),
                new Person("Максим", 19),
                new Person("Олена", 22),
                new Person("Ірина", 18)
            };

            Console.WriteLine("Список людей (ForEach):");
            FunctionalUtils.ForEach(people, p => Console.WriteLine($"  - {p}"));

            var names = FunctionalUtils.Map(people, p => p.Name);
            Console.Write("Список імен (Map): ");
            FunctionalUtils.ForEach(names, name => Console.Write($"{name}; "));
            Console.WriteLine();

            int totalAge = FunctionalUtils.Reduce(people, 0, (acc, p) => acc + p.Age);
            Console.WriteLine($"Сумарний вік (Reduce): {totalAge} р.");

            bool allAdults = FunctionalUtils.All(people, p => p.Age >= 18);
            Console.WriteLine($"Усі повнолітні (All): {allAdults}");

            Person? olena = FunctionalUtils.FirstOrDefault(people, p => p.Name == "Олена");
            Console.WriteLine($"Знайдена людина (FirstOrDefault): {olena?.ToString() ?? "Не знайдено"}");
        }
    }
}