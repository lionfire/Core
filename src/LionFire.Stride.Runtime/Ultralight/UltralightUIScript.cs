//#define ULSharp
// Based on https://github.com/makotech222/Ultralight-Stride3d_Integration
//  - See that repo's Readme for instructions

#define TRACE_Touch
#define TRACE_Keyboard
#if Ultralight
using System;
using System.IO;
using System.Linq;
using LionFire.Dependencies;
using Microsoft.Extensions.Logging;
using Stride.Core;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI.Controls;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using LionFire.Stride3D.Input;
using System.Diagnostics;
using Keys = Stride.Input.Keys;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
#if LiveSharp
using LiveSharp;
using LionFire.LiveSharp;
#endif
using LionFire.Threading;
using LionFire.Dispatching;
using Stride.Audio;

#if ULSharp
using ImpromptuNinjas.UltralightSharp.Safe;
using MouseButton = ImpromptuNinjas.UltralightSharp.Enums.MouseButton;
using ULMouseEventType = ImpromptuNinjas.UltralightSharp.Enums.MouseEventType;
using ImpromptuString = ImpromptuNinjas.UltralightSharp.String;
using Renderer = ImpromptuNinjas.UltralightSharp.Safe.Renderer;
using ULView = ImpromptuNinjas.UltralightSharp.Safe.View;
using ULSession = ImpromptuNinjas.UltralightSharp.Safe.Session;
#error TODO:
using ULMouseEvent = ;
using ULKeyEvent = ;
using ULScrollEvent = ;
using ULKeyEventType = ImpromptuNinjas.UltralightSharp.Enums.KeyEventType
#elif ULNet
using Renderer = UltralightNet.Renderer;
using UltralightNet.AppCore;
using UltralightNet;
using ULView = UltralightNet.View;
using ULSession = UltralightNet.Session;
//using ULMouseEvent = UltralightNet.ULMouseEvent;
//using ULScrollEvent = UltralightNet.ULScrollEvent;
using System.Drawing;
using Silk.NET.OpenXR;
using Stride.UI;

#endif

namespace LionFire.Stride3D.UI;

// TODO: Analyze for threadsafety.
// Docs:
// The Ultralight API is not thread-safe at this time-- calling the API from multiple threads is not supported and will lead 
// to subtle issues / application instability. The library does not need to run on the main thread though-- you can create 
// the Renderer on another thread and make all calls to the API on that thread.

public class UltralightUIScriptDispatcher
#if LiveSharp
    : INotificationHandler<UpdatedMethodNotification>
    , INotificationHandler<UpdatedResourceNotification>
#endif
{
#if LiveSharp
    Task INotificationHandler<UpdatedMethodNotification>.Handle(UpdatedMethodNotification notification, CancellationToken cancellationToken)
        => UltralightUIScript.Instance?.OnUpdatedMethodNotification(notification);

    Task INotificationHandler<UpdatedResourceNotification>.Handle(UpdatedResourceNotification notification, CancellationToken cancellationToken)
        => UltralightUIScript.Instance?.OnUpdatedResourceNotification(notification);
#endif

}

/// <remarks>
/// Sets up Ultralight to draws to Grid > Image "img"
/// Override Start() and Update() and call the base methods
/// </remarks>
public class UltralightUIScript : SyncScript
{
    #region (Static)

    internal static UltralightUIScript Instance { get; set; } // TODO - avoid the static

    /// <summary>
    /// Should be only one renderer per Game.
    /// </summary>
    protected static Renderer renderer;

    #endregion

    #region Parameters

    [DataMemberIgnore]
    public Keys? ToggleKey { get; set; } = Keys.F10;
    public Keys? InspectorServerKey { get; set; } = Keys.F12;
    public Keys? ExternalBrowserKey { get; set; } = Keys.F9;
    public Keys? RefreshBrowserKey { get; set; } = Keys.F5;

    public bool ShouldPassToBrowser(Keys key)
        => key switch
        {
            Keys.F5 => false,
            Keys.F9 => false,
            Keys.F10 => false,
            _ => true
        };

