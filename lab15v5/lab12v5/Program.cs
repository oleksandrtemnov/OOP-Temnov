using System;
using System.Collections.Generic;

namespace Lab12v5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. ТЕСТУВАННЯ З ТИПОМ INT ===");
            List<int> numbers = new List<int> { 2, 4, 6, 8, 10 };

            var doubled = FunctionalUtils.Map(numbers, x => x * 2);
            Console.WriteLine("Map: " + string.Join(", ", doubled));

            int sum = FunctionalUtils.Reduce(numbers, 0, (acc, x) => acc + x);
            Console.WriteLine($"Reduce: {sum}");

            Console.Write("ForEach: ");
            FunctionalUtils.ForEach(numbers, x => Console.Write($"[{x}] "));
            Console.WriteLine();

            bool allEven = FunctionalUtils.All(numbers, x => x % 2 == 0);
            Console.WriteLine($"All: {allEven}");

            int firstGt5 = FunctionalUtils.FirstOrDefault(numbers, x => x > 5);
            Console.WriteLine($"FirstOrDefault: {firstGt5}\n");


            Console.WriteLine("=== 2. ТЕСТУВАННЯ З ТИПОМ STRING ===");
            List<string> words = new List<string> { "C#", "Generics", "Func", "Action", "Lambda" };

            var lengths = FunctionalUtils.Map(words, w => w.Length);
            Console.WriteLine("Map: " + string.Join(", ", lengths));

            string sentence = FunctionalUtils.Reduce(words, "", (acc, w) => acc == "" ? w : $"{acc} -> {w}");
            Console.WriteLine($"Reduce: {sentence}");

            Console.WriteLine("ForEach:");
            FunctionalUtils.ForEach(words, w => Console.WriteLine($" - {w.ToUpper()}"));

            bool allCapitalized = FunctionalUtils.All(words, w => char.IsUpper(w[0]));
            Console.WriteLine($"All: {allCapitalized}");

            string longWord = FunctionalUtils.FirstOrDefault(words, w => w.Length > 5) ?? "";
            Console.WriteLine($"FirstOrDefault: {longWord}\n");


            Console.WriteLine("=== 3. ТЕСТУВАННЯ З ВЛАСНИМ КЛАСОМ PRODUCT ===");
            List<Product> products = new List<Product>
            {
                new Product("Ноутбук", 1200.0, 5),
                new Product("Мишка", 25.0, 20),
                new Product("Клавіатура", 75.0, 12),
                new Product("Монітор", 300.0, 8)
            };

            List<string> productTitles = FunctionalUtils.Map(products, p => p.Title);
            Console.WriteLine("Map: " + string.Join(", ", productTitles));

            double totalInventoryValue = FunctionalUtils.Reduce(products, 0.0, (acc, p) => acc + (p.Price * p.Stock));
            Console.WriteLine($"Reduce: ${totalInventoryValue}");

            Console.WriteLine("ForEach:");
            FunctionalUtils.ForEach(products, p => Console.WriteLine($" - {p}"));

            bool allInStock = FunctionalUtils.All(products, p => p.Stock > 0);
            Console.WriteLine($"All: {allInStock}");

            Product? expensiveProduct = FunctionalUtils.FirstOrDefault(products, p => p.Price > 100);
            Console.WriteLine($"FirstOrDefault: {expensiveProduct}");
        }
    }
}