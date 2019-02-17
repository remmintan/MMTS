using Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataProvider
{
    internal class DataParserService
    {
        public IList<TickerData> ParseData(string input)
        {
            return input.Split('\n')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(_lineToData)
                .ToList();
        }

        //формат времени строки: Омскшина;D;06/08/18;00:00;472;10
        private TickerData _lineToData(string line)
        {
            var p = line.Split(';');
            var name = p[0];
            var dt = DateTime.ParseExact(p[2] + " " + p[3], "dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
            var close = decimal.Parse(p[4]);
            var vol = long.Parse(p[5]);

            return new TickerData(name, dt, close, vol);
        }
    }
}