    /// <summary>
    /// Full path to directory containing html files.
    /// </summary>
    [DataMemberIgnore]
    public string AssetDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "wwwroot");

    /// <summary>
    /// File path to main html file. Should be inside the AssetDirectory folder.
    /// </summary>
    [DataMemberIgnore]
    //public string LoadingUrl { get; set; } = "http://google.com"; // file:///index.html
    public string LoadingUrl { get; set; } = "file:///loading.html";

    [DataMemberIgnore]
    //public string StartUrl { get; set; } = "http://google.com";
    public string StartUrl { get; set; } = "http://localhost:7150/";

    #endregion

    #region Dependencies

    private ILogger Logger { get; set; }

    private IHostApplicationLifetime HostApplicationLifetime { get; }

    private IDispatcher Dispatcher => StrideDispatcher.Instance;
    //private IDispatcher Dispatcher => dispatcher ??= DependencyContext.Current?.GetService<IDispatcher>();
    //private IDispatcher dispatcher ;

    #endregion

    #region Relationships

    /// <summary>
    /// View created by Ultralight.
    /// </summary>
    protected ULView View { get; set; }
    protected ULSession session;
    protected Texture texture;
    protected SpriteFromTexture sprite;

    #region ImageElement

    private UIComponent UIComponent => Entity.Get<UIComponent>();

    private ImageElement ImageElement
    {
        get => imageElement;
        set
        {
            imageElement = value;
            UpdateImageVisibility();
        }
    }
    private ImageElement imageElement;

    private void UpdateImageVisibility()
    {
        if (imageElement != null)
        {
            imageElement.Visibility = visible ? Stride.UI.Visibility.Visible : Stride.UI.Visibility.Hidden;
            imageElement.CanBeHitByUser = visible;
        }
        if (UIComponent?.Page?.RootElement != null)
        {
            if (UIComponent.Page.RootElement is Stride.UI.Panels.Grid grid)
            {
                grid.CanBeHitByUser = visible;
            }
        }
    }

    #endregion

    #endregion

    //Sound tReloadSound;
    //SoundInstance reloadSound;

    #region Construction and Destruction

    public UltralightUIScript()
    {
        Instance ??= this;
        Logger = DependencyContext.Current?.GetService<ILogger<UltralightUIScript>>() ?? (ILogger)Logging.Null.NullLogger.Instance;
        HostApplicationLifetime = DependencyContext.Current?.GetService<IHostApplicationLifetime>();
    }

    //void InitSound()
    //{
    //    try
    //    {
    //        Sound tReloadSound = Content.Load<Sound>("Audio/Tiny Button Push-SoundBible.com-513260752 [notification]");
    //        reloadSound = tReloadSound.CreateInstance();
    //        reloadSound.Volume = 0.25f;
    //        reloadSound.Play();
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogError(ex, "Failed to init sound");
    //    }
    //}


#if ULNet && false
    void Headless_TOTRIAGE()
    {
        AppCoreMethods.SetPlatformFontLoader();
        var cfg = new ULConfig();
        using Renderer renderer = ULPlatform.CreateRenderer(cfg);
        // --------------

        // Create View
        using View view = renderer.CreateView(1024, 768);

        // Load URL

        bool loaded = false;

        view.OnFinishLoading += (_, _, _) =>
        {
            loaded = true;
        };

        view.URL = "https://ultralig.ht";

        // Update Renderer until page is loaded
        while (!loaded)
        {
            renderer.Update();
            // give time to process network etc.
            Thread.Sleep(10);
        }

        // Render
        renderer.Render();

        // Get Surface
        ULSurface surface = view.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

        // Get Bitmap
        ULBitmap bitmap = surface.Bitmap;

        // Save bitmap to png file
        var path = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
        bitmap.WritePng(Path.Combine(path, "OUTPUT12345.png"));
    }
