#if DEPRECATED // Replaced with IUIRoot.QuerySubPath(this IHasPathCache<>)
using Caliburn.Micro;
using LionFire.Collections;
using System;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace LionFire.UI.Entities
{

    public interface IUIRegistry
    {
        T Query<T>(string path)
            where T : class, IUIKeyed;
        void OnPathChanged(string oldPath);
    }
        
    /// <summary>
    /// Caches queries to paths.
    /// Assumes Keys don't change on UINodes.
    /// </summary>
    public class UIRegistry : IUIRegistry
    {
#region Dependencies

        public IUIRoot RootPresenter { get; }

#endregion

#region Construction

        public UIRegistry(IUIRoot rootPresenter)
        {
            RootPresenter = rootPresenter;
        }

#endregion

#region State

        protected ConcurrentDictionary<string, IUIKeyed> nodesByPath = new ConcurrentDictionary<string, IUIKeyed>();

#endregion

#region (Public) Methods

        public T Query<T>(string path)
            where T : class, IUIKeyed
        {
            {
                if (nodesByPath.TryGetValue(path, out IUIKeyed result))
                {
                    return (T)result;
                }
            }

            {
                var result = RootPresenter.QuerySubPath<IUIKeyed>(path) as T;
                if (result != null)
                {
                    nodesByPath.AddOrUpdate(path, result, (k, v) => v);
                }
                return result;
            }
        }

        public void OnPathChanged(string oldPath) => nodesByPath.TryRemove(oldPath, out _);

#endregion
    }
}

#region OLD - startup task
//Func<Task> startup; // StartAsync waits for ctor tasks to finish
//startup = () => Task.Run(async () =>
//{
//    WindowSettings = await windowSettings.GetNonDefaultValue().ConfigureAwait(false);
//    // ENH Make this a Participant that contributes to CanStartShell?
//});

//await startup().ConfigureAwait(false);
//startup = null;
#endregion

#endif