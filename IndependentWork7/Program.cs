using System;

namespace IndependentWork7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var cache = new Cache<string, string>(3);

            Console.WriteLine("Додаємо 3 елементи (ємність = 3):");
            cache.Add("user_1", "Олександр");
            cache.Add("user_2", "Марія");
            cache.Add("user_3", "Іван");

            Console.WriteLine($"Кількість у кеші: {cache.Count}");
            Console.WriteLine($"user_1 -> {cache.Get("user_1")}");

            Console.WriteLine("\nДодаємо 4-й елемент (має витіснити user_1 за FIFO):");
            cache.Add("user_4", "Олена");

            Console.WriteLine($"Кількість у кеші: {cache.Count}");
            Console.WriteLine($"Чи є user_1 у кеші? {cache.ContainsKey("user_1")}");
            Console.WriteLine($"user_4 -> {cache.Get("user_4")}");

            try
            {
                cache.Get("user_1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Очікуваний виняток: {ex.Message}");
            }
        }
    }
}