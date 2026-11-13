using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab15v5
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Олександр Коваленко", City = "Kyiv" },
                new Customer { Id = 2, Name = "Марія Шевченко", City = "Lviv" },
                new Customer { Id = 3, Name = "Дмитро Бондаренко", City = "Kyiv" },
                new Customer { Id = 4, Name = "Ірина Ткаченко", City = "Odesa" },
                new Customer { Id = 5, Name = "Максим Мельник", City = "Kyiv" },
                new Customer { Id = 6, Name = "Анна Кравченко", City = "Kharkiv" },
                new Customer { Id = 7, Name = "Владислав Олійник", City = "Rivne" },
                new Customer { Id = 8, Name = "Олена Поліщук", City = "Kyiv" },
                new Customer { Id = 9, Name = "Артем Савченко", City = "Dnipro" },
                new Customer { Id = 10, Name = "Софія Руденко", City = "Lviv" },
                new Customer { Id = 11, Name = "Тарас Мороз", City = "Kyiv" },
                new Customer { Id = 12, Name = "Юлія Лисенко", City = "Rivne" }
            };

            var orders = new List<Order>
            {
                new Order { Id = 101, OrderDate = new DateTime(2026, 10, 1), CustomerId = 1, TotalAmount = 1550m },
                new Order { Id = 102, OrderDate = new DateTime(2026, 10, 3), CustomerId = 2, TotalAmount = 890m },
                new Order { Id = 103, OrderDate = new DateTime(2026, 10, 5), CustomerId = 3, TotalAmount = 450m },
                new Order { Id = 104, OrderDate = new DateTime(2026, 10, 7), CustomerId = 1, TotalAmount = 2300m },
                new Order { Id = 105, OrderDate = new DateTime(2026, 10, 10), CustomerId = 5, TotalAmount = 1200m },
                new Order { Id = 106, OrderDate = new DateTime(2026, 10, 12), CustomerId = 4, TotalAmount = 3400m },
                new Order { Id = 107, OrderDate = new DateTime(2026, 10, 14), CustomerId = 8, TotalAmount = 999m },
                new Order { Id = 108, OrderDate = new DateTime(2026, 10, 15), CustomerId = 11, TotalAmount = 5100m },
                new Order { Id = 109, OrderDate = new DateTime(2026, 10, 18), CustomerId = 6, TotalAmount = 670m },
                new Order { Id = 110, OrderDate = new DateTime(2026, 10, 20), CustomerId = 7, TotalAmount = 1850m },
                new Order { Id = 111, OrderDate = new DateTime(2026, 10, 22), CustomerId = 3, TotalAmount = 1750m },
                new Order { Id = 112, OrderDate = new DateTime(2026, 10, 25), CustomerId = 9, TotalAmount = 420m },
                new Order { Id = 113, OrderDate = new DateTime(2026, 10, 28), CustomerId = 10, TotalAmount = 2900m },
                new Order { Id = 114, OrderDate = new DateTime(2026, 11, 2), CustomerId = 8, TotalAmount = 1420m },
                new Order { Id = 115, OrderDate = new DateTime(2026, 11, 5), CustomerId = 12, TotalAmount = 780m },
                new Order { Id = 116, OrderDate = new DateTime(2026, 11, 10), CustomerId = 5, TotalAmount = 6400m }
            };

            var sw = new Stopwatch();

            Console.WriteLine("=== ЗАПИТ 1: Замовлення з м. Kyiv на суму > 1000 ===");
            sw.Restart();
            var q1Query = (from o in orders
                           join c in customers on o.CustomerId equals c.Id
                           where c.City == "Kyiv" && o.TotalAmount > 1000m
                           select new { o.Id, c.Name, o.TotalAmount }).ToList();
            sw.Stop();
            var t1Query = sw.Elapsed;

            sw.Restart();
            var q1Method = orders
                .Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { o, c })
                .Where(x => x.c.City == "Kyiv" && x.o.TotalAmount > 1000m)
                .Select(x => new { x.o.Id, x.c.Name, x.o.TotalAmount })
                .ToList();
            sw.Stop();
            var t1Method = sw.Elapsed;

            foreach (var x in q1Method)
                Console.WriteLine($"#{x.Id} | {x.Name} | {x.TotalAmount} грн");
            Console.WriteLine($"Час: Query = {t1Query.TotalMilliseconds:F4} мс, Method = {t1Method.TotalMilliseconds:F4} мс\n");

            Console.WriteLine("=== ЗАПИТ 2: Дата замовлення та ім'я клієнта ===");
            sw.Restart();
            var q2Query = (from o in orders
                           join c in customers on o.CustomerId equals c.Id
                           select new { o.OrderDate, c.Name }).ToList();
            sw.Stop();
            var t2Query = sw.Elapsed;

            sw.Restart();
            var q2Method = orders
                .Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { o.OrderDate, c.Name })
                .ToList();
            sw.Stop();
            var t2Method = sw.Elapsed;

            foreach (var x in q2Method)
                Console.WriteLine($"{x.OrderDate:yyyy-MM-dd} | {x.Name}");
            Console.WriteLine($"Час: Query = {t2Query.TotalMilliseconds:F4} мс, Method = {t2Method.TotalMilliseconds:F4} мс\n");

            Console.WriteLine("=== ЗАПИТ 3: Загальна сума замовлень для кожного клієнта ===");
            sw.Restart();
            var q3Query = (from c in customers
                           join o in orders on c.Id equals o.CustomerId into grp
                           select new { c.Name, Sum = grp.Sum(x => x.TotalAmount) }).ToList();
            sw.Stop();
            var t3Query = sw.Elapsed;

            sw.Restart();
            var q3Method = customers
                .GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, grp) => new { c.Name, Sum = grp.Sum(x => x.TotalAmount) })
                .ToList();
            sw.Stop();
            var t3Method = sw.Elapsed;

            foreach (var x in q3Method)
                Console.WriteLine($"{x.Name,-22} | {x.Sum} грн");
            Console.WriteLine($"Час: Query = {t3Query.TotalMilliseconds:F4} мс, Method = {t3Method.TotalMilliseconds:F4} мс\n");

            Console.WriteLine("=== ЗАПИТ 4: Замовлення з максимальною сумою ===");
            sw.Restart();
            var q4Query = (from o in orders
                           where o.TotalAmount == (from ord in orders select ord.TotalAmount).Max()
                           select o).FirstOrDefault();
            sw.Stop();
            var t4Query = sw.Elapsed;

            sw.Restart();
            var q4Method = orders.OrderByDescending(o => o.TotalAmount).FirstOrDefault();
            sw.Stop();
            var t4Method = sw.Elapsed;

            if (q4Method != null)
                Console.WriteLine($"#{q4Method.Id} | {q4Method.OrderDate:yyyy-MM-dd} | {q4Method.TotalAmount} грн");
            Console.WriteLine($"Час: Query = {t4Query.TotalMilliseconds:F4} мс, Method = {t4Method.TotalMilliseconds:F4} мс");
        }
    }
}