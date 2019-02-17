using Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataProvider
{
    internal class TickerService
    {
        public IList<Ticker> ReadAllTickersFromFile()
        {
            return File.ReadLines("./tickers.dat").Select(_lineToTicker).ToList();
        }

        private Ticker _lineToTicker(string line)
        {
            var p = line.Split(';');
            return new Ticker
            {
                Id = int.Parse(p[0]),
                Name = p[1]
            };
        }
    }
}
