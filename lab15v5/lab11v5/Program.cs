using System;
using System.Collections.Generic;

namespace Lab11
{
    public abstract class GameLevel
    {
        public string LevelName { get; set; }
        public int EnemiesSpawned { get; protected set; }
        public int Score { get; protected set; }
        public bool IsCompleted { get; protected set; }

        protected GameLevel(string levelName)
        {
            LevelName = levelName;
            EnemiesSpawned = 0;
            Score = 0;
            IsCompleted = false;
        }

        public void Play()
        {
            Console.WriteLine($"\n============================================");
            Console.WriteLine($"   СТАРТ РІВНЯ: {LevelName}");
            Console.WriteLine($"============================================");
            
            try
            {
                LoadLevel();
                SpawnEnemies();
                StartTimer();
                CheckWinCondition();
                ShowResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА] Під час гри виникла помилка: {ex.Message}");
            }
        }

        protected abstract void LoadLevel();
        protected abstract void SpawnEnemies();

        protected virtual void StartTimer()
        {
            Console.WriteLine("[Timer] Стандартний таймер запущено: 60 секунд на проходження.");
        }

        protected virtual void CheckWinCondition()
        {
            Console.WriteLine("[Check] Базова перевірка: гравець знищив усіх ворогів.");
            IsCompleted = true;
            Score += 1000;
        }

        protected virtual void ShowResults()
        {
            Console.WriteLine($"\n[Results] Рівень '{LevelName}' завершено!");
            Console.WriteLine($" Status: {(IsCompleted ? "ПЕРЕМОГА" : "ПОРАЗКА")}");
            Console.WriteLine($" Спавнено ворогів: {EnemiesSpawned}");
            Console.WriteLine($" Набрано очок: {Score}");
        }
    }

    public class EasyLevel : GameLevel
    {
        public bool HasAssistMode { get; set; }

        public EasyLevel(string levelName, bool hasAssistMode = true) 
            : base(levelName)
        {
            HasAssistMode = hasAssistMode;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine($"[Easy Load] Завантаження текстур легкого рівня... Режим допомоги: {(HasAssistMode ? "Увімкнено" : "Вимкнено")}.");
        }

        protected override void SpawnEnemies()
        {
            EnemiesSpawned = 5;
            Console.WriteLine($"[Easy Spawn] Заспавнено {EnemiesSpawned} повільних ворогів (слабке здоров'я).");
        }

        protected override void StartTimer()
        {
            Console.WriteLine("[Easy Timer] Таймер не обмежений! Грайте у своєму темпі.");
        }
    }

    public class MediumLevel : GameLevel
    {
        public int CheckpointsCount { get; set; }

        public MediumLevel(string levelName, int checkpointsCount = 3) 
            : base(levelName)
        {
            CheckpointsCount = checkpointsCount;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine($"[Medium Load] Завантаження стандартного рівня з {CheckpointsCount} чекпоінтами.");
        }

        protected override void SpawnEnemies()
        {
            EnemiesSpawned = 15;
            Console.WriteLine($"[Medium Spawn] Заспавнено {EnemiesSpawned} стандартних ворогів із бронею.");
        }

        protected override void CheckWinCondition()
        {
            base.CheckWinCondition();
            Score += 500;
            Console.WriteLine("[Medium Check] Бонус за середній рівень додано!");
        }
    }

    public class HardLevel : GameLevel
    {
        public bool BossFight { get; set; }

        public HardLevel(string levelName, bool bossFight = true) 
            : base(levelName)
        {
            BossFight = bossFight;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine($"[Hard Load] Завантаження складного ландшафту та пасток... Бос: {(BossFight ? "Присутній" : "Відсутній")}.");
        }

        protected override void SpawnEnemies()
        {
            EnemiesSpawned = 35;
            Console.WriteLine($"[Hard Spawn] Заспавнено {EnemiesSpawned} агресивних ворогів та 1 Боса!");
        }

        protected override void StartTimer()
        {
            Console.WriteLine("[Hard Timer] Суворий таймер: усього 30 секунд на проходження!");
        }

        protected override void CheckWinCondition()
        {
            Console.WriteLine("[Hard Check] Перевірка: знищено всіх ворогів ТА подолано Боса!");
            IsCompleted = true;
            Score += 3000;
        }

        protected override void ShowResults()
        {
            base.ShowResults();
            Console.WriteLine(" [HARD BONUS] Отримано досягнення 'Хардкорний боєць'!");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<GameLevel> levels = new List<GameLevel>
            {
                new EasyLevel("Green Hills (Easy)", hasAssistMode: true),
                new MediumLevel("Dark Forest (Medium)", checkpointsCount: 2),
                new HardLevel("Dragon Castle (Hard)", bossFight: true)
            };

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ШАБЛОННОГО МЕТОДУ (GameLevel) ===");

            foreach (var level in levels)
            {
                level.Play();
            }

            Console.WriteLine("\n=================================================");
            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}