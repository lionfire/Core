using LionFire.Applications.Hosting;
using LionFire.Trading.Triggers;
using System;
using System.Collections.Generic;

namespace LionFire.Notifications.Service
{

    class Program
    {
        static void Main(string[] args)
        {
            var triggers = new List<object>();

            triggers.Add(new TPriceTrigger("GBPUSD", ComparisonOperator.GreaterThan, 1.2900m));

            triggers.Add(new TPriceChangeAmountTrigger
            {
                Symbol = "XAUUSD",
                ChangeAmount = "3.00",
                TimeSpan = TimeSpan.FromHours(1),
            });

            new AppHost()
                .Add(new AppInfo("LionFire", "Notification Service", "Notifications"))
                .Add<TServiceSupervisor>()
                .AddJsonAssetProvider()
                .Run().Wait();

        }
    }
}