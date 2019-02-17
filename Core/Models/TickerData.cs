using System;
using System.Globalization;

namespace Core.Models
{
    public class TickerData
    {
        public readonly string Ticker;
        public readonly DateTime DateTime;
        public readonly decimal Close;
        public readonly long Volume;

        public TickerData(string ticker, DateTime dateTime, decimal close, long volume)
        {
            Ticker = ticker;
            DateTime = dateTime;
            Close = close;
            Volume = volume;
        }

        public override string ToString()
        {
            return $"{Ticker}\t{DateTime:dd/MM/yyyy}\t{DateTime:HH:mm}\t{Close}\t{Volume}";
        }

        public static TickerData FromString(string line)
        {
            var p = line.Split('\t');
            var dt = DateTime.ParseExact(p[1] + " " + p[2], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            var c = decimal.Parse(p[3]);
            var v = long.Parse(p[4]);

            return new TickerData(p[0], dt, c, v);
        }
    }
}
