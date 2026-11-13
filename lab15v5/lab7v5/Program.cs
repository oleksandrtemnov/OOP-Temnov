using System;

namespace Lab7Task
{
    public class Document
    {
        public string Title { get; set; }

        public Document(string title)
        {
            Title = title;
        }

        public virtual void Open()
        {
            Console.WriteLine($"[Document] Відкриваємо базовий документ '{Title}'");
        }
    }

    public class PDF : Document
    {
        public double FileSizeMB { get; set; }

        public PDF(string title, double fileSizeMB) : base(title)
        {
            FileSizeMB = fileSizeMB;
        }

        public override void Open()
        {
            Console.WriteLine($"[PDF (override)] Відкриваємо PDF-документ '{Title}' розміром {FileSizeMB} МБ у спеціальному переглядачі");
        }
    }

    public class Text : Document
    {
        public string Encoding { get; set; }

        public Text(string title, string encoding) : base(title)
        {
            Encoding = encoding;
        }

        public new void Open()
        {
            Console.WriteLine($"[Text (new)] Відкриваємо текстовий документ '{Title}' у кодуванні {Encoding} через Блокнот");
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Document doc = new Document("Загальний документ");
            PDF pdf = new PDF("Лабораторна_Робота.pdf", 2.5);
            Text text = new Text("Замітки.txt", "UTF-8");

            Console.WriteLine("=== 1. Прямі виклики через власні типи ===");
            doc.Open();
            pdf.Open();
            text.Open();

            Console.WriteLine("\n=== 2. Виклики через посилання базового типу Document (Upcasting) ===");
            Document docPdf = pdf;
            Document docText = text;

            docPdf.Open();
            docText.Open();

            Console.WriteLine("\n=== 3. Явне приведення типів (Downcasting) ===");
            ((PDF)docPdf).Open();
            ((Text)docText).Open();
        }
    }
}