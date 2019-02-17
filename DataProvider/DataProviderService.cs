using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataProvider
{
    public class DataProviderService
    {
        private const int MAX_TRIES = 3;

        private readonly RequestUrlHelperService urlService;
        private readonly TickerService tickerService;
        private readonly ApiClient client;
        private readonly DataParserService parserService;

        public DataProviderService()
        {
            urlService = new RequestUrlHelperService();
            tickerService = new TickerService();
            client = new ApiClient();
            parserService = new DataParserService();
        }

        public IEnumerable<IList<TickerData>> GetSamplessWithTimeframeAndStep(DateTime ed, Timeframe Frame, int SamplesCount, int RequestDelay = 1000)
        {
            var StepInDays = _getStepForTimeframe(Frame);

            var tries = 0;
            var endDate = ed;
            var startDate = endDate.AddDays(-StepInDays);

            while (true)
            {
                Console.WriteLine("Получаю список тикеров");
                var tickers = tickerService.ReadAllTickersFromFile();

                urlService.StartDate = startDate;
                urlService.EndDate = endDate;
                urlService.Timeframe = Frame;
                
                var random = new Random();
                var cnt = 0;
                
                while (true)
                {
                    var i = random.Next(tickers.Count);
                    var t = tickers[i];
                    tickers.RemoveAt(i);

                    urlService.Tick = t;
                    Console.WriteLine($"Получаю данные для тикера: {t.Name}");
                    var r = client.GetTickerData(urlService.URL);
                    var data = parserService.ParseData(r);
                    if (data.Count > 0)
                    {
                        var avg = data.Select(d => d.Volume).Where(v => v > 0).Average();
                        var nonNullDays = data.Select(d => d.Volume).Count(v => v > 0);
                        var nullDays = data.Select(d => d.Volume).Count(v => v == 0);
                        if (nonNullDays * _getNonNullMultiplierForTimeframe(Frame) < nullDays || avg < _getAverageVolumeForTimeframe(Frame))
                        {
                            Console.WriteLine($"Объем торгов для данных слишком мал: {avg} {nonNullDays} - периодов с торгами; {nullDays} - периодов без торгов");
                        }
                        else
                        {
                            cnt++;
                            yield return data;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Недостаточно данных");
                    }

                    if (cnt >= SamplesCount || tickers.Count == 0) break;
                    Thread.Sleep(RequestDelay);
                }
                if (cnt >= SamplesCount) break;


                Console.WriteLine("Не удалось найти достаточно данных на этом промежутке. Отхожу по времени на шаг назад.");
                startDate = startDate.AddDays(-StepInDays);
                endDate = endDate.AddDays(-StepInDays);

                if (++tries > 3) throw new Exception("Не удалось найти данные, удовлетворяющие требованиям");
            }

        }

        

        private int _getAverageVolumeForTimeframe(Timeframe frame)
        {
            switch (frame)
            {
                case Timeframe.DAY: return 5000;
                case Timeframe.HOUR: return 200;
                case Timeframe.MINUTE15: return 50;
                default: throw new NotImplementedException();
            }
        }

        private int _getStepForTimeframe(Timeframe frame)
        {
            switch (frame)
            {
                case Timeframe.DAY: return 180;
                case Timeframe.HOUR: return 30;
                case Timeframe.MINUTE15: return 10;
                default: throw new NotImplementedException();
            }
        }

        private int _getNonNullMultiplierForTimeframe(Timeframe frame)
        {
            switch (frame)
            {
                case Timeframe.DAY: return 1;
                case Timeframe.HOUR: return 3;
                case Timeframe.MINUTE15: return 3;
                default: throw new NotImplementedException();
            }
        }
    }
}
