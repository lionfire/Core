using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Games;
using static Stride.Engine.SceneSystem;
using Stride.Graphics;
using Stride.Rendering.Compositing;
using Stride.Rendering;

namespace LionFire.Stride_.Runtime;

#if true // DerivedFromSceneSystem
public class HeadlessSceneSystem : SceneSystem
{
    public HeadlessSceneSystem(IServiceRegistry registry) : base(registry)
    {
    }

    public override bool BeginDraw()
    {
        return false;
    }
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    protected override void LoadContent()
    {
        var content = Services.GetSafeServiceAs<ContentManager>();

        if (InitialSceneUrl != null && content.Exists(InitialSceneUrl))
        {
            SceneInstance = new SceneInstance(Services, content.Load<Scene>(InitialSceneUrl));
        }
        else
        {
            SceneInstance ??= new SceneInstance(Services) { RootScene = new Scene() };
        }
    }
}

#else // Clean implementation of HeadlessSceneSystem without graphics code (can't work due to physics code requiring the SceneSystem class

// Based on SceneSystem from Stride (https://github.com/stride3d/stride), MIT License
/// <summary>
/// Headless version of SceneSystem
/// - calls Update on SceneInstance, which runs entity processors
/// </summary>
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


#endif