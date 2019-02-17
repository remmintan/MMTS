using Core.Models;
using DataProvider;
using System;
using System.IO;
using System.Linq;

namespace TradingBotAnalyzer
{
    internal class Downloader
    {
        public void SaveSamplesToFile(DateTime dt, Timeframe frame, int samplesCount, int delay)
        {
            var dirPath = $"./{frame.ToString()}";
            Directory.CreateDirectory(dirPath);

            var provider = new DataProviderService();
            var samps = provider.GetSamplessWithTimeframeAndStep(dt, frame, samplesCount, delay);
            foreach (var s in samps)
            {
                var r = string.Join('\n', s.Select(i => i.ToString()).ToArray());
                Console.WriteLine("Найдены данные!");
                Console.WriteLine(r);
                File.WriteAllText(dirPath + $"/{s[0].Ticker}_{s[0].DateTime.ToShortDateString().Replace('.', '_')}_{Guid.NewGuid()}.txt", r);
            }
        }
    }
}
