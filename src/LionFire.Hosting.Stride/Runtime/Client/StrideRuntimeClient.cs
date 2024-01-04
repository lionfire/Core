using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine.Design;
using Stride.Engine;
using Stride.Physics;
using Microsoft.Extensions.Logging;

namespace  LionFire.Stride_.Runtime;

public class StrideRuntimeClient : StrideRuntime
{
    public StrideRuntimeClient(IServiceProvider serviceProvider, ILogger<StrideRuntimeClient> logger) : base(serviceProvider, logger)
    {
        
    }

    //protected override Task LoadMainScene(string? mainSceneName = null)
    //{
    //    throw new NotImplementedException();

    //    //void TRIAGE()
    //    //{
    //    //    // Load scene (physics only)
    //    //    var loadSettings = new ContentManagerLoaderSettings
    //    //    {
    //    //        // Ignore all references (Model, etc...)
    //    //        ContentFilter = ContentManagerLoaderSettings.NewContentFilterByType()
    //    //    };
    //    //    var scene = await Content.LoadAsync<Scene>("MainScene", loadSettings);
    //    //    var sceneInstance = new SceneInstance(Services, scene, ExecutionMode.None);
    //    //    var sceneSystem = new SceneSystem(Services)
    //    //    {
    //    //        SceneInstance = sceneInstance
    //    //    };
    //    //    GlobalStrideServices.AddService(sceneSystem);

    //    //    var physics = new PhysicsProcessor();
    //    //    sceneInstance.Processors.Add(physics);

    //    //    //var result = physics.Simulation.Raycast(start, end);
    //    //    //Console.WriteLine($"Performing raycast: {(result.Succeeded ? "hit" : "miss")}");

    //    //}

    //}
}
