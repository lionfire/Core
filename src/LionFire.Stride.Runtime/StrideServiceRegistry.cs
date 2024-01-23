using LionFire.Dependencies;
using Stride.Engine;
using Stride.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Stride3D
{
    /// <summary>
    /// Wraps DynamicServiceProvider and automatically finds the first Component in SceneInstance
    /// </summary>
    public class StrideServiceRegistry : IServiceProvider, IDisposable
    {

        #region Dependencies
        
        #region Game

        [SetOnce]
        public Game Game
        {
            get => game;
            set
            {
                if (game == value) return;
                if (game != default) throw new AlreadySetException();
                game = value;
            }
        }
        private Game game;

        #endregion

        #endregion

        #region Parameters

        public bool SearchComponents { get; set; } = false;
        
        #endregion

        public StrideServiceRegistry(Game game)
        {
            if (game == null) throw new ArgumentNullException();
            Game = game;
            dictionary.Add(typeof(InputManager), game.Input);
            // FUTURE: Add more
        }

        #region State

        Dictionary<Type, object> dictionary = new Dictionary<Type, object>();

        #endregion

        #region Derived

        //private ScriptComponent AnyScriptComponent => game?.SceneSystem?.SceneInstance?.SelectMany(e => e.Components).OfType<ScriptComponent>().FirstOrDefault();

        #endregion

        #region Methods

        public object GetService(Type serviceType)
        {
            var result = (dictionary ?? throw new ObjectDisposedException(nameof(StrideServiceRegistry))).TryGetValue(serviceType);

            if (result is EntityComponent ec && ec?.Entity.IsDisposed != false)
            {
                dictionary.Remove(serviceType);
                result = null;
            }
            if (result != null) return result;

            if (SearchComponents)
            {
                var sceneInstance = game.SceneSystem?.SceneInstance;
                if (sceneInstance != null)
                {
                    var instance = sceneInstance
                        .SelectMany(e => e.Components)
                        .Where(c => serviceType.IsAssignableFrom(c.GetType())).FirstOrDefault();

                    if (instance != null)
                    {
                        dictionary.Add(serviceType, instance);
                        return instance;
                    }
                }
            }
            return null;
        }

        public void Dispose() => dictionary = null;

        #endregion
    }
}
