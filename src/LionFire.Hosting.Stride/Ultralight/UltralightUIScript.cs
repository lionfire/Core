// Based on https://github.com/makotech222/Ultralight-Stride3d_Integration
//  - See that repo's Readme for instructions

//#define TRACE_TouchEvents
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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using LionFire.Threading;

using ImpromptuNinjas.UltralightSharp.Safe;
using MouseButton = ImpromptuNinjas.UltralightSharp.Enums.MouseButton;
using MouseEventType = ImpromptuNinjas.UltralightSharp.Enums.MouseEventType;
using System.Runtime.InteropServices;
using ImpromptuString = ImpromptuNinjas.UltralightSharp.String;
using LionFire.Stride3D.Input;

namespace LionFire.Stride3D.UI
{
    /// <remarks>
    /// Sets up Ultralight to draws to Grid > Image "img"
    /// Override Start() and Update() and call the base methods
    /// </remarks>
    public class UltralightUIScript : SyncScript
    {
        #region (Static)

        /// <summary>
        /// Should be only one renderer per Game.
        /// </summary>
        protected static ImpromptuNinjas.UltralightSharp.Safe.Renderer renderer;

        #endregion

        #region Parameters

        [DataMemberIgnore]
        public Stride.Input.Keys? ToggleKey { get; set; } = Stride.Input.Keys.F10;

        /// <summary>
        /// Full path to directory containing html files.
        /// </summary>
        [DataMemberIgnore]
        public string AssetDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "wwwroot");

        /// <summary>
        /// File path to main html file. Should be inside the AssetDirectory folder.
        /// </summary>
        [DataMemberIgnore]
        public string LoadingUrl { get; set; } = "http://google.com"; // file:///index.html

        [DataMemberIgnore]
        public string StartUrl { get; set; } = "http://google.com";
        //public string StartUrl { get; set; } = "http://localhost:5000/";

        #endregion

        #region Dependencies

        private ILogger Logger { get; set; }

        private IHostApplicationLifetime HostApplicationLifetime { get; }

        #endregion

        #region Relationships

        /// <summary>
        /// View created by Ultralight.
        /// </summary>
        protected ImpromptuNinjas.UltralightSharp.Safe.View View { get; set; }
        protected ImpromptuNinjas.UltralightSharp.Safe.Session session;
        protected Texture texture;
        protected SpriteFromTexture sprite;

        #region ImageElement

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
                //imageElement.CanBeHitByUser = visible; // REVIEW - is this needed?
            }
        }

        #endregion

        #endregion

        #region Construction and Destruction

        public UltralightUIScript()
        {
            Logger = DependencyContext.Current?.GetService<ILogger<UltralightUIScript>>() ?? (ILogger)Logging.Null.NullLogger.Instance;
            HostApplicationLifetime = DependencyContext.Current?.GetService<IHostApplicationLifetime>();
        }

        protected void InitUltralight()
        {
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
        }

        ~UltralightUIScript()
        {
            View?.Dispose();
            renderer?.Dispose();
        }

        #endregion

        #region State

        protected uint width;
        protected uint height;

        private bool loadedLoadingUrl = false;
        private bool IsWebServerAvailable => HostApplicationLifetime.ApplicationStarted.IsCancellationRequested;
        private bool startedLoadingStartUrl = false;
        private bool javascriptTestComplete = false;

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
        private bool visible;

        #endregion

        ComponentUI pendingInputEvents = new ComponentUI();

        #endregion

        #region Event Handlers

        private void OnPrivateWebServerStarted()
        {
            if (startedLoadingStartUrl) return;

            startedLoadingStartUrl = true;
            Logger.LogInformation($"Loading: {StartUrl}");
            View.LoadUrl(StartUrl);
        }

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
#if TRACE_TouchEvents
            Logger.LogTrace($"TouchDown {e.ScreenPosition} ({(int)(e.ScreenPosition.X * width)},{(int)(e.ScreenPosition.Y * height)}) {(Input.Mouse.DownButtons.Any() ? Input.Mouse.DownButtons.Select(m => m.ToString()).Aggregate((x, y) => $"{x},{y}") : "")}");
