using System;
using System.Collections.Generic;
using System.IO;

namespace Lab03Recursion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int N = 25;
            int K = 19;
            int a = 1;
            int b = 1;
            int c = 0;

            Console.WriteLine("=== Лабораторна робота 3 ===");
            Console.WriteLine($"N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

            // --- Завдання 1 ---
            Console.WriteLine("--- Завдання 1: Обхід каталогів (Максимальна глибина) ---");
            Console.Write("Введіть шлях до директорії: ");
            string? dirPath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(dirPath) && Directory.Exists(dirPath))
            {
                int maxDepth = Task1_GetMaxDirectoryDepth(dirPath);
                Console.WriteLine($"Максимальна глибина вкладеності папок: {maxDepth}");
            }
            else
            {
                Console.WriteLine("Помилка: Вказана директорія не існує.");
            }

            Console.WriteLine();

            // --- Завдання 2 ---
            Console.WriteLine("--- Завдання 2: Задача через дві половини (Елементи > 9) ---");
            int[] array16 = GenerateArray(16, N);
            Console.WriteLine("Згенерований масив (16 елементів):");
            Console.WriteLine(string.Join(", ", array16));

            int maxRecursionDepth = 0;
            int threshold = K % 10; // 9
            int countGreaterThan9 = Task2_Solve(array16, 0, array16.Length - 1, 1, ref maxRecursionDepth, threshold);

            Console.WriteLine($"Кількість елементів, більших за {threshold}: {countGreaterThan9}");
            Console.WriteLine($"Максимальна глибина рекурсії: {maxRecursionDepth}");
            Console.WriteLine($"Теоретичне значення log2(16): {Math.Log2(16)}");

            Console.WriteLine();

            // --- Завдання 3 ---
            Console.WriteLine("--- Завдання 3: Перебір з поверненням (Перестановки літер 'ОЛЕК') ---");
            char[] letters = new char[] { 'О', 'Л', 'Е', 'К' };
            bool[] used = new bool[letters.Length];
            List<char> currentPermutation = new List<char>();

            Console.WriteLine("Усі перестановки:");
            int totalPermutations = Task3_Permutations(letters, used, currentPermutation);
            Console.WriteLine($"Усього перестановок: {totalPermutations}");

            Console.WriteLine();

            // --- Завдання 4 ---
            Console.WriteLine("--- Завдання 4: Трасування (Стек викликів) ---");
            int[] array4 = GenerateArray(4, N);
            Console.WriteLine("Масив з 4 елементів: " + string.Join(", ", array4));
            Console.WriteLine("Трасування рекурсії:");
            Task4_TraceSolve(array4, 0, array4.Length - 1, 0, threshold);

            Console.WriteLine();

            // --- Завдання 5 ---
            Console.WriteLine("--- Завдання 5: Ітеративна версія (Цикл) ---");
            int iterativeCount = Task5_SolveIterative(array16, threshold);
            Console.WriteLine($"Результат циклу: {iterativeCount}");
            Console.WriteLine($"Результат рекурсії: {countGreaterThan9}");
            Console.WriteLine($"Результати збігаються: {iterativeCount == countGreaterThan9}");
        }

        // --- Допоміжний метод заповнення масиву ---
        static int[] GenerateArray(int size, int seed)
        {
            Random rnd = new Random(seed);
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = rnd.Next(-20, 21);
            }
            return arr;
        }

        // --- Завдання 1: Пошук максимальної глибини ---
        public static int Task1_GetMaxDirectoryDepth(string currentDir)
        {
            int maxSubDepth = 0;

            try
            {
                string[] subDirs = Directory.GetDirectories(currentDir);
                foreach (string subDir in subDirs)
                {
                    int subDepth = Task1_GetMaxDirectoryDepth(subDir);
                    if (subDepth > maxSubDepth)
                    {
                        maxSubDepth = subDepth;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаємо папки без доступу
            }
            catch (DirectoryNotFoundException)
            {
                // Пропускаємо видалені під час роботи папки
            }

            return 1 + maxSubDepth;
        }

        // --- Завдання 2: Поділ навпіл ---
        public static int Task2_Solve(int[] a, int lo, int hi, int currentDepth, ref int maxDepth, int threshold)
        {
            if (currentDepth > maxDepth)
            {
                maxDepth = currentDepth;
            }

            // Базовий випадок: залишився один елемент
            if (lo == hi)
            {
                return a[lo] > threshold ? 1 : 0;
            }

            int mid = lo + (hi - lo) / 2;

            int leftCount = Task2_Solve(a, lo, mid, currentDepth + 1, ref maxDepth, threshold);
            int rightCount = Task2_Solve(a, mid + 1, hi, currentDepth + 1, ref maxDepth, threshold);

            return leftCount + rightCount;
        }

        // --- Завдання 3: Перестановки (Backtracking) ---
        public static int Task3_Permutations(char[] items, bool[] used, List<char> current)
        {
            // Базовий випадок: сформовано повну перестановку
            if (current.Count == items.Length)
            {
                Console.WriteLine(string.Join("", current));
                return 1;
            }

            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (!used[i])
                {
                    used[i] = true;             // Вибір
                    current.Add(items[i]);       // Перейти вглиб
                    count += Task3_Permutations(items, used, current);
                    current.RemoveAt(current.Count - 1); // Скасування вибору (Backtrack)
                    used[i] = false;
                }
            }

            return count;
        }

        // --- Завдання 4: Трасування ---
        public static int Task4_TraceSolve(int[] a, int lo, int hi, int depth, int threshold)
        {
            string indent = new string(' ', depth * 2);

            if (lo == hi)
            {
                int val = a[lo] > threshold ? 1 : 0;
                Console.WriteLine($"{indent}enter Solve({lo}..{lo}) -> {val}");
                Console.WriteLine($"{indent}exit  Solve({lo}..{lo}) -> {val}");
                return val;
            }

            Console.WriteLine($"{indent}enter Solve({lo}..{hi})");

            int mid = lo + (hi - lo) / 2;
            int left = Task4_TraceSolve(a, lo, mid, depth + 1, threshold);
            int right = Task4_TraceSolve(a, mid + 1, hi, depth + 1, threshold);

            int result = left + right;
            Console.WriteLine($"{indent}exit  Solve({lo}..{hi}) -> {result}");
            return result;
        }

        // --- Завдання 5: Ітеративна версія (Цикл) ---
        public static int Task5_SolveIterative(int[] a, int threshold)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > threshold)
                {
                    count++;
                }
            }
            return count;
        }
    }
}