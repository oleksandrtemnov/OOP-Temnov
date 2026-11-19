using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Генерація тестових даних (Seed = 42 для відтворюваності)
            var (products, customers, sales) = GenerateTestData(seed: 42);
            Console.WriteLine($"Згенеровано: {products.Count} товарів, {customers.Count} клієнтів, {sales.Count} продажів.\n");

            TopProductsByCategory(sales, products);
            TotalRevenueByCity(sales, products, customers);
            SalesByYearAndMonth(sales, products);
            VipCustomersAnalytics(sales, products, customers);
        }

        static (List<Product>, List<Customer>, List<Sale>) GenerateTestData(int seed)
        {
            var rnd = new Random(seed);
            var categories = new[] { "Електроніка", "Побутова техніка", "Аксесуари", "Офіс", "Мережеве обладнання" };
            var cities = new[] { "Київ", "Львів", "Рівне", "Одеса", "Дніпро", "Харків" };

            var products = Enumerable.Range(1, 60)
                .Select(i => new Product
                {
                    Id = i,
                    Name = $"Товар #{i}",
                    Category = categories[rnd.Next(categories.Length)],
                    UnitPrice = Math.Round((decimal)(rnd.NextDouble() * 4800 + 200), 2)
                }).ToList();

            var customers = Enumerable.Range(1, 70)
                .Select(i => new Customer
                {
                    Id = i,
                    Name = $"Клієнт #{i}",
                    City = cities[rnd.Next(cities.Length)]
                }).ToList();

            var startDate = new DateTime(2024, 1, 1);
            var sales = Enumerable.Range(1, 150)
                .Select(i => new Sale
                {
                    Id = i,
                    ProductId = rnd.Next(1, products.Count + 1),
                    CustomerId = rnd.Next(1, customers.Count + 1),
                    Quantity = rnd.Next(1, 15),
                    SaleDate = startDate.AddDays(rnd.Next(0, 730))
                }).ToList();

            return (products, customers, sales);
        }

        // Пайплайн 1: Найпопулярніші продукти за категоріями (ТОП-3 у кожній)
        static void TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .Join(products,
                      stat => stat.ProductId,
                      prod => prod.Id,
                      (stat, prod) => new
                      {
                          prod.Name,
                          prod.Category,
                          stat.TotalQuantity
                      })
                .GroupBy(x => x.Category)
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    Top3 = catGroup
                        .OrderByDescending(x => x.TotalQuantity)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 1: ТОП-3 продукти за категоріями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var group in result)
            {
                Console.WriteLine($"Категорія: {group.Category}");
                foreach (var item in group.Top3)
                {
                    Console.WriteLine($"  - {item.Name}: {item.TotalQuantity} од.");
                }
            }
            Console.WriteLine();
        }

        // Пайплайн 2: Загальний дохід за містами клієнтів
        static void TotalRevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products,
                      s => s.ProductId,
                      p => p.Id,
                      (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers,
                      sp => sp.CustomerId,
                      c => c.Id,
                      (sp, c) => new { c.City, sp.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 2: Загальний дохід за містами (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var item in result)
            {
                Console.WriteLine($"  {item.City,-10}: {item.TotalRevenue:N2} грн");
            }
            Console.WriteLine();
        }

        // Пайплайн 3: Продажі за місяцями та роками
        static void SalesByYearAndMonth(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var productPrices = products.ToDictionary(p => p.Id, p => p.UnitPrice);

            var result = sales
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(s => s.Quantity),
                    TotalRevenue = g.Sum(s => s.Quantity * productPrices[s.ProductId])
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Take(10)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 3: Динаміка продажів за роками та місяцями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var row in result)
            {
                Console.WriteLine($"  {row.Year}-{row.Month:D2}: продано {row.TotalUnits} од., дохід {row.TotalRevenue:N2} грн");
            }
            Console.WriteLine();
        }

        // Пайплайн 4 (власний): ТОП-5 VIP-клієнтів за загальною сумою покупок за 2025 рік
        static void VipCustomersAnalytics(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var vipList = sales
                .Where(s => s.SaleDate.Year == 2025 && s.Quantity >= 2)
                .Join(products,
                      s => s.ProductId,
                      p => p.Id,
                      (s, p) => new { s.CustomerId, Amount = s.Quantity * p.UnitPrice })
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    OrdersCount = g.Count(),
                    TotalSpent = g.Sum(x => x.Amount)
                })
                .Join(customers,
                      stat => stat.CustomerId,
                      c => c.Id,
                      (stat, c) => new
                      {
                          c.Name,
                          c.City,
                          stat.OrdersCount,
                          stat.TotalSpent
                      })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 4: ТОП-5 VIP-клієнтів за 2025 рік (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var vip in vipList)
            {
                Console.WriteLine($"  {vip.Name} ({vip.City}) — замовлень: {vip.OrdersCount}, сума: {vip.TotalSpent:N2} грн");
            }
            Console.WriteLine();
        }
    }
}