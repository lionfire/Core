using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using LionFire.Reactive;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Execution;
using LionFire.DependencyInjection;

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


    /*public class TTrueFxTradingAccount : TMarketAccount
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
    */

    class Program
    {
        static void Main(string[] args)
        {

            var tNotifications = new List<Notifier>
            {
            };


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
                //.AddAsset<TTrueFxFeed>("jaredthirsk")
                .Add(new TTrueFxFeed
                    {
                    }
                    .Register<IFeed>()
                )
                .Add<TServiceSupervisor>()
                
                .Run().Wait();

        }
    }

#if ThoughtExperiment

    public interface IHasResolutionObject
    {
        object ResolutionObject { get; set; }
    }

    public class IocRegistration : IConfigures<IServiceCollection>, IHasResolutionObject
    {
        object IHasResolutionObject.ResolutionObject { get { return Object; } set { Object = value; } }

        //public object ResolutionObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        object Object;
        Type Type;
        public IocRegistration(object obj, Type t)
        {
            this.Object = obj;
            this.Type = t;
        }

        public void Configure(IServiceCollection context)
        {
            // FUTURE: More addition types: transient, or bag of available services
            context.AddSingleton(Type, Object);
        }
    }

    public interface IServiceSelector<T>
    {
        InjectionContext InjectionContext { get; set; }
        T SelectedService { get; }
        event Action<T, T> SelectedServiceChangedFromTo;
    }

    public static class RegistrationExtensions
    {
        public static IocRegistration Register<T>(this object obj)
        {
            var r = new IocRegistration(obj, typeof(T));
            return r;
        }
    }
#endif
}