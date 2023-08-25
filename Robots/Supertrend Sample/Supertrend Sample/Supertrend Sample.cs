// -------------------------------------------------------------------------------------------------
//
//    This code is a cTrader Automate API example.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//
// -------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SupertrendSample : Robot
    {
        private double _volumeInUnits;

        private Supertrend _supertrend;

        [Parameter("Volume (Lots)", DefaultValue = 0.01)]
        public double VolumeInLots { get; set; }

        [Parameter("Stop Loss (Pips)", DefaultValue = 10, MaxValue = 100, MinValue = 1, Step = 1)]
        public double StopLossInPips { get; set; }

        [Parameter("Take Profit (Pips)", DefaultValue = 10, MaxValue = 100, MinValue = 1, Step = 1)]
        public double TakeProfitInPips { get; set; }

        [Parameter("Label", DefaultValue = "SupertrendSample")]
        public string Label { get; set; }

        [Parameter("Periods", DefaultValue = 10, Group = "Supertrend", MinValue = 1)]
        public int Periods { get; set; }

        [Parameter("Multiplier", DefaultValue = 3.0, Group = "Supertrend")]
        public double Multiplier { get; set; }


        public Position[] BotPositions
        {
            get
            {
                return Positions.FindAll(Label);
            }
        }

        protected override void OnStart()
        {
            _volumeInUnits = Symbol.QuantityToVolumeInUnits(VolumeInLots);

            _supertrend = Indicators.Supertrend(Periods, Multiplier);
        }

        protected override void OnBarClosed()
        {
            if (_supertrend.UpTrend.Last(0) < Bars.LowPrices.Last(0) && _supertrend.DownTrend.Last(1) > Bars.HighPrices.Last(1))
            {
                ClosePositions(TradeType.Sell);

                ExecuteMarketOrder(TradeType.Buy, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
            }
            else if (_supertrend.DownTrend.Last(0) > Bars.HighPrices.Last(0) && _supertrend.UpTrend.Last(1) < Bars.LowPrices.Last(1))
            {
                ClosePositions(TradeType.Buy);

                ExecuteMarketOrder(TradeType.Sell, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
            }
        }

        private void ClosePositions(TradeType tradeType)
        {
            foreach (var position in BotPositions)
            {
                if (position.TradeType != tradeType) continue;

                ClosePosition(position);
            }
        }
    }
}