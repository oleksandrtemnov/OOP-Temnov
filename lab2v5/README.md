віт з Лабораторної роботи №2
Тема: Клас із кількома конструкторами. Життєвий цикл об’єкта.
Навчальний заклад: Рівненський фаховий коледж інформаційних технологій
Дисципліна: Об’єктно-орієнтоване програмування
Варіант: №5 (Клас City)
Виконав: студент групи КН 3/1 Темнов Олександр
Посилання на GitHub: [https://github.com/oleksandrtemnov/OOP-Temnov/tree/main/lab2v5](https://github.com/oleksandrtemnov/OOP-Temnov/tree/main/lab2v5)

1. Мета роботи
Ознайомитися з перевантаженням конструкторів у C#, опанувати механізм ланцюгового виклику за допомогою : this(), навчитися реалізовувати властивості з валідацією даних, а також дослідити життєвий цикл об’єктів та механізм роботи Збирача Сміття (Garbage Collector).

2. Повний код програми (Program.cs)
C#
using System;
using System.Text;

namespace lab2v5
{
    public class City
    {
        private string _name;
        private string _country;
        private long _population;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Country
        {
            get => _country;
            set => _country = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public long Population
        {
            get => _population;
            set
            {
                if (value < 0)
                {
                    Console.WriteLine($"[Увага] Населення для {Name} не може бути відємним ({value}). Встановлено 0.");
                    _population = 0;
                }
                else
                {
                    _population = value;
                }
            }
        }

        public City(string name, string country, long population)
        {
            Name = name;
            Country = country;
            Population = population;
            Console.WriteLine($"[Конструктор] Створено обєкт City: {Name}");
        }

        public City() : this("Unknown", "Unknown", 0)
        {
        }

        public City(string name, string country) : this(name, country, 0)
        {
        }

        public string GetCityInfo()
        {
            return $"Місто: {Name} | Країна: {Country} | Населення: {Population:N0} осіб";
        }

        ~City()
        {
            Console.WriteLine($"[Деструктор] Обєкт City ({_name}) знищено з памяті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Створення обєктів ===");

            City city1 = new City();
            City city2 = new City("Київ", "Україна", 2950000);
            City city3 = new City("Рівне", "Україна", -100);

            Console.WriteLine("\n=== Демонстрація роботи методів ===");
            Console.WriteLine(city1.GetCityInfo());
            Console.WriteLine(city2.GetCityInfo());
            Console.WriteLine(city3.GetCityInfo());

            Console.WriteLine("\n=== Демонстрація життєвого циклу обєктів та GC ===");

            city1 = null;
            city2 = null;
            city3 = null;

            Console.WriteLine("Примусовий виклик Garbage Collector...");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Завершення виконання програми.");
        }
    }
}
3. Результати виконання програми (Консольний вивід)
Plaintext
=== Створення обєктів ===
[Конструктор] Створено обєкт City: Unknown
[Конструктор] Створено обєкт City: Київ
[Увага] Населення для Рівне не може бути відємним (-100). Встановлено 0.
[Конструктор] Створено обєкт City: Рівне

=== Демонстрація роботи методів ===
Місто: Unknown | Країна: Unknown | Населення: 0 осіб
Місто: Київ | Країна: Україна | Населення: 2,950,000 осіб
Місто: Рівне | Країна: Україна | Населення: 0 осіб

=== Демонстрація життєвого циклу обєктів та GC ===
Примусовий виклик Garbage Collector...
[Деструктор] Обєкт City (Рівне) знищено з памяті.
[Деструктор] Обєкт City (Київ) знищено з памяті.
[Деструктор] Обєкт City (Unknown) знищено з памяті.
Завершення виконання програми.
Висновок:
У ході виконання лабораторної роботи №2 було практично опановано створення класів із перевантаженими конструкторами та досліджено особливості життєвого циклу об’єктів у мові C#.
Конструктори та ланцюговий виклик: За допомогою синтаксису : this(...) реалізовано ланцюговий виклик конструкторів. Це дозволило уникнути дублювання коду ініціалізації та централізовано передавати значення через єдиний параметризований конструктор.
Інкапсуляція та валідація: Реалізовано властивості з перевіркою коректності вхідних даних (валідація від’ємного значення населення у властивості Population), що забезпечує цілісність стану об’єкта.
Життєвий цикл та Garbage Collector: На прикладі роботи деструктора (фіналізатора ~City()) та методів GC.Collect() і GC.WaitForPendingFinalizers() наочно простежено процес вивільнення пам'яті Збирачем Сміття для об'єктів, на які більше немає активних посилань у програмі.
