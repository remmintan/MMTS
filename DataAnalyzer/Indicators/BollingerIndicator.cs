using System.Collections.Generic;

namespace DataAnalyzer.Indicators
{
    internal class BollingerIndicator
    {
        public BollingerIndicator(IList<double> Avg, IList<double> Up, IList<double> Down)
        {
            this.Avg = Avg;
            this.Up = Up;
            this.Down = Down;
        }

        public IList<double> Avg { get; private set; }
        public IList<double> Up { get; private set; }
        public IList<double> Down { get; private set; }

    }
}
