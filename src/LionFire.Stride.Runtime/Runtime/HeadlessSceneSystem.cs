
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Games;

namespace LionFire.Stride_.Runtime;

public class HeadlessSceneSystem : GameSystemBase
{
    /// <summary>
    /// URL of the scene loaded at initialization.
    /// </summary>
    public string? InitialSceneUrl { get; set; }

    bool LoadAsync = true;
    public HeadlessSceneSystem([NotNull] IServiceRegistry registry) : base(registry)
    {
    }

    /// <summary>
    /// Gets or sets the root scene.
    /// </summary>
    /// <value>The scene</value>
    /// <exception cref="System.ArgumentNullException">Scene cannot be null</exception>
    public SceneInstance? SceneInstance { get; set; }
    private Task<Scene>? sceneTask;

    protected override void LoadContent()
    {
        var content = Services.GetSafeServiceAs<ContentManager>();
        //var graphicsContext = Services.GetSafeServiceAs<GraphicsContext>();

        // Preload the scene if it exists and show splash screen
        if (InitialSceneUrl != null && content.Exists(InitialSceneUrl))
        {
            if (LoadAsync)
                sceneTask = content.LoadAsync<Scene>(InitialSceneUrl);
            else
                SceneInstance = new SceneInstance(Services, content.Load<Scene>(InitialSceneUrl));
        }
        else
        {
            SceneInstance ??= new SceneInstance(Services) { RootScene = new Scene() };
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Execute Update step of SceneInstance
        // This will run entity processors
        SceneInstance?.Update(gameTime);
    }
}