#endif

            pendingInputEvents.MouseEvents.Enqueue(new MouseEvent(MouseEventType.MouseDown, (int)(e.ScreenPosition.X * width), (int)(e.ScreenPosition.Y * height), MouseButton.Left));
        }

        private void ImageElement_TouchUp(object sender, Stride.UI.TouchEventArgs e)
        {
#if TRACE_TouchEvents
            Logger.LogTrace($"TouchUp {e.ScreenPosition} ({(int)(e.ScreenPosition.X * width)},{(int)(e.ScreenPosition.Y * height)})");
#endif
            pendingInputEvents.MouseEvents.Enqueue(new MouseEvent(MouseEventType.MouseUp, (int)(e.ScreenPosition.X * width), (int)(e.ScreenPosition.Y * height), MouseButton.Left));
        }

        #endregion

        #region Keyboard

        private void OnKeysPressed()
        {

            foreach (var key in Input.PressedKeys)
            {
                if (key == ToggleKey) continue;

                // DEPRECATE the char approach, if virtual key-code works

                string keyString = null;
                switch (key)
                {
                    case Stride.Input.Keys.D0:
                    case Stride.Input.Keys.D1:
                    case Stride.Input.Keys.D2:
                    case Stride.Input.Keys.D3:
                    case Stride.Input.Keys.D4:
                    case Stride.Input.Keys.D5:
                    case Stride.Input.Keys.D6:
                    case Stride.Input.Keys.D7:
                    case Stride.Input.Keys.D8:
                    case Stride.Input.Keys.D9:
                        keyString = key.ToString()[1].ToString();
                        break;

                    case Stride.Input.Keys.A:
                    case Stride.Input.Keys.B:
                    case Stride.Input.Keys.C:
                    case Stride.Input.Keys.D:
                    case Stride.Input.Keys.E:
                    case Stride.Input.Keys.F:
                    case Stride.Input.Keys.G:
                    case Stride.Input.Keys.H:
                    case Stride.Input.Keys.I:
                    case Stride.Input.Keys.J:
                    case Stride.Input.Keys.K:
                    case Stride.Input.Keys.L:
                    case Stride.Input.Keys.M:
                    case Stride.Input.Keys.N:
                    case Stride.Input.Keys.O:
                    case Stride.Input.Keys.P:
                    case Stride.Input.Keys.Q:
                    case Stride.Input.Keys.R:
                    case Stride.Input.Keys.S:
                    case Stride.Input.Keys.T:
                    case Stride.Input.Keys.U:
                    case Stride.Input.Keys.V:
                    case Stride.Input.Keys.W:
                    case Stride.Input.Keys.X:
                    case Stride.Input.Keys.Y:
                    case Stride.Input.Keys.Z:
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
                            new KeyEvent(ImpromptuNinjas.UltralightSharp.Enums.KeyEventType.Char, 0, 0, 0, ImpromptuString.Create(keyString), ImpromptuString.Create(keyString), false, false, false));
                    }
                    continue;
                }

                int nativeCode = key.ToWindowsVirtualKeyCode(); // TODO: Other platforms

                if (nativeCode != 0)
                {
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

            #region UIComponent

            var uiComponent = Entity.Get<UIComponent>();
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

            session = new Session(renderer, false, "");
            View = new View(renderer, width, height, true, session);
            View.SetFinishLoadingCallback((data, caller, frameId, isMainFrame, url) =>
            {
                loadedLoadingUrl = true;
                Logger.LogInformation($"Finished loading: {url}");
            }, default);

            #endregion

            #region LoadUrl

            if (!IsWebServerAvailable)
            {
                Logger.LogInformation("Web server not available yet.  Loading LoadingUrl.");
                View.LoadUrl(LoadingUrl);
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


            if (!javascriptTestComplete) { DoJavascriptTest(); }

            FireInputs(pendingInputEvents);

            if (ToggleKey.HasValue && Input.PressedKeys.Contains(ToggleKey.Value)) { Visible ^= true; }

            if (Input.HasPressedKeys) { OnKeysPressed(); }

            renderer.Update();
            renderer.Render();

            var surface = View.GetSurface();
            var bitmap = surface.GetBitmap();
            var pixels = bitmap.LockPixels();

            DataPointer dataPointer = new DataPointer((IntPtr)pixels, (int)bitmap.GetHeight() * (int)bitmap.GetWidth() * (int)bitmap.GetBpp());
            texture.SetData(this.Game.GraphicsContext.CommandList, dataPointer);
            sprite.Texture = texture;
            imageElement.Source = sprite;

            bitmap.UnlockPixels();
            bitmap.Dispose();

            void FireInputs(ComponentUI ui)
            {
                while (pendingInputEvents.MouseEvents.TryDequeue(out var e)) { View.FireMouseEvent(e); }
                while (pendingInputEvents.ScrollEvents.TryDequeue(out var e)) { View.FireScrollEvent(e); }
                while (pendingInputEvents.KeyboardEvents.TryDequeue(out var e)) { View.FireKeyEvent(e); }
            }
            void DoJavascriptTest()
            {
                javascriptTestComplete = true;
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

        #endregion Methods

        #region Classes

        public class ComponentUI
        {
            public ConcurrentQueue<MouseEvent> MouseEvents { get; set; } = new ConcurrentQueue<MouseEvent>();
            public ConcurrentQueue<KeyEvent> KeyboardEvents { get; set; } = new ConcurrentQueue<KeyEvent>();
            public ConcurrentQueue<ScrollEvent> ScrollEvents { get; set; } = new ConcurrentQueue<ScrollEvent>();
        }

        #endregion

        #region Logging

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

        #endregion
    }
}

#endif