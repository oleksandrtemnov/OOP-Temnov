using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            // Шаблон [\w#\+]+ дозволяє зберегти слова на кшталт "c#" або "c++"
            var matches = Regex.Matches(text.ToLowerInvariant(), @"[\w#\+]+");
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _documents = new();

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;

            var tokens = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));
            foreach (var word in tokens)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _index[word] = docIds;
                }
                docIds.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalized = TextProcessor.Tokenize(word).FirstOrDefault();
            if (normalized == null || !_index.TryGetValue(normalized, out var docIds))
                return new List<Document>();

            return docIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(TextProcessor.Tokenize)
                .Distinct()
                .ToList();

            if (normalizedWords.Count == 0)
                return new List<Document>();

            if (!_index.TryGetValue(normalizedWords[0], out var firstSet))
                return new List<Document>();

            var resultIds = new HashSet<int>(firstSet);

            for (int i = 1; i < normalizedWords.Count; i++)
            {
                if (_index.TryGetValue(normalizedWords[i], out var nextSet))
                {
                    resultIds.IntersectWith(nextSet);
                }
                else
                {
                    return new List<Document>();
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var normalizedWords = words.SelectMany(TextProcessor.Tokenize).Distinct();
            var resultIds = new HashSet<int>();

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultIds.UnionWith(docIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            var baseTemplates = new[]
            {
                ("Introduction to C#", "C# is a modern object-oriented programming language developed by Microsoft."),
                ("Java Basics", "Java is a popular programming language known for its portability and performance."),
                ("Python Overview", "Python is a high-level programming language with simple syntax."),
                ("C# Advanced Features", "C# supports LINQ, async/await, and many modern programming paradigms."),
                ("Comparing Languages", "C#, Java, and Python are among the most popular programming languages today.")
            };

            var docs = new List<Document>(count);
            for (int i = 1; i <= count; i++)
            {
                var template = baseTemplates[(i - 1) % baseTemplates.Length];
                docs.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Item1} (Vol. {i})",
                    Content = template.Item2
                });
            }
            return docs;
        }

        public static void RunBenchmarks()
        {
            int[] sizes = { 10, 100, 1000, 10000 };
            string searchWord = "async";

            Console.WriteLine("=== ВИМІРЮВАННЯ ПРОДУКТИВНОСТІ ПОШУКУ (слово: 'async') ===");
            Console.WriteLine($"{"К-сть документів",-18} | {"Лінійний (ticks)",-18} | {"Індекс (ticks)",-16} | {"Знайдено",-10}");
            Console.WriteLine(new string('-', 70));

            foreach (int size in sizes)
            {
                var docs = GenerateDocuments(size);
                var index = new InvertedIndex();
                foreach (var doc in docs)
                    index.AddDocument(doc);

                // Прогрів JIT
                _ = docs.Count(d => TextProcessor.Tokenize(d.Content).Contains(searchWord));
                _ = index.SearchSingleWord(searchWord);

                // 1. Лінійний пошук (однакова нормалізація через Tokenize)
                var sw = Stopwatch.StartNew();
                var linearResults = docs
                    .Where(d => TextProcessor.Tokenize(d.Content).Contains(searchWord))
                    .ToList();
                sw.Stop();
                long linearTicks = sw.ElapsedTicks;

                // 2. Пошук через Inverted Index
                sw.Restart();
                var indexResults = index.SearchSingleWord(searchWord);
                sw.Stop();
                long indexTicks = sw.ElapsedTicks;

                Console.WriteLine($"{size,-18} | {linearTicks,-18} | {indexTicks,-16} | {linearResults.Count} == {indexResults.Count}");
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Запуск бенчмарків для 10 / 100 / 1000 / 10000 документів
            PerformanceTest.RunBenchmarks();

            // 2. Інтерактивна демонстрація на 5 базових документах
            var index = new InvertedIndex();
            var sampleDocs = PerformanceTest.GenerateDocuments(5);
            foreach (var doc in sampleDocs)
                index.AddDocument(doc);

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.Equals(query, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.IsNullOrWhiteSpace(query))
                    continue;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("\n--- Search Results (AND) ---");
                var andResults = index.SearchMultipleWords(words);
                foreach (var doc in andResults)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content[..Math.Min(100, doc.Content.Length)]}...");
                }
                Console.WriteLine($"Found {andResults.Count} documents");

                Console.WriteLine("\n--- Search Results (OR) ---");
                var orResults = index.SearchAnyWord(words);
                Console.WriteLine($"Found {orResults.Count} documents: {string.Join(", ", orResults.Select(d => d.Id))}\n");
            }
        }
    }
}