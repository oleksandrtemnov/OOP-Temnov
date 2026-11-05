using System;
using System.Collections.Generic;
using System.Text;

namespace Lab14V5
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(int id, string sender, string recipient, string content)
        {
            Id = id;
            Sender = sender;
            Recipient = recipient;
            Content = content;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [ID:{Id}] Від: {Sender} -> Кому: {Recipient} | \"{Content}\"";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("    ЛАБОРАТОРНА РОБОТА №14 | ВАРІАНТ 5            ");
            Console.WriteLine("    ТЕМА: HashSet / Queue / Stack в C#             ");
            Console.WriteLine("==================================================\n");

            Console.WriteLine("--- 1. HASHSET<T> (Унікальні одержувачі) ---");
            HashSet<string> uniqueRecipients = new HashSet<string>();

            Console.WriteLine("1.1. Додавання одержувачів до системи:");
            AddRecipient(uniqueRecipients, "ivan@example.com");
            AddRecipient(uniqueRecipients, "maria@example.com");
            AddRecipient(uniqueRecipients, "petro@example.com");

            Console.WriteLine("\n1.2. Спроба додати дублікат:");
            AddRecipient(uniqueRecipients, "ivan@example.com");

            Console.WriteLine("\n1.3. Перевірка наявності (Contains):");
            string checkEmail = "maria@example.com";
            Console.WriteLine($"Чи є '{checkEmail}' у списку? -> {uniqueRecipients.Contains(checkEmail)}");

            Console.WriteLine("\n1.4. Видалення одержувача (Remove):");
            uniqueRecipients.Remove("petro@example.com");
            Console.WriteLine("Одержувача 'petro@example.com' видалено.");

            Console.WriteLine($"\nПоточна кількість унікальних одержувачів: {uniqueRecipients.Count}");
            PrintCollection(uniqueRecipients, "Список активних одержувачів");

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            Console.WriteLine("--- 2. QUEUE<T> (Черга повідомлень - FIFO) ---");
            Queue<Message> outgoingQueue = new Queue<Message>();

            Console.WriteLine("2.1. Додавання повідомлень у чергу (Enqueue):");
            Message msg1 = new Message(101, "System", "ivan@example.com", "Ласкаво просимо!");
            Message msg2 = new Message(102, "Support", "maria@example.com", "Ваш квиток оновлено.");
            Message msg3 = new Message(103, "Admin", "ivan@example.com", "Нагадування про пароль.");

            outgoingQueue.Enqueue(msg1);
            Console.WriteLine($"[+] Повідомлення #{msg1.Id} додано в чергу.");
            outgoingQueue.Enqueue(msg2);
            Console.WriteLine($"[+] Повідомлення #{msg2.Id} додано в чергу.");
            outgoingQueue.Enqueue(msg3);
            Console.WriteLine($"[+] Повідомлення #{msg3.Id} додано в чергу.");

            Console.WriteLine($"\n2.2. Перегляд першого повідомлення в черзі (Peek):");
            Console.WriteLine($"Наступне до відправки: {outgoingQueue.Peek()}");

            Console.WriteLine("\n2.3. Послідовна відправка (Dequeue):");
            Stack<Message> sentHistory = new Stack<Message>();

            while (outgoingQueue.Count > 0)
            {
                Message currentMsg = outgoingQueue.Dequeue();
                Console.WriteLine($"[-->] Відправлено: {currentMsg}");
                sentHistory.Push(currentMsg);
            }

            Console.WriteLine($"Залишок у черзі відправки: {outgoingQueue.Count}");

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            Console.WriteLine("--- 3. STACK<T> (Історія надісланих - LIFO) ---");

            Console.WriteLine("3.1. Перегляд останнього надісланого повідомлення (Peek):");
            if (sentHistory.Count > 0)
            {
                Console.WriteLine($"Останнє надіслане: {sentHistory.Peek()}");
            }

            Console.WriteLine("\n3.2. Всі надіслані повідомлення (порядок від найновішого до найстарішого):");
            PrintCollection(sentHistory, "Історія (Stack)");

            Console.WriteLine("\n3.3. Відкликання / Видалення з історії останньої дії (Pop):");
            Message recalledMsg = sentHistory.Pop();
            Console.WriteLine($"[<--] Відкликано повідомлення з історії: {recalledMsg}");

            Console.WriteLine($"\nНове останнє повідомлення в історії: {sentHistory.Peek()}");
            Console.WriteLine($"Кількість записів в історії після Pop: {sentHistory.Count}");

            Console.WriteLine("\n==================================================");
            Console.WriteLine("    ЛАБОРАТОРНУ РОБОТУ УСПІШНО ВИКОНАНО!          ");
            Console.WriteLine("==================================================");
        }

        private static void AddRecipient(HashSet<string> set, string email)
        {
            bool added = set.Add(email);
            if (added)
                Console.WriteLine($"[Успіх] Одержувача '{email}' додано.");
            else
                Console.WriteLine($"[Дублікат] Одержувач '{email}' вже існує в системі!");
        }

        private static void PrintCollection<T>(IEnumerable<T> collection, string header)
        {
            Console.WriteLine($"--- {header} ---");
            int index = 1;
            foreach (var item in collection)
            {
                Console.WriteLine($"  {index++}. {item}");
            }
        }
    }
}