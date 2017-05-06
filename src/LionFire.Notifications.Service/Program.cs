using LionFire.Applications.Hosting;
using LionFire.Trading;
using LionFire.Trading.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using LionFire.Reactive;
using LionFire.Trading.Accounts;
using LionFire.Trading.Statistics;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LionFire.Notifications.Service
{
    public abstract class MarketFeed
    {
        public IEnumerable<string> SymbolsAvailable { get; protected set; }

        public ObservableCollection<string> SymbolsOfInterest { get; set; }

        public IObservable<MarketQuote> QuoteStream { get { return quoteStream; } }
        private Subject<MarketQuote> quoteStream = new Subject<MarketQuote>();

        protected void OnQuote(MarketQuote q)
        {
            quoteStream.OnNext(q);
        }

    }

    public class MarketQuote
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
    }

    //public class SymbolNameResolver
    //{
    //    public Dictionary<string, string> Aliases { get; set; }
    //}


    public class TTrueFxTradingAccount : TMarketAccount
    {
    }
    public class TrueFxTradingAccount : LiveAccountBase<TTrueFxTradingAccount>
    {
        public override TradeResult ClosePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public override MarketSeries CreateMarketSeries(string symbol, TimeFrame timeFrame)
        {
            throw new NotImplementedException();
        }

        public override TradeResult ExecuteMarketOrder(TradeType tradeType, Symbol symbol, long volume, string label = null, double? stopLossPips = default(double?), double? takeProfitPips = default(double?), double? marketRangePips = default(double?), string comment = null)
        {
            throw new NotImplementedException();
        }

        public override TradeResult ModifyPosition(Position position, double? stopLoss, double? takeProfit)
        {
            throw new NotImplementedException();
        }

        protected override Symbol CreateSymbol(string symbolCode)
        {
            throw new NotImplementedException();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {

            



            var triggers = new List<object>();

            triggers.Add(new TPriceTrigger("GBPUSD", ComparisonOperator.GreaterThan, 1.2900m));

            triggers.Add(new TPriceChangeAmountTrigger
            {
                Symbol = "XAUUSD",
                ChangeAmount = 3.00m,
                TimeSpan = TimeSpan.FromHours(1),
            });

            new AppHost()
                .Add(new AppInfo("LionFire", "Notification Service", "Notifications"))
                .AddJsonAssetProvider()
                .Initialize()
                .Add<TServiceSupervisor>()
                .Add<>()
                .Run().Wait();

        }
    }
}