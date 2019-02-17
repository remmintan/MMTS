using Core.Models;
using DataAnalyzer;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace TradingBotAnalyzer
{
    public class Tester
    {
        public void TestStrategy(string dataFilePath, int RSIMin, int RSIMax, int RSIWindow, int BBWindow, int delay, bool showMessages)
        {
            var data = File.ReadLines(dataFilePath).Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => TickerData.FromString(a)).ToList();
            var analyzer = new StrategyAnalyzer();

            var result = analyzer.CheckRSIBollingerLongStrategy(data, RSIMin, RSIMax, RSIWindow, BBWindow, closeOnStop:false);
            var log = result.ActionLog;
            var actionsDict = log.ToDictionary(l => l.DateTime);

            var balance = 0m;

            Console.WriteLine("Начинаю симуляцию стратегии");
            foreach(var p in data)
            {
                var dt = p.DateTime;
                var action = actionsDict.ContainsKey(dt) ? actionsDict[dt] : null;

                if (action != null)
                {
                    balance = action.BalanceAfter;
                }

                Console.WriteLine($"[{dt.ToShortDateString()} {dt.ToShortTimeString()}] Цена: {p.Close} ");
                if (showMessages) Console.WriteLine(action.Message);
                Console.WriteLine($"[БОТ] {(action!=null?(action.IsOpen?"Покупай! ":"Продавай! "):"")}Баланс: {balance}");
                Thread.Sleep(delay);
            }
            Console.WriteLine($"Симуляция завершена.");
        }
    }
}
