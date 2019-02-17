using Core.Models;
using DataAnalyzer.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer
{
    internal class IndicatorProvider
    {
        public RSIIndicator CalculateRSIForDataAndWindow(IList<TickerData> Data, int Window)
        {
            var U = new List<decimal>();
            var D = new List<decimal>();

            var EMAU = new List<double>();
            var EMAD = new List<double>();
            var RS = new List<double>();
            var RSI = new List<double>();


            U.Add(-1m);
            D.Add(-1m);
            for (var i = 1; i < Data.Count; i++)
            {
                var y = Data[i - 1].Close;
                var t = Data[i].Close;
                U.Add(t > y ? t - y : 0);
                D.Add(y > t ? y - t : 0);
            }

            for (var i = 0; i < Data.Count; i++)
            {
                if (i <= Window)
                {
                    EMAU.Add(-1);
                    EMAD.Add(-1);
                    RS.Add(-1);
                    RSI.Add(-1);
                }
                else
                {
                    if (i == Window + 1)
                    {
                        EMAU.Add((double)U.Skip(1).Take(Window).Average());
                        EMAD.Add((double)D.Skip(1).Take(Window).Average());
                    }
                    else
                    {
                        EMAU.Add(EMAU[i - 1] + ((double)1 / (Window + 1) * ((double)U[i] - EMAU[i - 1])));
                        EMAD.Add(EMAD[i - 1] + ((double)1 / (Window + 1) * ((double)D[i] - EMAD[i - 1])));
                    }

                    RS.Add(EMAU[i] / EMAD[i]);
                    RSI.Add(100 - 100 / (1 + RS[i]));
                }

            }

            return new RSIIndicator(U, D, EMAU, EMAD, RS, RSI);
        }

        public BollingerIndicator CalculateBollingerForDataAndWindow(IList<TickerData> Data, int Window)
        {
            var Avg = new List<double>();
            var Up = new List<double>();
            var Down = new List<double>();
            var avg = 0d;
            for (var i = 0; i < Data.Count; i++)
            {
                if (i < Window - 1)
                {
                    Avg.Add(-1);
                    Up.Add(-1);
                    Down.Add(-1);
                    avg += (double)Data[i].Close / Window;
                }
                else
                {
                    avg += (double)Data[i].Close / Window;
                    if (i >= Window) avg -= (double)Data[i - Window].Close / Window;
                    Avg.Add(avg);
                    var disp = 0d;
                    for (var j = i - Window + 1; j <= i; j++)
                    {
                        disp += Math.Pow((double)Data[j].Close - avg, 2);
                    }
                    disp /= Window;
                    var std = Math.Sqrt(disp);
                    Up.Add(avg + std);
                    Down.Add(avg - std);
                }
            }

            return new BollingerIndicator(Avg, Up, Down);
        }
    }
}
