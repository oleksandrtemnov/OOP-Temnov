using System;

namespace Lab3V5
{
    /// <summary>
    /// Варіант 5: Клас GraphicsContext
    /// </summary>
    public class GraphicsContext : IDisposable
    {
        private bool _disposed = false;
        private readonly int _contextId;
        private bool _isContextCreated;

        public int ContextId => _contextId;
        public bool IsContextCreated => _isContextCreated;

        public GraphicsContext(int contextId)
        {
            _contextId = contextId;
            _isContextCreated = true;
            Console.WriteLine($"[GraphicsContext {_contextId}] Контекст успішно створено та виділено ресурси.");
        }

        public void DrawShape(string shape)
        {
            if (_disposed || !_isContextCreated)
            {
                throw new ObjectDisposedException(nameof(GraphicsContext), $"Неможливо намалювати {shape}: графічний контекст знищено!");
            }

            Console.WriteLine($"[GraphicsContext {_contextId}] Малювання фігури: {shape}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів
                    Console.WriteLine($"[GraphicsContext {_contextId}] Звільнення керованих ресурсів...");
                }

                // Звільнення некерованих ресурсів
                if (_isContextCreated)
                {
                    Console.WriteLine($"[GraphicsContext {_contextId}] Знищення графічного контексту (некерований ресурс).");
                    _isContextCreated = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GraphicsContext()
        {
            Console.WriteLine($"[GraphicsContext {_contextId}] Виклик деструктора (фіналізатора)...");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Сценарій 1: Використання оператора using ===");
            using (var context1 = new GraphicsContext(101))
            {
                context1.DrawShape("Коло");
                context1.DrawShape("Квадрат");
            }
            Console.WriteLine();

            Console.WriteLine("=== Сценарій 2: Явний виклик Dispose() ===");
            var context2 = new GraphicsContext(102);
            context2.DrawShape("Трикутник");
            context2.Dispose();
            Console.WriteLine();

            Console.WriteLine("=== Сценарій 3: Демонстрація роботи деструктора через GC ===");
            CreateAndAbandonObject();

            Console.WriteLine("Примусовий запуск Garbage Collector...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateAndAbandonObject()
        {
            var context3 = new GraphicsContext(103);
            context3.DrawShape("Еліпс");
            // Об'єкт залишається без виклику Dispose()
        }
    }
}