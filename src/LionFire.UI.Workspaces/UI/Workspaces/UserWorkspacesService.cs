using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Workspaces;


// REVIEW: Document the lifecycle of this service
public class UserWorkspacesService
{
    #region Dependencies

    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Lifecycle

    public UserWorkspacesService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

    }

    #endregion

    public IServiceProvider? UserServices { get; set; }

    #region Properties

    public IReference? UserWorkspaces { get; set; }

    #region REVIEW - Avoids Vos / Reference resolution, allows direct use of filesystem

    public string? UserWorkspacesDir
    {
        get => (UserWorkspaces as FileReference)?.Path;
        set => UserWorkspaces = new FileReference(value);
    }

    #endregion

    #endregion
}
