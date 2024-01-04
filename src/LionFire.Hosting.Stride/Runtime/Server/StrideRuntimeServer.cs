using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine.Design;
using Stride.Engine;
using Stride.Physics;

namespace LionFire.Stride_.Runtime;

public class StrideRuntimeServer : StrideRuntime
{
    public StrideRuntimeServer(IServiceProvider serviceProvider, ILogger<StrideRuntimeServer> logger) : base(serviceProvider, logger)
    {
    }

    protected override ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings
    {
        // Ignore all references (Model, etc...)
        ContentFilter = ContentManagerLoaderSettings.NewContentFilterByType()
    };

    //protected override async Task LoadMainScene()
    //{
        

    //    Physics = new PhysicsProcessor();
    //    SceneInstance.Processors.Add(Physics);
    //}

    //void Example()
    //{
    //    //var result = Physics.Simulation.Raycast(start, end);
    //    //Console.WriteLine($"Performing raycast: {(result.Succeeded ? "hit" : "miss")}");

    //}
}
