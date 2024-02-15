// Inspired by and derived from Stride's Stride.Games.dll
//
// Stride license:
//
// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine.Design;
using Stride.Engine;
using Stride.Physics;
using Microsoft.Extensions.Logging;
using Stride.Games;
using Stride.Graphics;

namespace LionFire.Stride_.Runtime;

public class StrideRuntimeClient : StrideRuntime
{
    protected override ClientOrServer ClientOrServer => ClientOrServer.Client;

    public StrideRuntimeClient(IServiceProvider serviceProvider, ILogger<StrideRuntimeClient> logger) : base(serviceProvider, logger)
    {
#if TODO
        Streaming = new StreamingManager(StrideServices);

        Audio = new AudioSystem(StrideServices);
        StrideServices.AddService(Audio);
        StrideServices.AddService<IAudioEngineProvider>(Audio);

        gameFontSystem = new GameFontSystem(StrideServices);
        StrideServices.AddService(gameFontSystem.FontSystem);
        StrideServices.AddService<IFontFactory>(gameFontSystem.FontSystem);

        SpriteAnimation = new SpriteAnimationSystem(StrideServices);
        StrideServices.AddService(SpriteAnimation);

        DebugTextSystem = new DebugTextSystem(StrideServices);
        StrideServices.AddService(DebugTextSystem);

        ProfilingSystem = new GameProfilingSystem(StrideServices);
        StrideServices.AddService(ProfilingSystem);

        VRDeviceSystem = new VRDeviceSystem(StrideServices);
        StrideServices.AddService(VRDeviceSystem);

        // Creates the graphics device manager
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        StrideServices.AddService<IGraphicsDeviceManager>(GraphicsDeviceManager);
        StrideServices.AddService<IGraphicsDeviceService>(GraphicsDeviceManager);

        AutoLoadDefaultSettings = true;
#endif
    }
    protected override IGamePlatformEx CreateGamePlatform() =>
        throw new NotImplementedException();
    //Stride.Games.GamePlatform.Create(game);

    protected override void AddEarlyGameSystems()
    {
        base.AddEarlyGameSystems();

#if TODO
        // Add input system early so that it can obtained by the UI system
        var inputSystem = new InputSystem(Services);
        Input = inputSystem.Manager;
        Services.AddService(Input);
        GameSystems.Add(inputSystem);
#endif

    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Adapted from Stride's GameBase.cs</remarks>
    protected override void InitGraphics()
    {
        // TODO temporary workaround as the engine doesn't support yet resize
        var graphicsDeviceManagerImpl = (GraphicsDeviceManager)GraphicsDeviceManager;

        Context.RequestedWidth = graphicsDeviceManagerImpl.PreferredBackBufferWidth;
        Context.RequestedHeight = graphicsDeviceManagerImpl.PreferredBackBufferHeight;
        Context.RequestedBackBufferFormat = graphicsDeviceManagerImpl.PreferredBackBufferFormat;
        Context.RequestedDepthStencilFormat = graphicsDeviceManagerImpl.PreferredDepthStencilFormat;
        Context.RequestedGraphicsProfile = graphicsDeviceManagerImpl.PreferredGraphicsProfile;
        Context.DeviceCreationFlags = graphicsDeviceManagerImpl.DeviceCreationFlags;
    }

    protected override void InitializeGraphicsBeforeRun()
    {
#if TODO
                // Make sure that the device is already created
                graphicsDeviceManager.CreateDevice();

                // Gets the graphics device service
                graphicsDeviceService = Services.GetService<IGraphicsDeviceService>();
                if (graphicsDeviceService == null)
                {
                    throw new InvalidOperationException("No GraphicsDeviceService found");
                }

                // Checks the graphics device
                if (graphicsDeviceService.GraphicsDevice == null)
                {
                    throw new InvalidOperationException("No GraphicsDevice found");
                }

                // Setup the graphics device if it was not already setup.
                SetupGraphicsDeviceEvents();

                // Bind Graphics Context enabling initialize to use GL API eg. SetData to texture ...etc
                BeginDraw();
#endif
    }

    /// <summary>Ends the drawing of a frame. This method will always be preceeded by calls to <see cref="BeginDraw"/> and perhaps <see cref="Draw"/> depending on if we had to do so. </summary>
    protected override void EndDraw(bool present)
    {

        throw new NotImplementedException("TODO");
        //if (beginDrawOk)
        //{
        //    if (GraphicsDevice.Presenter != null)
        //    {
        //        // Perform end of frame presenter operations
        //        GraphicsDevice.Presenter.EndDraw(GraphicsContext.CommandList, present);

        //        GraphicsContext.CommandList.ResourceBarrierTransition(GraphicsDevice.Presenter.BackBuffer, GraphicsResourceState.Present);
        //    }

        //    GraphicsContext.ResourceGroupAllocator.Flush();

        //    // Close command list
        //    GraphicsContext.CommandList.Flush();

        //    // Present (if necessary)
        //    graphicsDeviceManager.EndDraw(present);

        //    beginDrawOk = false;
        //}
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
