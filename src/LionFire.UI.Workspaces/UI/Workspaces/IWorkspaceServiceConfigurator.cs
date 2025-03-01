﻿using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Workspaces;
public interface IWorkspaceServiceConfigurator
{
    ValueTask ConfigureWorkspaceServices(IServiceCollection services, UserWorkspacesService userWorkspacesService, string workspaceId);
}
