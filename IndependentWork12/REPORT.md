# Звіт до Самостійної роботи №12: LINQ pipelines

## 1. Опис пайплайнів та використаних операторів
- **Пайплайн 1 (`TopProductsByCategory`):** `GroupBy` (по товарах) → `Sum` → `Join` (з довідником товарів) → повторний `GroupBy` (по категоріях) → `OrderByDescending` + `Take(3)`.
- **Пайплайн 2 (`TotalRevenueByCity`):** подвійний `Join` (`Sale` + `Product` + `Customer`) → `GroupBy` (по місту) → `Sum` → `OrderByDescending`.
- **Пайплайн 3 (`SalesByYearAndMonth`):** `GroupBy` за складеним ключем `{ Year, Month }` → агрегація обсягів і виручки → `OrderBy` + `ThenBy`.
- **Пайплайн 4 (`VipCustomersAnalytics`):** попередня фільтрація (`Where`) → `Join` → `GroupBy` → `Join` → `OrderByDescending` + `Take(5)`.

## 2. Аналіз продуктивності та висновки
- Перший пайплайн виконується дещо довше (~2 мс) через JIT-компіляцію методів LINQ, наступні виконуються швидше (~0.3–0.6 мс).
- LINQ забезпечує високу декларативність коду та ефективність завдяки відкладеному виконанню (deferred execution) і хеш-алгоритмам всередині `Join` та `GroupBy` (складність $O(N + M)$).