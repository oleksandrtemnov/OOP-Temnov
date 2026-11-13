using System;

namespace OOPLab1
{
    public class City
    {
        private string name;
        private string country;

        public int Population { get; set; }

        public City(string name, string country, int population)
        {
            this.name = name;
            this.country = country;
            Population = population;
        }

        ~City()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт міста {name} видалено з пам'яті.");
        }

        public string GetInfo()
        {
            return $"Місто: {name} | Країна: {country} | Населення: {Population:N0} осіб";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            City city1 = new City("Київ", "Україна", 2950000);
            City city2 = new City("Рівне", "Україна", 243000);
            City city3 = new City("Токіо", "Японія", 13960000);

            Console.WriteLine("=== Інформація про міста ===");
            Console.WriteLine(city1.GetInfo());
            Console.WriteLine(city2.GetInfo());
            Console.WriteLine(city3.GetInfo());
        }
    }
}