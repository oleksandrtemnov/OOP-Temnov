using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork9
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;
    }

    // Сценарій 1: Кеш метаданих (Dictionary)
    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

        public void Add(string path, FileMetadata metadata) => _cache[path] = metadata;

        public FileMetadata? Get(string path) => _cache.TryGetValue(path, out var meta) ? meta : null;

        public bool Remove(string path) => _cache.Remove(path);

        public bool Contains(string path) => _cache.ContainsKey(path);
    }

    // Сценарій 2: Унікальні теги (HashSet)
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
                tags.Remove(tag);
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                allTags.UnionWith(tags);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1) ||
                !_articleTags.TryGetValue(articleId2, out var tags2))
                return new HashSet<string>();

            var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(tags2);
            return common;
        }
    }

    // Сценарій 3: Індекс для пошуку (Dictionary + HashSet)
    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                    _invertedIndex[word] = new HashSet<int>();

                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            return _invertedIndex.TryGetValue(word, out var docs)
                ? new HashSet<int>(docs)
                : new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words.Length == 0) return new HashSet<int>();

            var result = Search(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                result.IntersectWith(Search(words[i]));
            }
            return result;
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Тест FileMetadataCache
            var fileCache = new FileMetadataCache();
            fileCache.Add("C:/docs/report.pdf", new FileMetadata { FileName = "report.pdf", Size = 1024, LastModified = DateTime.Now, Hash = "ABC123" });
            Console.WriteLine($"Файл у кеші: {fileCache.Contains("c:/docs/report.pdf")}, Розмір: {fileCache.Get("C:/docs/report.pdf")?.Size} байт");

            // 2. Тест TagManager
            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", ".NET");
            tagManager.AddTag("art2", "c#");
            tagManager.AddTag("art2", "LINQ");
            Console.WriteLine("Усі унікальні теги: " + string.Join(", ", tagManager.GetAllUniqueTags()));
            Console.WriteLine("Спільні теги art1 та art2: " + string.Join(", ", tagManager.FindCommonTags("art1", "art2")));

            // 3. Тест SearchIndex
            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# is a modern programming language");
            searchIndex.AddDocument(2, "C# and .NET are great for backend");
            searchIndex.AddDocument(3, "Python is popular for data science");
            Console.WriteLine("Пошук 'C#' та 'programming': Документи ID = " + string.Join(", ", searchIndex.SearchMultiple("C#", "programming")));
        }
    }
}