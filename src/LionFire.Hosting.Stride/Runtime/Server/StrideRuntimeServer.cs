using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine.Design;
using Stride.Engine;
using Stride.Physics;

namespace LionFire.Hosting;

public class StrideRuntimeServer : StrideRuntime
{
    #region Parameters

    public string SceneName { get; set; } = "MainScene";

    #endregion

    public StrideRuntimeServer(ILogger<StrideRuntimeServer> logger, ServiceRegistry globalStrideServices, ContentManager contentManager) : base(logger, globalStrideServices, contentManager: contentManager)
    {
    }

    protected override ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings
    {
        // Ignore all references (Model, etc...)
        ContentFilter = ContentManagerLoaderSettings.NewContentFilterByType()
    };

    protected override async Task Init()
    {
        var scene = await GlobalContent.LoadAsync<Scene>(SceneName, loadSettings);

        var sceneInstance = new SceneInstance(StrideServices, scene, ExecutionMode.None);
        var sceneSystem = new SceneSystem(StrideServices)
        {
            SceneInstance = sceneInstance
        };
        StrideServices.AddService(sceneSystem);

        Physics = new PhysicsProcessor();
        sceneInstance.Processors.Add(Physics);
    }

    //void Example()
    //{
    //    //var result = Physics.Simulation.Raycast(start, end);
    //    //Console.WriteLine($"Performing raycast: {(result.Succeeded ? "hit" : "miss")}");

    //}
}
