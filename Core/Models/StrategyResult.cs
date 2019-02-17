using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class StrategyResult
    {
        public StrategyResult(string TickerName, IList<StrategyAction> actionLog, decimal result)
        {
            this.TickerName = TickerName;
            ActionLog = actionLog;
            Result = result;
        }

        public string TickerName { get; }
        public bool IsSuccess => Result > 0;
        public IList<StrategyAction> ActionLog { get; }
        public decimal Result { get; }
        public decimal Max => ActionLog.Count>0?ActionLog.Where(a => !a.IsOpen).Select(a => a.BalanceAfter).Max():0;
        public decimal Min => ActionLog.Count > 0 ? ActionLog.Where(a => !a.IsOpen).Select(a => a.BalanceAfter).Min():0;
        public decimal MaxSpent => ActionLog.Count > 0 ? -ActionLog.Where(a => a.IsOpen).Select(a => a.BalanceAfter).Min():0;
        public decimal OperationsCount => ActionLog.Count;

        public class StrategyAction
        {
            public StrategyAction(DateTime dateTime, bool isOpen, decimal price, decimal balanceAfter, string message, bool isLong)
            {
                Message = message;
                DateTime = dateTime;
                IsOpen = isOpen;
                Price = price;
                BalanceAfter = balanceAfter;
                IsLong = isLong;
            }

            public DateTime DateTime { get; }
            public bool IsOpen { get; }
            public decimal Price { get; }
            public decimal BalanceAfter { get; }
            public bool IsLong { get; }
            public string Message { get; }
        }

    }
}
