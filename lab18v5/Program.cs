using System;
using System.Collections.Generic;

namespace lab18v5
{
    public class OrderNotFoundException : Exception
    {
        public int OrderId { get; }

        public OrderNotFoundException()
        {
        }

        public OrderNotFoundException(int orderId)
            : base($"Замовлення з ID {orderId} не знайдено.")
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(int orderId, string message)
            : base(message)
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

        public static Result<T> Failure(string error) => new Result<T>(default!, false, error);

        public static Result<T> Failure(Exception ex) => new Result<T>(default!, false, ex.Message);
    }

    public class Order
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class OrderService
    {
        private readonly Dictionary<int, Order> _orders = new()
        {
            { 101, new Order { Id = 101, ProductName = "Ноутбук", Quantity = 1 } },
            { 102, new Order { Id = 102, ProductName = "Мишка", Quantity = 2 } }
        };

        private readonly Dictionary<string, int> _stock = new()
        {
            { "Ноутбук", 3 },
            { "Мишка", 0 }
        };

        public Order GetOrder(int orderId)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                throw new OrderNotFoundException(orderId);
            }

            return order;
        }

        public Result<bool> PlaceOrder(Order order)
        {
            if (order == null || order.Quantity <= 0)
            {
                return Result<bool>.Failure("Некоректні дані замовлення.");
            }

            if (!_stock.TryGetValue(order.ProductName, out int available) || available < order.Quantity)
            {
                return Result<bool>.Failure($"Товар '{order.ProductName}' відсутній у потрібній кількості (доступно: {available}).");
            }

            _stock[order.ProductName] -= order.Quantity;
            _orders[order.Id] = order;
            return Result<bool>.Success(true);
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var orderService = new OrderService();

            Console.WriteLine("=== 1. Пошук замовлення (GetOrder + OrderNotFoundException) ===");
            try
            {
                var existingOrder = orderService.GetOrder(101);
                Console.WriteLine($"Знайдено замовлення #{existingOrder.Id}: {existingOrder.ProductName} ({existingOrder.Quantity} шт.)");

                var missingOrder = orderService.GetOrder(999);
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine($"[Виняток] {ex.Message} (ID = {ex.OrderId})");
            }

            Console.WriteLine();

            Console.WriteLine("=== 2. Оформлення замовлення (PlaceOrder + Result<bool>) ===");
            var validOrder = new Order { Id = 103, ProductName = "Ноутбук", Quantity = 2 };
            var res1 = orderService.PlaceOrder(validOrder);
            if (res1.IsSuccess)
            {
                Console.WriteLine($"Замовлення #{validOrder.Id} успішно оформлено.");
            }
            else
            {
                Console.WriteLine($"Помилка: {res1.Error}");
            }

            var outOfStockOrder = new Order { Id = 104, ProductName = "Мишка", Quantity = 1 };
            var res2 = orderService.PlaceOrder(outOfStockOrder);
            if (res2.IsSuccess)
            {
                Console.WriteLine($"Замовлення #{outOfStockOrder.Id} успішно оформлено.");
            }
            else
            {
                Console.WriteLine($"[Result Failure] Не вдалося оформити замовлення #{outOfStockOrder.Id}: {res2.Error}");
            }
        }
    }
}