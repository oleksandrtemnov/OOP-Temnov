using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab9
{
    public class DeliveryException : Exception
    {
        public DeliveryException(string message) : base(message) { }
    }

    public abstract class DeliveryService
    {
        public string ItemName { get; set; } = string.Empty;
        public bool IsInStock { get; set; } = true;

        public abstract void Deliver(string address);
        public abstract void Validate(string address);

        protected void BaseValidate(string address)
        {
            if (!IsInStock)
            {
                throw new DeliveryException($"[Склад] Товар '{ItemName}' відсутній на складі.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new DeliveryException("[Адреса] Адреса доставки не може бути порожньою.");
            }
        }
    }

    public class CourierDelivery : DeliveryService
    {
        public TimeSpan DeliveryWindow { get; set; } = TimeSpan.FromHours(2);

        public override void Validate(string address)
        {
            BaseValidate(address);

            if (!address.Contains("вул.") && !address.Contains("просп."))
            {
                throw new DeliveryException($"[Courier Error] Для кур'єрської доставки вкажіть вулицю (вул. або просп.) у адресі: '{address}'");
            }
        }

        public override void Deliver(string address)
        {
            Validate(address);
            Console.WriteLine($"[Кур'єр] Товар '{ItemName}' буде доставлено за адресою '{address}'. Очікуваний час: {DeliveryWindow.Hours} год.");
        }
    }

    public class PostDelivery : DeliveryService
    {
        public string PostalCode { get; set; } = string.Empty;

        public override void Validate(string address)
        {
            BaseValidate(address);

            if (string.IsNullOrWhiteSpace(PostalCode) || !Regex.IsMatch(PostalCode, @"^\d{5}$"))
            {
                throw new DeliveryException($"[Post Error] Невалідний поштовий індекс: '{PostalCode}'. Повинен містити рівно 5 цифр.");
            }
        }

        public override void Deliver(string address)
        {
            Validate(address);
            Console.WriteLine($"[Пошта] Товар '{ItemName}' відправлено поштою за адресою '{address}' (Індекс: {PostalCode}).");
        }
    }

    public class PickupDelivery : DeliveryService
    {
        public string PickupPointId { get; set; } = string.Empty;

        public override void Validate(string address)
        {
            BaseValidate(address);

            if (string.IsNullOrWhiteSpace(PickupPointId) || !PickupPointId.StartsWith("PICKUP-"))
            {
                throw new DeliveryException($"[Pickup Error] Некоректний ID пункту видачі: '{PickupPointId}'. Повинен починатися з 'PICKUP-'.");
            }
        }

        public override void Deliver(string address)
        {
            Validate(address);
            Console.WriteLine($"[Самовивіз] Товар '{ItemName}' готовий до видачі в пункті {PickupPointId} (Адреса: '{address}').");
        }
    }

    public class OrderDeliveryManager
    {
        public void DeliverAll(List<(DeliveryService service, string address)> deliveryOrders)
        {
            Console.WriteLine("=== ПОЧАТОК ОБРОБКИ ДОСТАВОК ===");
            int successCount = 0;
            int errorCount = 0;

            foreach (var order in deliveryOrders)
            {
                try
                {
                    order.service.Deliver(order.address);
                    successCount++;
                }
                catch (DeliveryException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"НЕПЕРЕДБАЧЕНА ПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
            }

            Console.WriteLine("================================");
            Console.WriteLine($"Успішно оброблено: {successCount} | З помилками: {errorCount}\n");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var ordersList = new List<(DeliveryService service, string address)>
            {
                (
                    new CourierDelivery 
                    { 
                        ItemName = "Ноутбук Lenovo", 
                        IsInStock = true 
                    }, 
                    "м. Рівне, вул. Соборна, 12"
                ),
                (
                    new PostDelivery 
                    { 
                        ItemName = "Книга 'C# 12 in a Nutshell'", 
                        PostalCode = "33000", 
                        IsInStock = true 
                    }, 
                    "м. Рівне, Головне відділення"
                ),
                (
                    new PickupDelivery 
                    { 
                        ItemName = "Навушники Sony", 
                        PickupPointId = "PICKUP-102", 
                        IsInStock = true 
                    }, 
                    "м. Рівне, вул. Київська, 40"
                ),
                (
                    new CourierDelivery 
                    { 
                        ItemName = "Смартфон Samsung", 
                        IsInStock = false 
                    }, 
                    "м. Рівне, вул. Миру, 5"
                ),
                (
                    new CourierDelivery 
                    { 
                        ItemName = "Монітор Dell", 
                        IsInStock = true 
                    }, 
                    "м. Рівне, центр"
                ),
                (
                    new PostDelivery 
                    { 
                        ItemName = "Мишка Logitech", 
                        PostalCode = "3300", 
                        IsInStock = true 
                    }, 
                    "м. Рівне"
                ),
                (
                    new PickupDelivery 
                    { 
                        ItemName = "Клавіатура Keychron", 
                        PickupPointId = "POINT-55", 
                        IsInStock = true 
                    }, 
                    "м. Рівне, вул. Степана Бандери, 12"
                )
            };

            OrderDeliveryManager manager = new OrderDeliveryManager();
            manager.DeliverAll(ordersList);

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}