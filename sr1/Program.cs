using System;
using System.Collections.Generic;

namespace IndependentWork1
{
    public class Employee
    {
        private string _fullName;
        private double _baseSalary;
        private int _experienceYears;

        public string FullName
        {
            get { return _fullName; }
        }

        public double BaseSalary
        {
            get { return _baseSalary; }
            set
            {
                if (value >= 0) _baseSalary = value;
            }
        }

        public Employee(string fullName, double baseSalary, int experienceYears)
        {
            _fullName = fullName;
            _baseSalary = baseSalary;
            _experienceYears = experienceYears;
        }

        public double CalculateTotalSalary()
        {
            double bonusRate = _experienceYears * 0.05;
            return _baseSalary * (1 + bonusRate);
        }
    }

    public class Playlist
    {
        private string _title;
        private int _trackCount;
        private int _totalDurationSeconds;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public int TrackCount
        {
            get { return _trackCount; }
        }

        public Playlist(string title, int trackCount, int totalDurationSeconds)
        {
            _title = title;
            _trackCount = trackCount;
            _totalDurationSeconds = totalDurationSeconds;
        }

        public string GetFormattedDuration()
        {
            int minutes = _totalDurationSeconds / 60;
            int seconds = _totalDurationSeconds % 60;
            return minutes + " хв " + seconds + " сек";
        }
    }

    public class Recipe
    {
        private string _dishName;
        private int _servings;
        private int _cookingTimeMinutes;

        public string DishName
        {
            get { return _dishName; }
        }

        public int Servings
        {
            get { return _servings; }
            set
            {
                if (value > 0) _servings = value;
            }
        }

        public Recipe(string dishName, int servings, int cookingTimeMinutes)
        {
            _dishName = dishName;
            _servings = servings;
            _cookingTimeMinutes = cookingTimeMinutes;
        }

        public bool IsQuickRecipe()
        {
            return _cookingTimeMinutes <= 30;
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Employee emp = new Employee("Олександр Темнов", 25000, 4);
            Console.WriteLine("Працівник: " + emp.FullName);
            Console.WriteLine("Базова ставка: " + emp.BaseSalary + " грн");
            Console.WriteLine("Зарплата з урахуванням стажу: " + emp.CalculateTotalSalary() + " грн");
            Console.WriteLine();

            Playlist playlist = new Playlist("Улюблені треки", 12, 2350);
            Console.WriteLine("Плейлист: " + playlist.Title);
            Console.WriteLine("Кількість треків: " + playlist.TrackCount);
            Console.WriteLine("Загальна тривалість: " + playlist.GetFormattedDuration());
            Console.WriteLine();

            Recipe recipe = new Recipe("Борщ український", 6, 90);
            Console.WriteLine("Рецепт: " + recipe.DishName);
            Console.WriteLine("Порцій: " + recipe.Servings);
            Console.WriteLine("Швидкий рецепт (<= 30 хв): " + (recipe.IsQuickRecipe() ? "Так" : "Ні"));
        }
    }
}