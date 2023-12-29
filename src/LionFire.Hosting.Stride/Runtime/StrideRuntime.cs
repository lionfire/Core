using Microsoft.Extensions.Logging;
using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Games;
using Stride.Physics;

namespace LionFire.Hosting;

public abstract class StrideRuntime
{
    public ILogger Logger { get; }
    public ServiceRegistry GlobalStrideServices { get; }
    public ContentManager GlobalContent { get; }

    //ObjectDatabase ObjectDatabase { get; }
    //DatabaseFileProvider DatabaseFileProvider { get; }
    //DatabaseFileProviderService DatabaseFileProviderService { get; }
    //IDatabaseFileProviderService IDatabaseFileProviderService { get; }

    #region Lifecycle

    public StrideRuntime(ILogger logger, ServiceRegistry globalStrideServices, ContentManager contentManager)
    {
        Logger = logger;
        GlobalStrideServices = globalStrideServices;
        StrideServices = new(globalStrideServices);

        GlobalContent = contentManager;

        // Game systems
        gameSystems = new GameSystemCollection(StrideServices);
        StrideServices.AddService<IGameSystemCollection>(gameSystems);
        gameSystems.Initialize();

        loadSettings = CreateContentManagerLoaderSettings;

        Init();
    }

    protected virtual Task Init()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Configuration

    protected ContentManagerLoaderSettings loadSettings;

    #endregion

    protected virtual ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings();

    #region State

    GameSystemCollection gameSystems;
    public InheritingServiceRegistry StrideServices { get; }
    public PhysicsProcessor Physics { get; protected set; }

    #endregion

}
