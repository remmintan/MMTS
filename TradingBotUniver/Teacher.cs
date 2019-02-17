using Core.Models;
using DataAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TradingBotAnalyzer
{
    internal class Teacher
    {
        private IDictionary<string, int> Fails = new Dictionary<string, int>();
        public void GenerateTrainingInfo(int RepeatsCount, Timeframe frame)
        {
            
            var analyzer = new StrategyAnalyzer();
            var samples = ReadAllDataFromTimeframeFolder(frame).ToList();

            var random = new Random();
            int RSIMin, RSIMax, RSIWindow, BBWindow;

            Directory.CreateDirectory($"./teach{frame}");
            for (var i = 0; i < RepeatsCount; i++)
            {
                RSIMin = 20 + random.Next(21);
                RSIMax = 50 + random.Next(21);
                RSIWindow = 5 + random.Next(21);
                BBWindow = 5 + random.Next(21);
                Console.WriteLine($"Проверяю для параметров. RSIMIN {RSIMin}; RSIMAX {RSIMax}; RSIWIN{RSIWindow}; BBWIN{BBWindow}");
                _testStrategyAndSaveToFile(samples, RSIMin, RSIMax, RSIWindow, BBWindow, analyzer, frame);
            }
            
            var fileName = $"./teach{frame}/{DateTime.Now.ToShortDateString().Replace('.', '_')}_{DateTime.Now.ToShortTimeString().Replace(":", "")}_{frame}_TOP_FAILS_COMPILATION.log";
            var log = Fails.OrderBy(p => p.Value).Select(p => p.Key + " - " + p.Value);
            File.WriteAllLines(fileName, log);
        }

        private  void _testStrategyAndSaveToFile(IList<IList<TickerData>> samples, int RSIMin, int RSIMax, int RSIWindow, int BBWindow, StrategyAnalyzer analyzer, Timeframe frame)
        {
            var results = new HashSet<StrategyResult>();
            foreach (var s in samples)
            {
                var r = analyzer.CheckRSIBollingerLongStrategy(s, RSIMin, RSIMax, RSIWindow, BBWindow);
                if (!r.IsSuccess)
                {
                    if (!Fails.ContainsKey(r.TickerName)) Fails[r.TickerName] = 0;
                    Fails[r.TickerName]++;
                }
                results.Add(r);
            }

            var winRate = (double)results.Count(r => r.IsSuccess) / results.Count() * 100;
            var averageProfit = results.Average(r => r.Result);
            var maxProfit = results.Max(r => r.Result);
            var minProfit = results.Min(r => r.Result);

            var fileName = $"./teach{frame}/{winRate:f2}_{DateTime.Now.ToShortDateString().Replace('.', '_')}_{DateTime.Now.ToShortTimeString().Replace(":", "")}_{frame}_RSI[{RSIWindow} {RSIMin} {RSIMax}]_BB[{BBWindow}].log";
            var log = new List<string>();

            log.Add("Лог проверки стратегии BB RSI");
            log.Add("Данные стратегии");
            log.Add($"RSI Window: {RSIWindow}; Min: {RSIMin}; Max: {RSIMax};");
            log.Add($"BB Window: {BBWindow}");
            log.Add($"Success rate: {winRate:f2}");
            log.Add($"Results. Average: {averageProfit:f2}; Min: {minProfit:f2}; Max: {maxProfit:f2};");
            log.Add($"");
            log.Add($"");

            foreach (var r in results)
            {
                log.Add($"Результаты обработки тикера {r.TickerName}");
                log.Add($"Успех: {r.IsSuccess}");
                log.Add($"Финансовый результат: {r.Result} руб. на акцию.");
                log.Add($"Баланс. Максимальный подъем: {r.Max}. Максимальная просадка {r.Min}");
                log.Add($"Требуемые вложения на счет {r.MaxSpent} руб. на акцию");
                log.Add($"Количество операций: {r.OperationsCount}");
                log.Add($"");
                log.Add($"Подробный лог операций:");
                if (r.ActionLog.Count > 0)
                {
                    foreach (var a in r.ActionLog)
                    {
                        log.Add($"[{a.DateTime.ToShortDateString()} {a.DateTime.ToShortTimeString()}] {a.Message}");
                        log.Add($"[{a.DateTime.ToShortDateString()} {a.DateTime.ToShortTimeString()}] {(a.IsOpen ? "Открываю" : "Закрываю")} {(a.IsLong ? "длинную" : "короткую")} позицию по цене {a.Price}. Баланс: {a.BalanceAfter}");
                    }
                }
                else
                {
                    log.Add("Операций не было");
                }
                log.Add($"");
                log.Add($"");
            }

            File.AppendAllLines(fileName, log);
        }

        private IEnumerable<IList<TickerData>> ReadAllDataFromTimeframeFolder(Timeframe frame)
        {
            var files = Directory.GetFiles($"./{frame}");

            foreach (var f in files)
            {
                yield return File.ReadLines(f).Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => TickerData.FromString(a)).ToList();
            }
        }
    }
}
