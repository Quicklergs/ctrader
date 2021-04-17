﻿using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    /// <summary>
    /// This sample cBot shows how to use the Time Series Moving Average indicator
    /// </summary>
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TimeSeriesMovingAverageSample : Robot
    {
        private double _volumeInUnits;

        private TimeSeriesMovingAverage _fastTimeSeriesMovingAverage;

        private TimeSeriesMovingAverage _slowTimeSeriesMovingAverage;

        [Parameter("Source", Group = "Fast MA")]
        public DataSeries FastMaSource { get; set; }

        [Parameter("Period", DefaultValue = 9, Group = "Fast MA")]
        public int FastMaPeriod { get; set; }

        [Parameter("Source", Group = "Slow MA")]
        public DataSeries SlowMaSource { get; set; }

        [Parameter("Period", DefaultValue = 20, Group = "Slow MA")]
        public int SlowMaPeriod { get; set; }

        [Parameter("Volume (Lots)", DefaultValue = 0.01, Group = "Trade")]
        public double VolumeInLots { get; set; }

        [Parameter("Stop Loss (Pips)", DefaultValue = 10, Group = "Trade")]
        public double StopLossInPips { get; set; }

        [Parameter("Take Profit (Pips)", DefaultValue = 10, Group = "Trade")]
        public double TakeProfitInPips { get; set; }

        [Parameter("Label", DefaultValue = "Sample", Group = "Trade")]
        public string Label { get; set; }

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

            _fastTimeSeriesMovingAverage = Indicators.TimeSeriesMovingAverage(FastMaSource, FastMaPeriod);
            _slowTimeSeriesMovingAverage = Indicators.TimeSeriesMovingAverage(SlowMaSource, SlowMaPeriod);
        }

        protected override void OnBar()
        {
            if (_fastTimeSeriesMovingAverage.Result.HasCrossedAbove(_slowTimeSeriesMovingAverage.Result, 0))
            {
                ClosePositions(TradeType.Sell);

                ExecuteMarketOrder(TradeType.Buy, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
            }
            else if (_fastTimeSeriesMovingAverage.Result.HasCrossedBelow(_slowTimeSeriesMovingAverage.Result, 0))
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