#endif

    protected void InitUltralight()
    {
#if ULSharp
        Ultralight.SetLogger(new Logger { LogMessage = LoggerCallback });
        using var cfg = new Config();

        var cachePath = Path.Combine(AssetDirectory, "Cache");
        cfg.SetCachePath(cachePath);

        var resourcePath = Path.Combine(AssetDirectory, "resources");
        cfg.SetResourcePath(resourcePath);

        cfg.SetUseGpuRenderer(false);
        cfg.SetEnableImages(true);
        cfg.SetEnableJavaScript(true);

        AppCore.EnablePlatformFontLoader();
        AppCore.EnablePlatformFileSystem(AssetDirectory);

        renderer = new Renderer(cfg);
#elif ULNet
        //UltralightNet.Logger.LogMessage = LoggerCallback;    // TODO
        AppCoreMethods.ulEnableDefaultLogger("z:/log/ultralight.log"); // TEMP HARDCODE HARDPATH
        AppCoreMethods.SetPlatformFontLoader();
        AppCoreMethods.ulEnablePlatformFileSystem(AssetDirectory);

        var cfg = new ULConfig();

        cfg.CachePath = Path.Combine(AssetDirectory, "Cache") + Path.DirectorySeparatorChar;
        cfg.ResourcePathPrefix = Path.Combine(AssetDirectory, "resources") + Path.DirectorySeparatorChar;

        //cfg.UseGpuRenderer = false;
        //cfg.SetEnableImages(true);
        //cfg.SetEnableJavaScript(true);

        renderer = ULPlatform.CreateRenderer(cfg);
#endif

    }

    ~UltralightUIScript()
    {
        View?.Dispose();
        renderer?.Dispose();
    }

    #endregion

    #region State

    #region Settings

    public bool AutoReload { get; set; } = true;

    #endregion

    protected uint width;
    protected uint height;

    private bool loadedLoadingUrl = false;
    private bool IsWebServerAvailable => HostApplicationLifetime.ApplicationStarted.IsCancellationRequested;
    private bool startedLoadingStartUrl = false;
    private bool JavascriptTestComplete = false;

    #region Visible

    [DataMemberIgnore]
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            UpdateImageVisibility();
        }
    }
    private bool visible = true;

    #endregion

    ComponentUI pendingInputEvents = new ComponentUI();

    #endregion

    #region Event Handlers

    #region MediatR

    public bool IsWebUI(Type type) => type.FullName.Contains("Blazor") || type.BaseType == typeof(Microsoft.AspNetCore.Components.ComponentBase);

#if LiveSharp
    internal Task OnUpdatedMethodNotification(UpdatedMethodNotification notification)
    {
        //Logger.LogInformation($"OnUpdatedMethodNotification -- " + notification.UpdatedMethod.DeclaringType.BaseType.BaseType.FullName);
        if (AutoReload)
        {
            if (IsWebUI(notification.UpdatedMethod.DeclaringType))
            {
                Logger.LogInformation($"AutoReloading due to method change: {notification.UpdatedMethod.DeclaringType.FullName}.{notification.UpdatedMethod.MethodIdentifier}");
                return Dispatcher.BeginInvoke(() =>
                {
                    //try
                    //{
                    //    reloadSound.Play();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Logger.LogError(ex, "Error playing sound");
                    //}
                    View.Reload();
                });
            }
            else
            {
                Logger.LogDebug($"Ignorning non-web UI code change in {notification.UpdatedMethod.DeclaringType}");
            }
        }
        return Task.CompletedTask;
    }

    internal Task OnUpdatedResourceNotification(UpdatedResourceNotification notification)
    {
        var path = notification?.Path;
        if (path != null &&
            (path.EndsWith(".css"))
            )
        {
            Dispatcher.BeginInvoke(() =>
            {
                View?.Reload();
            });
        }
        return Task.CompletedTask;

    }
