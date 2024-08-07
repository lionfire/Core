﻿using LionFire.Inspection;
using LionFire.Inspection.Nodes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class InspectionHostingX
{
    public static IServiceCollection AddInspector(this IServiceCollection services, bool useDefaults = true)
    {
        services
            .AddSingleton<IInspectorService, InspectorService>()
            ;

        if (useDefaults)
        {
            services.AddDefaultInspectors();
            services.AddOrleansInspectors(); // MOVE to separate DLL?
        }
        return services;
    }
    public static IServiceCollection AddDefaultInspectors(this IServiceCollection services)
    {
        return services
            .TryAddEnumerableSingleton<IInspector, ReflectionInspector>()
            ;
    }
}
