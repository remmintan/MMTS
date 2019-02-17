using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer
{
    public class StrategyAnalyzer
    {
        private readonly IndicatorProvider indicatorProvider;

        private decimal balance;
        private IList<StrategyResult.StrategyAction> actionLog;
        private bool hasOpenPosition;

        public StrategyAnalyzer()
        {
            indicatorProvider = new IndicatorProvider();
        }

        public StrategyResult CheckRSIBollingerLongStrategy(IList<TickerData> Data, int RSIMin, int RSIMax, int RSIWindow, int BollingerWindow, bool closeOnStop = true)
        {
            var rsi = indicatorProvider.CalculateRSIForDataAndWindow(Data, RSIWindow);
            var bb = indicatorProvider.CalculateBollingerForDataAndWindow(Data, BollingerWindow);

            balance = 0;
            actionLog = new List<StrategyResult.StrategyAction>();
            hasOpenPosition = false;

            for(var i = 0; i<Data.Count; i++)
            {
                var price = (double)Data[i].Close;
                if (!hasOpenPosition && rsi.RSI[i] < RSIMin && price < bb.Down[i]) openPosition(Data[i].DateTime, Data[i].Close, $"RSI={rsi.RSI[i]:f2} < {RSIMin:f2} и цена = {price:f2} < {bb.Down[i]:f2}");
                if (hasOpenPosition && price > bb.Avg[i]) closePosition(Data[i].DateTime, Data[i].Close, $"цена = {price:f2} выше скользящей средней {bb.Avg[i]:f2}");
            }

            var last = Data.Last();
            if (closeOnStop && hasOpenPosition) closePosition(last.DateTime, last.Close, $"Вынужденно закрываю позицию в связи с окончанием данных.");

            return new StrategyResult(Data[0].Ticker, actionLog, balance);
        }

        private void openPosition(DateTime dt, decimal price, string message, bool isLong = true)
        {
            if (hasOpenPosition) throw new InvalidOperationException("Позиция уже открыта");
            balance += (isLong ? -1 : 1) * price;
            actionLog.Add(new StrategyResult.StrategyAction(dt, true, price, balance, message, true));
            hasOpenPosition = true;
        }

        private void closePosition(DateTime dt, decimal price, string message, bool isLong = true)
        {
            if (!hasOpenPosition) throw new InvalidOperationException("Позиция уже закрыта");
            balance += (isLong ? 1 : -1) * price;
            actionLog.Add(new StrategyResult.StrategyAction(dt, false, price, balance, message, true));
            hasOpenPosition = false;
        }

        //private bool _hasOpenPos = false;
        //public decimal CheckStrategyForRSILong(RSIIndicator rsi, IList<TickerData> data)
        //{
        //    var tradeLog = new List<string>();
        //    var rsiData = rsi.RSI;
        //    var _currentBalance = 0m;
        //    for (var i = 1; i < rsiData.Count; i++)
        //    {
        //        if (!_hasOpenPos && rsiData[i - 1] < 30 && data[i].Close > data[i - 1].Close)
        //        {
        //            _currentBalance -= data[i].Close;
        //            tradeLog.Add($"[{data[i].DateTime}] покупка по цене: {data[i].Close}. Баланс: {_currentBalance}");
        //            _hasOpenPos = true;
        //        }
        //        if (_hasOpenPos && rsiData[i - 1] > 70)
        //        {
        //            _currentBalance += data[i].Close;
        //            tradeLog.Add($"[{data[i].DateTime}] продажа по цене: {data[i].Close}. Баланс: {_currentBalance}");
        //            _hasOpenPos = false;
        //        }
        //    }
        //    if (_hasOpenPos)
        //    {
        //        tradeLog.Add($"[{data.Last().DateTime}] вынужденное закрытие по цене: {data.Last().Close}. Баланс: {_currentBalance}");
        //        _currentBalance += data.Last().Close;
        //        _hasOpenPos = false;
        //    }
        //    Console.WriteLine("Тестирую длинную позицию.");
        //    foreach (var t in tradeLog)
        //    {
        //        Console.WriteLine(t);
        //    }

        //    return _currentBalance;
        //}

        //public decimal CheckStrategyForRSIShort(RSIIndicator rsi, IList<TickerData> data)
        //{
        //    var tradeLog = new List<string>();
        //    var rsiData = rsi.RSI;
        //    var _currentBalance = 0m;
        //    for (var i = 1; i < rsiData.Count; i++)
        //    {
        //        if (!_hasOpenPos && rsiData[i - 1] > 70 && data[i].Close < data[i - 1].Close)
        //        {
        //            _currentBalance += data[i].Close;
        //            tradeLog.Add($"[{data[i].DateTime}] продажа по цене: {data[i].Close}. Баланс: {_currentBalance}");
        //            _hasOpenPos = true;
        //        }
        //        if (_hasOpenPos && rsiData[i - 1] < 30)
        //        {
        //            _currentBalance -= data[i].Close;
        //            tradeLog.Add($"[{data[i].DateTime}] покупка по цене: {data[i].Close}. Баланс: {_currentBalance}");
        //            _hasOpenPos = false;
        //        }
        //    }
        //    if (_hasOpenPos)
        //    {
        //        _currentBalance -= data.Last().Close;
        //        tradeLog.Add($"[{data.Last().DateTime}] вынужденное закрытие по цене: {data.Last().Close}. Баланс: {_currentBalance}");
        //        _hasOpenPos = false;
        //    }
        //    Console.WriteLine("Тестирую короткую позицию.");
        //    foreach (var t in tradeLog)
        //    {
        //        Console.WriteLine(t);
        //    }

        //    return _currentBalance;
        //}
    }
}
