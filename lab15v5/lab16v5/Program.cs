using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab16V5
{
    public class Landmark
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Landmark(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Population { get; set; }
        public List<Landmark> Landmarks { get; set; } = new List<Landmark>();

        public City(int id, string name, long population, List<Landmark> landmarks)
        {
            Id = id;
            Name = name;
            Population = population;
            Landmarks = landmarks;
        }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<City> Cities { get; set; } = new List<City>();

        public Country(int id, string name, List<City> cities)
        {
            Id = id;
            Name = name;
            Cities = cities;
        }
    }

    public static class CityExtensions
    {
        public static long TotalPopulation(this IEnumerable<City> cities)
        {
            if (cities == null) return 0;
            return cities.Sum(c => c.Population);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("    ЛАБОРАТОРНА РОБОТА №16 | ВАРІАНТ 5            ");
            Console.WriteLine("    ТЕМА: LINQ: SelectMany, пагінація й extension ");
            Console.WriteLine("==================================================\n");

            List<Country> countries = GetSampleData();

            Console.WriteLine("--- 1. SELECTMANY: Отримання всіх пам'яток в одному списку ---");

            var allLandmarks = countries
                .SelectMany(country => country.Cities)
                .SelectMany(city => city.Landmarks)
                .ToList();

            Console.WriteLine($"Усього знайдено пам'яток у всіх країнах: {allLandmarks.Count}");
            int landmarkIndex = 1;
            foreach (var landmark in allLandmarks)
            {
                Console.WriteLine($"  {landmarkIndex++}. [ID:{landmark.Id}] {landmark.Name}");
            }

            Console.WriteLine("\n" + new string('-', 55) + "\n");

            Console.WriteLine("--- 2. ПАГІНАЦІЯ: Вивід міст по 4 на сторінку ---");

            List<City> allCities = countries
                .SelectMany(country => country.Cities)
                .ToList();

            int pageSize = 4;
            int totalCities = allCities.Count;
            int totalPages = (int)Math.Ceiling((double)totalCities / pageSize);

            for (int currentPage = 1; currentPage <= totalPages; currentPage++)
            {
                Console.WriteLine($"\n--- Сторінка {currentPage} з {totalPages} ---");

                var pageCities = allCities
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize);

                int cityNumber = ((currentPage - 1) * pageSize) + 1;
                foreach (var city in pageCities)
                {
                    Console.WriteLine($"  {cityNumber++}. Місто: {city.Name,-12} | Населення: {city.Population,10:N0} | Пам'яток: {city.Landmarks.Count}");
                }
            }

            Console.WriteLine("\n" + new string('-', 55) + "\n");

            Console.WriteLine("--- 3. CUSTOM EXTENSION: Обчислення загального населення ---");

            long grandTotalPopulation = allCities.TotalPopulation();
            Console.WriteLine($"1. Загальне населення всіх міст у базі: {grandTotalPopulation:N0} осіб.");

            Console.WriteLine("\n2. Населення по країнах (із застосуванням TotalPopulation()):");
            foreach (var country in countries)
            {
                long countryPopulation = country.Cities.TotalPopulation();
                Console.WriteLine($"   - {country.Name,-10}: {countryPopulation,10:N0} осіб (Міст: {country.Cities.Count})");
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("    ЛАБОРАТОРНУ РОБОТУ УСПІШНО ВИКОНАНО!          ");
            Console.WriteLine("==================================================");
        }

        private static List<Country> GetSampleData()
        {
            return new List<Country>
            {
                new Country(1, "Україна", new List<City>
                {
                    new City(101, "Київ", 2950000, new List<Landmark>
                    {
                        new Landmark(1, "Києво-Печерська лавра"),
                        new Landmark(2, "Софійський собор")
                    }),
                    new City(102, "Львів", 717000, new List<Landmark>
                    {
                        new Landmark(3, "Ансамбль площі Ринок"),
                        new Landmark(4, "Оперний театр")
                    }),
                    new City(103, "Одеса", 1010000, new List<Landmark>
                    {
                        new Landmark(5, "Потьомкінські сходи"),
                        new Landmark(6, "Одеський оперний театр")
                    }),
                    new City(104, "Рівне", 243000, new List<Landmark>
                    {
                        new Landmark(7, "Парк Лебединка"),
                        new Landmark(8, "Тараканівський форт (поблизу)")
                    })
                }),

                new Country(2, "Франція", new List<City>
                {
                    new City(201, "Париж", 2161000, new List<Landmark>
                    {
                        new Landmark(9, "Ейфелева вежа"),
                        new Landmark(10, "Лувр"),
                        new Landmark(11, "Собор Паризької Богоматері")
                    }),
                    new City(202, "Марсель", 870000, new List<Landmark>
                    {
                        new Landmark(12, "Замок Іф"),
                        new Landmark(13, "Базиліка Нотр-Дам-де-ла-Ґард")
                    }),
                    new City(203, "Ліон", 522000, new List<Landmark>
                    {
                        new Landmark(14, "Амфітеатр трьох Галлій"),
                        new Landmark(15, "Базиліка Нотр-Дам-де-Фурв'єр")
                    }),
                    new City(204, "Ніцца", 342000, new List<Landmark>
                    {
                        new Landmark(16, "Англійська набережна")
                    })
                })
            };
        }
    }
}