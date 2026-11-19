Самостійна робота №9: Вивчення Dictionary<TKey, TValue> та HashSet<T> у .NET

Опис реалізації:
Реалізовано три практичні сценарії використання хеш-колекцій .NET:
1. FileMetadataCache — кешування метаданих файлів у Dictionary<string, FileMetadata> з нечутливим до регістру компаратором StringComparer.OrdinalIgnoreCase.
2. TagManager — управління унікальними тегами статей (Dictionary<string, HashSet<string>>) з використанням операцій UnionWith та IntersectWith.
3. SearchIndex — простий інвертований пошуковий індекс (Dictionary<string, HashSet<int>>) з підтримкою пошуку перетину документів за кількома словами.

Приклад виводу програми:
Файл у кеші: True, Розмір: 1024 байт
Усі унікальні теги: C#, .NET, LINQ
Спільні теги art1 та art2: C#
Пошук 'C#' та 'programming': Документи ID = 1