#endif

    #endregion

    private void OnPrivateWebServerStarted()
    {
        if (startedLoadingStartUrl) return;

        startedLoadingStartUrl = true;
        Logger.LogInformation($"Loading: {StartUrl}");
        SetUrl(StartUrl);
        renderer.Render();
    }

    private void SetUrl(string url)
    {
#if ULSharp
        View.LoadUrl(LoadingUrl); // TEMP commented
#else
        View.URL = url;
#endif
        loaded = false;

    }
    private bool loaded = false;
    public async Task WaitForLoaded()
    {
        while (!loaded)
        {
            renderer.Update();
            await Task.Delay(10);
        }
        renderer.Render();
        TakeScreenshot();
    }

    #region Diag

    private void TakeScreenshot()
    {
        ULSurface surface = View.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

        ULBitmap bitmap = surface.Bitmap;
        var path = Path.GetDirectoryName(@"z:\log\"); // TEMP
        bitmap.WritePng(Path.Combine(path, $"Snapshot-{SnapshotId++}.png"));
    }
    static int SnapshotId = 0;

    #endregion

    #region Event Handlers: Stride UI

    #region Mouse / Touch

    private void ImageElement_MouseOverStateChanged(object sender, Stride.UI.PropertyChangedArgs<Stride.UI.MouseOverState> e)
    {
        Logger.LogDebug($"MouseOverStateChanged {e.NewValue}");
    }

    private void ImageElement_TouchEnter(object sender, Stride.UI.TouchEventArgs e)
    {
        Logger.LogDebug("TouchEnter " + e.ScreenPosition);
    }

    private void ImageElement_TouchDown(object sender, Stride.UI.TouchEventArgs e)
    {
#if TRACE_Touch
        Logger.LogTrace($"TouchDown {e.ScreenPosition} ({(int)(e.ScreenPosition.X * width)},{(int)(e.ScreenPosition.Y * height)}) {(Input.Mouse.DownButtons.Any() ? Input.Mouse.DownButtons.Select(m => m.ToString()).Aggregate((x, y) => $"{x},{y}") : "")}");
#endif

        pendingInputEvents.TouchEvents.Enqueue(e);
#if ULSharp
        //new ULMouseEvent(ULMouseEventType.MouseDown, (int)(e.ScreenPosition.X * width), (int)(e.ScreenPosition.Y * height), ULMouseButton.Left));
#endif
        e.Handled = false;
    }

    private void ImageElement_TouchUp(object sender, Stride.UI.TouchEventArgs e)
    {
#if TRACE_Touch
        Logger.LogTrace($"TouchUp {e.ScreenPosition} ({(int)(e.ScreenPosition.X * width)},{(int)(e.ScreenPosition.Y * height)})");
#endif

#if ULSharp
        pendingInputEvents.MouseEvents.Enqueue(new ULMouseEvent(ULMouseEventType.MouseUp, (int)(e.ScreenPosition.X * width), (int)(e.ScreenPosition.Y * height), ULMouseButton.Left));
#else
        pendingInputEvents.TouchEvents.Enqueue(e);
#endif
    }

    #endregion

    #region Keyboard

    private void OnKeysPressed()
    {

        foreach (var key in Input.PressedKeys)
        {
            if (!ShouldPassToBrowser(key)) continue;

            // DEPRECATE the char approach, if virtual key-code works

            string keyString = null;
            switch (key)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    keyString = key.ToString()[1].ToString();
                    break;

                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    keyString = key.ToString().ToLowerInvariant();
                    break;
                default:
                    break;
            }

            if (keyString != null)
            {
                unsafe
                {
#if TRACE_Keyboard
                    Logger.LogTrace($"Key pressed: {key}");
#endif

                    pendingInputEvents.KeyboardEvents.Enqueue(
#if ULNet
                        ULKeyEvent.Create(ULKeyEventType.Char, 0, 0, 0, keyString, keyString, false, false, false)
#else
                        new ULKeyEvent(ULKeyEventType.Char, 0, 0, 0, ImpromptuString.Create(keyString), ImpromptuString.Create(keyString), false, false, false));
#endif
                    );
                }
                continue;
            }

            int nativeCode = key.ToWindowsVirtualKeyCode(); // TODO: Other platforms

            if (nativeCode != 0)
            {
#if ULNet
                Logger.LogWarning($"'{key}' Key pressed.  (code: {nativeCode}) - TODO");
#elif ULSharp
                unsafe
                {
#if TRACE_Keyboard
                    Logger.LogTrace($"'{key}' Key pressed.  (code: {nativeCode})");
#endif
                    // TODO from Ultralight docs:

                    // You'll need to generate a key identifier from the virtual key code
                    // when synthesizing events. This function is provided in KeyEvent.h
                    //GetKeyIdentifierFromVirtualKeyCode(evt.virtual_key_code, evt.key_identifier);

                    //In addition to key presses / key releases, you'll need to pass in the actual text generated. (For example, pressing the A key should generate the character 'a').
                    //KeyEvent evt;
                    //evt.type = KeyEvent::kType_Char;
                    //evt.text = "a";
                    //evt.unmodified_text = "a"; // If not available, set to same as evt.text

                    var empty = ImpromptuString.Create("");
                    pendingInputEvents.KeyboardEvents.Enqueue(new KeyEvent(ImpromptuNinjas.UltralightSharp.Enums.KeyEventType.RawKeyDown, 0, nativeCode, 0, ImpromptuString.Create(""), empty, false, false, false));
                }
#endif
            }
            else
            {
                Logger.LogWarning($"Ignoring key because native key code is unknown: {key}");
            }
        }
    }

    #endregion

    #endregion

    #endregion

    #region Stride Component Overrides

    public override void Start()
    {
        base.Start();

        //InitSound();

        #region UIComponent

        var uiComponent = UIComponent;
        if (uiComponent == null) { Logger.LogError($"{this.GetType().FullName} script must be installed on the same Entity as UIComponent"); return; }

        #endregion

        #region ImageElement

        //var gridElement = uiComponent.Page.RootElement.VisualChildren.FirstOrDefault() as Stride.UI.Panels.Grid;

        imageElement = uiComponent.Page.RootElement.VisualChildren.OfType<ImageElement>().FirstOrDefault() as ImageElement;
        if (imageElement == null) { Logger.LogError($"Failed to find image element.  The first ImageElement in VisualChildren will be used."); return; }

        #region Events

        imageElement.TouchUp += ImageElement_TouchUp;
        imageElement.TouchDown += ImageElement_TouchDown;
        imageElement.TouchEnter += ImageElement_TouchEnter;
        imageElement.MouseOverStateChanged += ImageElement_MouseOverStateChanged;

        #endregion

        Logger.LogDebug($"Drawing to image: {imageElement.Name}");

        #endregion

        width = (uint)uiComponent.Resolution.X;
        height = (uint)uiComponent.Resolution.Y;

        #region Ultralight setup

        texture = Texture.New2D(this.GraphicsDevice, (int)width, (int)height, Stride.Graphics.PixelFormat.B8G8R8A8_UNorm_SRgb, TextureFlags.ShaderResource | TextureFlags.RenderTarget);
        sprite = new SpriteFromTexture();

        if (renderer == null) { InitUltralight(); }

#if ULNet
        session = renderer.DefaultSession;
        var viewConfig = new ULViewConfig()
        {
            EnableImages = true,
            EnableJavaScript = true,
            //FontFamilyFixed =
            //FontFamilySansSerif = 
            //FontFamilySerif = 
            //FontFamilyStandard = 
            //IsAccelerated = true,
            IsTransparent = true,
            //InitialDeviceScale = 1,
            InitialFocus = true,
            //UserAgent = "Valor",
        };
        View = renderer.CreateView(width, height, viewConfig, session);
        // REVIEW - what was the true parameter in ULView?
#else
        session = new ULSession(renderer, false, "");
        View = new ULView(renderer, width, height, true, session);
#endif


#if ULSharp
        View.SetFinishLoadingCallback((data, caller, frameId, isMainFrame, url) =>
        {
            loadedLoadingUrl = true;
            Logger.LogInformation($"Finished loading: {url}");
        }, default);
#elif ULNet
        View.OnFinishLoading += View_OnFinishLoading;
#endif

        #endregion

        #region LoadUrl

        if (!IsWebServerAvailable)
        {
            Logger.LogInformation("Web server not available yet.  Loading LoadingUrl.");
            SetUrl(LoadingUrl);

            //Task.Run(async () =>
            //{
            //    //await HostApplicationLifetime.ApplicationStarted;
            //    while(!IsWebServerAvailable)
            //    {
            //        Logger.LogInformation("Waiting for web server to start... ");
            //        await Task.Delay(250);
            //    }
            //    Logger.LogInformation("Waiting for web server to start...done.");
            //    OnPrivateWebServerStarted();
            //});
        }
        else
        {
            Logger.LogInformation("Web server already available.  (Skipping LoadingUrl.)");
            loadedLoadingUrl = true;
            OnPrivateWebServerStarted();
        }

        #endregion
    }

