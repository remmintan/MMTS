using Core.Models;
using System;

namespace DataProvider
{
    internal class RequestUrlHelperService
    {
        public Ticker Tick { get; set; }
        public Timeframe Timeframe { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string URL
        {
            get
            {
                var url = "https://mfd.ru/export/handler.ashx/ГорФондНед_15min_04022019_04022019.txt?" +
                    "TickerGroup=16&" +
                    $"Tickers={Tick.Id}&" +
                    "Alias=false&" +
                    $"Period={(int)Timeframe}&" +
                    "timeframeValue=1&" +
                    "timeframeDatePart=day&" +
                    $"StartDate={StartDate:dd.MM.yyyy}&" +
                    $"EndDate={EndDate:dd.MM.yyyy}&" +
                    "SaveFormat=0&" +
                    "SaveMode=0&" +
                    "FileName=ГорФондНед_1hour_04022019_04022019.txt&" +
                    "FieldSeparator=%3b&" +
                    "DecimalSeparator=,&" +
                    "DateFormat=dd'/'MM'/'yy&" +
                    "TimeFormat=HH:mm&" +
                    "AddHeader=false&" +
                    "RecordFormat=2&" +
                    "Fill=true";

                return url;
            }
        }
    }
}
