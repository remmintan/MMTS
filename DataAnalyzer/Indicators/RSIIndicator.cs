using System.Collections.Generic;

namespace DataAnalyzer.Indicators
{
    internal class RSIIndicator
    {
        public RSIIndicator(IList<decimal> U, IList<decimal> D, IList<double> EMAU, IList<double> EMAD, IList<double> RS, IList<double> RSI)
        {
            this.U = U;
            this.D = D;
            this.EMAU = EMAU;
            this.EMAD = EMAD;
            this.RS = RS;
            this.RSI = RSI;
        }

        public IList<decimal> U { get; private set; }
        public IList<decimal> D { get; private set; }
        public IList<double> EMAU { get; private set; }
        public IList<double> EMAD { get; private set; }
        public IList<double> RS { get; private set; }
        public IList<double> RSI { get; private set; }

    }
}