#if ULNet
    private void View_OnFinishLoading(ulong frameId, bool isMainFrame, string url)
    {
        loadedLoadingUrl = true;
        Logger.LogInformation("Finished loading: {url}.  (IsMainFrame: {IsMainFrame})", url, isMainFrame);
    }
#endif

    bool IsInspectorServerStarted = false;

    public override void Update()
    {
        if (renderer == null) return;

        if (!loadedLoadingUrl)
        {
            renderer.Update();
            renderer.Render();
            if (!loadedLoadingUrl) { return; }
        }
        if (!startedLoadingStartUrl && IsWebServerAvailable) { OnPrivateWebServerStarted(); }


        if (!JavascriptTestComplete) { DoJavascriptTest(); }

        FireInputs(pendingInputEvents);

        if (RefreshBrowserKey.HasValue && Input.PressedKeys.Contains(RefreshBrowserKey.Value))
        {
            Logger.LogInformation("Refreshing");
            View.Reload();
       }
         if (InspectorServerKey.HasValue && Input.PressedKeys.Contains(InspectorServerKey.Value)) {
            if(!IsInspectorServerStarted)
            {
                Logger.LogInformation("Ultralight Inspector Server starting...");
                IsInspectorServerStarted = renderer.TryStartRemoteInspectorServer("127.0.0.1", 7152); // TODO Make Port HARDCODE configurable
                Logger.LogInformation("Ultralight Inspector Server started.");
            }
            else
            {
                Logger.LogInformation("Ultralight Inspector Server already started.");
            }
        }
        if (ToggleKey.HasValue && Input.PressedKeys.Contains(ToggleKey.Value)) { Visible ^= true; }
        if (ExternalBrowserKey.HasValue && Input.PressedKeys.Contains(ExternalBrowserKey.Value))
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = StartUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259) { Logger.LogError(noBrowser.Message); }
            }
            catch (System.Exception other)
            {
                Logger.LogError(other.Message);
            }
        }

        if (Input.HasPressedKeys) { OnKeysPressed(); }

        renderer.Update();
        renderer.Render();

        var surface = View.Surface;
        if (!surface.HasValue)
        {
            Logger.LogError("Ultralight: missing surface");
            return;
        }
        var bitmap = surface.Value.Bitmap;
        unsafe
        {
            var pixels = bitmap.LockPixels();

            DataPointer dataPointer = new DataPointer((IntPtr)pixels, (int)bitmap.Height * (int)bitmap.Width * (int)bitmap.Bpp);
            texture.SetData(this.Game.GraphicsContext.CommandList, dataPointer);
            sprite.Texture = texture;
            imageElement.Source = sprite;

            bitmap.UnlockPixels();
        }
        bitmap.Dispose();

        void FireInputs(ComponentUI ui)
        {
            while (pendingInputEvents.MouseEvents.TryDequeue(out var e)) { View.FireMouseEvent(e); }
            while (pendingInputEvents.ScrollEvents.TryDequeue(out var e)) { View.FireScrollEvent(e); }
            while (pendingInputEvents.KeyboardEvents.TryDequeue(out var e)) { View.FireKeyEvent(e); }
        }
        void DoJavascriptTest()
        {
            JavascriptTestComplete = true;
            try
            {
                var result = View.EvaluateScript($"console.log('hi ' + (2+2)); 2+2;", out string exception);
                Logger.LogInformation("Javascript returned: " + result);
                if (!string.IsNullOrEmpty(exception))
                {
                    Logger.LogError("Javascript returned exception: " + exception);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Javascript threw exception");
            }
        }
    }

    #endregion

    #region Classes

    public class ComponentUI
    {
        public ConcurrentQueue<TouchEventArgs> TouchEvents { get; set; } = new ConcurrentQueue<TouchEventArgs>();
        public ConcurrentQueue<ULMouseEvent> MouseEvents { get; set; } = new ConcurrentQueue<ULMouseEvent>();
        public ConcurrentQueue<ULKeyEvent> KeyboardEvents { get; set; } = new ConcurrentQueue<ULKeyEvent>();
        public ConcurrentQueue<ULScrollEvent> ScrollEvents { get; set; } = new ConcurrentQueue<ULScrollEvent>();
    }

    #endregion

    #region Logging

#if ULSharp
    private LoggerLogMessageCallback LoggerCallback
        => new LoggerLogMessageCallback((logLevel, msg) =>
        {
            var microsoftLogLevel = logLevel switch
            {
                ImpromptuNinjas.UltralightSharp.Enums.LogLevel.Error => LogLevel.Error,
                ImpromptuNinjas.UltralightSharp.Enums.LogLevel.Warning => LogLevel.Warning,
                ImpromptuNinjas.UltralightSharp.Enums.LogLevel.Info => LogLevel.Information,
                _ => LogLevel.Error,
            };
            Logger.Log(microsoftLogLevel, msg);
        });
#endif
    #endregion
}

#endif