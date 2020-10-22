using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Caliburn.Micro;

namespace LionFire.Hosting
{
    public static class EventAggregatorHostingExtensions
    {
        public static IServiceCollection AddEventAggregator(this IServiceCollection services)
            => services.AddSingleton<IEventAggregator, EventAggregator>();

    }
}
