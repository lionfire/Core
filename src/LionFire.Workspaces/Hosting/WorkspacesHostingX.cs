using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Workspaces;

namespace LionFire.Hosting;

public static class WorkspacesHostingX
{
    public static IServiceCollection AddWorkspacesModel(this IServiceCollection services)
        => services
        //.AddTransient<Workspace>()
        ;

}

