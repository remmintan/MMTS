using Core.Models;
using System;
using System.Globalization;

namespace TradingBotAnalyzer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Выберите режим работы порграммы");
            Console.WriteLine("1. Скачивание данных");
            Console.WriteLine("2. Обучение на данных");
            Console.WriteLine("3. Торговля");
            Console.WriteLine("Введите номер [1-3]");
            var i = int.Parse(Console.ReadLine());
            switch (i)
            {
                case 1:
                    _startDownload();
                    break;
                case 2:
                    _startTeach();
                    break;
                case 3:
                    _startDemo();
                    break;
            }
            Console.WriteLine("Работа завершена");
            Console.ReadKey();
        }

        private static void _startDownload()
        {
            var d = new Downloader();
            var dt = _getDate();
            var frame = _getTimeFrame();
            Console.WriteLine("Введите количество образцов данных которые нужно скачать: ");
            var samples = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите задержку на скачивание в мс: ");
            var delay = int.Parse(Console.ReadLine());
            
            d.SaveSamplesToFile(dt, frame, samples, delay);

            
        }

        private static void _startTeach()
        {
            var t = new Teacher();
            var frame = _getTimeFrame();
            Console.WriteLine("Введите количество повторений случайных тестов: ");
            var reps = int.Parse(Console.ReadLine());
            t.GenerateTrainingInfo(reps, frame);
        }

        private static void _startDemo()
        {
            Console.WriteLine("Введите названия файла с входными данными: ");
            var name = Console.ReadLine();
            Console.WriteLine("RSIMin: ");
            var rsiMin = int.Parse(Console.ReadLine());
            Console.WriteLine("RSIMax: ");
            var rsiMax = int.Parse(Console.ReadLine());
            Console.WriteLine("RSIWindow: ");
            var RSIWindow = int.Parse(Console.ReadLine());
            Console.WriteLine("BBWindow: ");
            var BBWindow = int.Parse(Console.ReadLine());
            Console.WriteLine("Задержка при выводе информации: ");
            var delay = int.Parse(Console.ReadLine());
            Console.WriteLine("Выводить сообщения о решениях: ");
            var m = bool.Parse(Console.ReadLine());

            var t = new Tester();
            t.TestStrategy(name, rsiMin, rsiMax, RSIWindow, BBWindow, delay, m);
            
        }

        private static DateTime _getDate()
        {
            Console.WriteLine("Введите дату в формате dd.MM.yyyy: ");
            var ds = Console.ReadLine();
            return DateTime.ParseExact(ds, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        private static Timeframe _getTimeFrame()
        {
            Console.WriteLine("Введите таймфрейм, который хотите использовать: ");
            Console.WriteLine("1. День");
            Console.WriteLine("2. Час");
            Console.WriteLine("3. 15 минут");
            var i = int.Parse(Console.ReadLine());
            switch (i)
            {
                case 1: return Timeframe.DAY;
                case 2: return Timeframe.HOUR;
                case 3: return Timeframe.MINUTE15;
                default: throw new Exception();
            }
        }

    }
}
